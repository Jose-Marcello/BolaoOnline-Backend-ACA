using ApostasApp.Core.Application.DTOs.Apostadores;
using ApostasApp.Core.Application.DTOs.Usuarios; // Para LoginRequestDto, LoginResponseDto, RegisterRequestDto, ResetPasswordRequestDto, etc.
using ApostasApp.Core.Application.Models;
using ApostasApp.Core.Application.Services.Interfaces.Usuarios; // Para IUsuarioService
using ApostasApp.Core.Domain.Interfaces.Notificacoes; // Para INotificador
using ApostasApp.Core.Domain.Models.Notificacoes;
using ApostasApp.Core.Infrastructure.Notificacoes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding; // Para ModelStateDictionary
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace ApostasApp.Core.Web.Controllers
{
  [ApiController]
  // Rota fixa e minúscula para ACCOUNT. Elimina o [controller] que pode causar o erro.
  [Route("api/account")]
  public class AccountController : BaseController
  {
    private readonly IUsuarioService _usuarioService;
    private readonly ILogger<AccountController> _logger;
    private readonly IConfiguration _configuration;
    private readonly INotificador _notificador;

    // CONSTRUTOR CORRIGIDO: Remove IUnitOfWork da injeção no Controller
    public AccountController(IUsuarioService usuarioService,
                                 ILogger<AccountController> logger,
                                 IConfiguration configuration,
                                 INotificador notificador)
                                 : base(notificador) // Passa apenas o notificador para a BaseController
    {
      _usuarioService = usuarioService;
      _logger = logger;
      _configuration = configuration;
      _notificador = notificador;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
      _logger.LogInformation($"Tentativa de login para o e-mail: {request.Email}");

      if (!ModelState.IsValid)
      {
        _logger.LogWarning("Login: ModelState é inválido.");
        foreach (var state in ModelState)
        {
          foreach (var error in state.Value.Errors)
          {
            _logger.LogWarning($"Erro no ModelState: {state.Key} - {error.ErrorMessage}");
          }
        }
        return CustomValidationProblem(ModelState); // Usa o método da BaseController
      }

      var response = await _usuarioService.LoginAsync(request);

      _logger.LogInformation($"Login: Resposta do UsuarioService - Sucesso: {response.Success}");
      if (response.Notifications != null)
      {
        foreach (var notification in response.Notifications)
        {
          _logger.LogInformation($"Notificação de Login: Codigo={notification.Codigo}, Tipo={notification.Tipo}, Mensagem={notification.Mensagem}, NomeCampo={notification.NomeCampo}");
        }
      }

      // CORRIGIDO: Usando CustomResponse
      return CustomResponse(response); // Retorna a ApiResponse do serviço de forma consistente
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
      _logger.LogInformation($"Requisição de Registro recebida para: {request.Email}");

      if (!ModelState.IsValid)
      {
        return CustomValidationProblem(ModelState); // Usa o método da BaseController
      }

      var registerResult = await _usuarioService.RegisterAsync(request);

      if (registerResult.Success)
      {
        _logger.LogInformation($"Registro de {request.Email} realizado com sucesso.");
      }
      else
      {
        _logger.LogWarning($"Registro de {request.Email} falhou.");
      }


      // <<-- CORREÇÃO: RETORNA O STATUS HTTP CORRETO -->>
      if (registerResult.Success)
      {
        _logger.LogInformation($"Registro de {request.Email} realizado com sucesso.");
        // Retorna Status 200 OK com a resposta
        return CustomResponse(registerResult);
      }
      else
      {
        _logger.LogWarning($"Registro de {request.Email} falhou.");
        // Retorna Status 400 Bad Request ou similar com a resposta
        return BadRequest(registerResult);
      }
    }


    // Localização: ApostasApp.Core.Web.Controllers/AccountController.cs

    [HttpPost("forgotpassword")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
    {
      // O seu código CustomValidationProblem pode ser inserido aqui
      if (!ModelState.IsValid)
      {
        return CustomValidationProblem(ModelState);
      }

      // 🎯 CORREÇÃO APLICADA AQUI: Mudando de "FrontendUrls:ResetUrls" para "FrontendUrls:BaseUrl"
      var baseUrl = _configuration.GetValue<string>("FrontendUrls:BaseUrl");

      // O SERVIÇO RETORNA O OBJETO COMPLETO DE RESPOSTA (ApiResponse<string>)
      var response = await _usuarioService.EsqueciMinhaSenhaAsync(request.Email, baseUrl);

      // O Controller agora apenas retorna o objeto de resposta.
      return CustomResponse(response);
    }


    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request)
    {
      // O DTO ResetPasswordRequestDto agora inclui UserId, Token, Email, NewPassword e ConfirmNewPassword.
      _logger.LogInformation($"Requisição de Redefinição de Senha para Email: {request.Email}");

      if (!ModelState.IsValid)
      {
        _logger.LogWarning($"Redefinição de Senha: ModelState inválido. Erros de validação do DTO (incluindo Compare de senhas).");
        return CustomValidationProblem(ModelState); // Usa o método da BaseController (retorna 400 com detalhes)
      }

      // 🎯 CORREÇÃO CRÍTICA APLICADA: Passa o DTO completo para o serviço,
      // que agora é responsável por decodificar o Token e buscar o usuário.
      var result = await _usuarioService.RedefinirSenhaAsync(request);

      if (result.Success)
      {
        _logger.LogInformation($"Senha redefinida com sucesso para UserId: {request.UserId}.");
      }
      else
      {
        _logger.LogWarning($"Falha na redefinição de senha para UserId: {request.UserId}.");
      }

      // CORRIGIDO: Usando CustomResponse
      return CustomResponse(result); // Retorna a ApiResponse do serviço de forma consistente
    }


    // Localização: ApostasApp.Core.Web/Controllers/AccountController.cs


    [AllowAnonymous]
    [HttpPost("ConfirmEmail")]
    public async Task<ApiResponse<bool>> ConfirmEmail([FromBody] ConfirmEmailDto model)
    {
      try
      {
        // 1. Validação inicial dos parâmetros
        if (string.IsNullOrEmpty(model.UserId) || string.IsNullOrEmpty(model.Code))
        {
          NotificarErro("Parâmetros de confirmação de e-mail inválidos.", "PARAMETROS_INVALIDOS");
        }

        // 2. Chama o serviço para processar a confirmação
        //    (A chamada é feita somente se não houver erros de validação inicial)
        var result = new ApiResponse<bool>();
        if (OperacaoValida())
        {
          result = await _usuarioService.ConfirmEmail(model.UserId, model.Code);

          _logger.LogInformation($"Requisição de Confirmação de E-mail para UserId: {model.UserId}");

          if (result.Success)
          {
            _logger.LogInformation($"E-mail confirmado com sucesso para UserId: {model.UserId}.");
            NotificarSucesso("E-mail confirmado com sucesso!");
          }
          else
          {
            _logger.LogWarning($"Falha na confirmação de e-mail para UserId: {model.UserId}.");
          }
        }
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Ocorreu um erro inesperado durante a confirmação de e-mail.");
        NotificarErro("Ocorreu um erro inesperado.", "ERRO_INESPERADO");
      }

      // 3. Coleta e retorna a resposta final
      //    Coleta as notificações de domínio
      var domainNotifications = _notificador.ObterNotificacoes().ToList();

      //    Limpa as notificações após a coleta (boa prática)
      _notificador.LimparNotificacoes();

      //    Converte as notificações de domínio para DTOs
      var allNotifications = domainNotifications.Select(n => new NotificationDto
      {
        Codigo = n.Codigo,
        Tipo = n.Tipo,
        Mensagem = n.Mensagem,
        NomeCampo = n.NomeCampo
      }).ToList();

      var hasErrors = allNotifications.Any(n => n.Tipo == "Erro");

      //    Retorna a resposta final com base no sucesso da operação
      return new ApiResponse<bool>
      {
        Success = !hasErrors,
        Data = !hasErrors,
        Notifications = allNotifications
      };
    }

    [HttpPost("resend-email-confirmation")]
    [AllowAnonymous]
    public async Task<IActionResult> ResendEmailConfirmation([FromBody] ResendEmailConfirmationRequestDto request)
    {
      _logger.LogInformation($"Requisição de Reenvio de Confirmação de E-mail para: {request.Email}");

      if (!ModelState.IsValid)
      {
        return CustomValidationProblem(ModelState); // Usa o método da BaseController
      }

      var scheme = HttpContext.Request.Scheme;
      var host = HttpContext.Request.Host.ToUriComponent();

      var result = await _usuarioService.ResendEmailConfirmationAsync(request.Email, scheme, host);

      if (result.Success)
      {
        _logger.LogInformation($"Novo e-mail de confirmação enviado para {request.Email}.");
      }
      else
      {
        _logger.LogWarning($"Falha no reenvio de confirmação de e-mail para {request.Email}.");
      }

      // CORRIGIDO: Usando CustomResponse
      return CustomResponse(result); // Retorna a ApiResponse do serviço de forma consistente
    }

    [HttpPost("change-password")]
    [Authorize] // Requer que o usuário esteja logado
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
    {
      _logger.LogInformation($"Requisição de Alteração de Senha para usuário logado.");

      if (!ModelState.IsValid)
      {
        return CustomValidationProblem(ModelState); // Usa o método da BaseController
      }

      var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
      if (string.IsNullOrEmpty(userId))
      {
        // CORRIGIDO: Usando NotificarErro do BaseController
        NotificarErro("Usuário não autenticado ou ID do usuário não encontrado no token.", "NAO_AUTENTICADO");
        return CustomResponse(); // Retorna a resposta padronizada com as notificações
      }

      var result = await _usuarioService.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword);

      if (result.Success)
      {
        _logger.LogInformation($"Senha alterada com sucesso para UserId: {userId}.");
      }
      else
      {
        _logger.LogWarning($"Falha na alteração de senha para UserId: {userId}.");
      }

      // CORRIGIDO: Usando CustomResponse
      return CustomResponse(result); // Retorna a ApiResponse do serviço de forma consistente
    }

    [HttpPost("change-email-request")]
    [Authorize] // Requer que o usuário esteja logado
    public async Task<IActionResult> ChangeEmailRequest([FromBody] ChangeEmailRequestDto request)
    {
      _logger.LogInformation($"Requisição de Alteração de E-mail para novo e-mail: {request.NewEmail}");

      if (!ModelState.IsValid)
      {
        return CustomValidationProblem(ModelState); // Usa o método da BaseController
      }

      var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
      if (string.IsNullOrEmpty(userId))
      {
        // CORRIGIDO: Usando NotificarErro do BaseController
        NotificarErro("Usuário não autenticado ou ID do usuário não encontrado no token.", "NAO_AUTENTICADO");
        return CustomResponse(); // Retorna a resposta padronizada com as notificações
      }

      var result = await _usuarioService.ChangeEmail(userId, request.NewEmail);

      if (result.Success)
      {
        _logger.LogInformation($"E-mail de confirmação de alteração enviado para {request.NewEmail}.");
      }
      else
      {
        _logger.LogWarning($"Falha na solicitação de alteração de e-mail para UserId: {userId}.");
      }

      // CORRIGIDO: Usando CustomResponse
      return CustomResponse(result); // Retorna a ApiResponse do serviço de forma consistente
    }

    [HttpPost("confirm-change-email")]
    [AllowAnonymous] // O link de confirmação pode ser acessado sem estar logado
    public async Task<IActionResult> ConfirmChangeEmail([FromBody] ConfirmChangeEmailRequestDto request)
    {
      _logger.LogInformation($"Requisição de Confirmação de Alteração de E-mail para UserId: {request.UserId}");

      if (!ModelState.IsValid)
      {
        return CustomValidationProblem(ModelState); // Usa o método da BaseController
      }

      var result = await _usuarioService.ConfirmChangeEmail(request.UserId, request.NewEmail, request.Code);

      if (result.Success)
      {
        _logger.LogInformation($"E-mail alterado com sucesso para UserId: {request.UserId}.");
      }
      else
      {
        _logger.LogWarning($"Falha na confirmação de alteração de e-mail para UserId: {request.UserId}.");
      }

      // CORRIGIDO: Usando CustomResponse
      return CustomResponse(result); // Retorna a ApiResponse do serviço de forma consistente
    }

    [Authorize]
    [HttpGet("MeuPerfil")]
    public async Task<ActionResult<ApostadorDto>> MeuPerfil()
    {
      var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

      if (string.IsNullOrEmpty(userId))
      {
        // CORRIGIDO: Usando NotificarErro e CustomResponse para consistência
        NotificarErro("Usuário não autenticado ou ID do usuário não encontrado no token.", "NAO_AUTENTICADO");
        return CustomResponse<ApostadorDto>(); // Retorna uma ApiResponse de erro com o tipo correto
      }

      var apostadorProfile = await _usuarioService.GetUsuarioProfileAsync(userId);

      if (apostadorProfile == null)
      {
        // CORRIGIDO: Usando NotificarErro e CustomResponse para consistência
        NotificarErro("Perfil do apostador não encontrado ou não associado.", "PERFIL_NAO_ENCONTRADO");
        return CustomResponse<ApostadorDto>(); // Retorna uma ApiResponse de erro com o tipo correto
      }

      // CORRIGIDO: Usando CustomResponse
      return CustomResponse(apostadorProfile); // Retorna o DTO encapsulado em ApiResponse de sucesso
    }


    /* método teste
    [HttpGet("GenerateTestHash")] // Rota de teste
    [AllowAnonymous]
    public async Task<IActionResult> GenerateTestHash()
    {
       // ASSUMINDO QUE _identityService ESTÁ INJETADO NO CONSTRUTOR!

       var hashData = await _usuarioService.GenerateTestHashAsync();

       return Ok(hashData);


    }
    */

  }
}
