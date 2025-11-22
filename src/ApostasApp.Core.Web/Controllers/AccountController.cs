using ApostasApp.Core.Application.DTOs.Apostadores;
using ApostasApp.Core.Application.DTOs.Usuarios;
using ApostasApp.Core.Application.Models;
using ApostasApp.Core.Application.Services.Interfaces.Identity;
using ApostasApp.Core.Application.Services.Interfaces.Usuarios;
using ApostasApp.Core.Domain.Interfaces.Notificacoes;
using ApostasApp.Core.Domain.Models.Notificacoes;
using ApostasApp.Core.Infrastructure.Notificacoes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Linq;
using System.Collections.Generic;
using System;

namespace ApostasApp.Core.Web.Controllers
{
  [ApiController]
  [Route("api/account")]
  public class AccountController : BaseController
  {
    private readonly IUsuarioService _usuarioService;
    private readonly IIdentityService _identityService;
    private readonly ILogger<AccountController> _logger;
    private readonly IConfiguration _configuration;
    private readonly INotificador _notificador;
    private readonly IWebHostEnvironment _environment;

    public AccountController(IUsuarioService usuarioService,
                            IIdentityService identityService,
                            ILogger<AccountController> logger,
                            IConfiguration configuration,
                            INotificador notificador,
                            IWebHostEnvironment environment)
                            : base(notificador)
    {
      _usuarioService = usuarioService;
      _identityService = identityService;
      _logger = logger;
      _configuration = configuration;
      _notificador = notificador;
      _environment = environment;
    }

  
    // **********************************************
    // MÉTODO DE ESQUECEU A SENHA (VERSÃO LIMPA/PADRÃO)
    // **********************************************
    [AllowAnonymous]
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
    {
      // === Validação PADRÃO: Confia no Model State (Corrigido globalmente no Program.cs) ===
      if (!ModelState.IsValid)
      {
        _logger.LogWarning("Esqueceu Senha: ModelState é inválido.");
        // Se a correção global funcionar, este método irá capturar o erro de validação corretamente.
        return CustomValidationProblem(ModelState);
      }

      // Logamos o e-mail (para debug)
      _logger.LogInformation($"Requisição de Esqueceu Senha para: {request.Email}");


      // Chama o serviço.
      var result = await _usuarioService.EsqueciMinhaSenhaAsync(
      request.Email,
      HttpContext.Request.Scheme,
      HttpContext.Request.Host.ToUriComponent()
      );

      if (result.Success)
      {
        _logger.LogInformation($"Instruções de redefinição de senha enviadas para {request.Email}.");
      }
      else
      {
        _logger.LogWarning($"Falha no envio de redefinição de senha para {request.Email}. Motivo: {result.Message}");
      }

      // Retorna o CustomResponse (200 OK)
      return CustomResponse(result);
    }
    // **********************************************


    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
      _logger.LogInformation($"Requisição de Registro recebida para: {request.Email}");

      if (!ModelState.IsValid)
      {
        return CustomValidationProblem(ModelState);
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


      if (registerResult.Success)
      {
        _logger.LogInformation($"Registro de {request.Email} realizado com sucesso.");
        return CustomResponse(registerResult);
      }
      else
      {
        _logger.LogWarning($"Registro de {request.Email} falhou.");
        return BadRequest(registerResult);
      }
    }


    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request)
    {
      _logger.LogInformation($"Requisição de Redefinição de Senha para Email: {request.Email}");

      if (!ModelState.IsValid)
      {
        _logger.LogWarning($"Redefinição de Senha: ModelState inválido. Erros de validação do DTO (incluindo Compare de senhas).");
        return CustomValidationProblem(ModelState);
      }

      var result = await _usuarioService.RedefinirSenhaAsync(request);

      if (result.Success)
      {
        _logger.LogInformation($"Senha redefinida com sucesso para UserId: {request.UserId}.");
      }
      else
      {
        _logger.LogWarning($"Falha na redefinição de senha para UserId: {request.UserId}.");
      }

      return CustomResponse(result);
    }


    [AllowAnonymous]
    [HttpPost("ConfirmEmail")]
    public async Task<ApiResponse<bool>> ConfirmEmail([FromBody] ConfirmEmailDto model)
    {
      try
      {
        if (string.IsNullOrEmpty(model.UserId) || string.IsNullOrEmpty(model.Code))
        {
          NotificarErro("Parâmetros de confirmação de e-mail inválidos.", "PARAMETROS_INVALIDOS");
        }

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

      var domainNotifications = _notificador.ObterNotificacoes().ToList();
      _notificador.LimparNotificacoes();

      var allNotifications = domainNotifications.Select(n => new NotificationDto
      {
        Codigo = n.Codigo,
        Tipo = n.Tipo,
        Mensagem = n.Mensagem,
        NomeCampo = n.NomeCampo
      }).ToList();

      var hasErrors = allNotifications.Any(n => n.Tipo == "Erro");

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
        return CustomValidationProblem(ModelState);
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
        _logger.LogWarning($"Falha no reenvio de confirmação de e-mail para {request.Email}. Motivo: {result.Message}");
      }

      return CustomResponse(result);
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
    {
      _logger.LogInformation($"Requisição de Alteração de Senha para usuário logado.");

      if (!ModelState.IsValid)
      {
        return CustomValidationProblem(ModelState);
      }

      var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
      if (string.IsNullOrEmpty(userId))
      {
        NotificarErro("Usuário não autenticado ou ID do usuário não encontrado no token.", "NAO_AUTENTICADO");
        return CustomResponse();
      }

      var result = await _usuarioService.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword);

      if (result.Success)
      {
        _logger.LogInformation($"Senha alterada com sucesso para UserId: {userId}.");
      }
      else
      {
        _logger.LogWarning($"Falha na alteração de senha para UserId: {userId}. Motivo: {result.Message}");
      }

      return CustomResponse(result);
    }

    [HttpPost("change-email-request")]
    [Authorize]
    public async Task<IActionResult> ChangeEmailRequest([FromBody] ChangeEmailRequestDto request)
    {
      _logger.LogInformation($"Requisição de Alteração de E-mail para novo e-mail: {request.NewEmail}");

      if (!ModelState.IsValid)
      {
        return CustomValidationProblem(ModelState);
      }

      var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
      if (string.IsNullOrEmpty(userId))
      {
        NotificarErro("Usuário não autenticado ou ID do usuário não encontrado no token.", "NAO_AUTENTICADO");
        return CustomResponse();
      }

      var result = await _usuarioService.ChangeEmail(userId, request.NewEmail);

      if (result.Success)
      {
        _logger.LogInformation($"E-mail de confirmação de alteração enviado para {request.NewEmail}.");
      }
      else
      {
        _logger.LogWarning($"Falha na solicitação de alteração de e-mail para UserId: {userId}. Motivo: {result.Message}");
      }

      return CustomResponse(result);
    }

    [HttpPost("confirm-change-email")]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmChangeEmail([FromBody] ConfirmChangeEmailRequestDto request)
    {
      _logger.LogInformation($"Requisição de Confirmação de Alteração de E-mail para UserId: {request.UserId}");

      if (!ModelState.IsValid)
      {
        return CustomValidationProblem(ModelState);
      }

      var result = await _usuarioService.ConfirmChangeEmail(request.UserId, request.NewEmail, request.Code);

      if (result.Success)
      {
        _logger.LogInformation($"E-mail alterado com sucesso para UserId: {request.UserId}.");
      }
      else
      {
        _logger.LogWarning($"Falha na confirmação de alteração de e-mail para UserId: {request.UserId}. Motivo: {result.Message}");
      }

      return CustomResponse(result);
    }

    [Authorize]
    [HttpGet("MeuPerfil")]
    public async Task<ActionResult<ApostadorDto>> MeuPerfil()
    {
      var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

      if (string.IsNullOrEmpty(userId))
      {
        NotificarErro("Usuário não autenticado ou ID do usuário não encontrado no token.", "NAO_AUTENTICADO");
        return CustomResponse<ApostadorDto>();
      }

      var apostadorProfile = await _usuarioService.GetUsuarioProfileAsync(userId);

      if (apostadorProfile == null)
      {
        NotificarErro("Perfil do apostador não encontrado ou não associado.", "PERFIL_NAO_ENCONTRADO");
        return CustomResponse<ApostadorDto>();
      }

      return CustomResponse(apostadorProfile);
    }

    /* método teste
    [HttpGet("GenerateTestHash")] 
    [AllowAnonymous]
    public async Task<IActionResult> GenerateTestHash()
    {
        var hashData = await _usuarioService.GenerateTestHashAsync();
        return Ok(hashData);
    }
    */
  }
}
