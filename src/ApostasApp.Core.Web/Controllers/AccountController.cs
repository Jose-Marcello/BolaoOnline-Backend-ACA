// Exemplo: C:\Projetos\BolaoOnline\BolaoOnlineApi\Controllers\AccountController.cs

using ApostasApp.Core.Application.DTOs.Apostadores;
using ApostasApp.Core.Application.DTOs.Usuarios; // Para LoginRequestDto, LoginResponseDto, RegisterRequestDto, RegisterResponse
using ApostasApp.Core.Application.Models; // Para ApiResponse
using ApostasApp.Core.Domain.Interfaces; // Para IUnitOfWork
using ApostasApp.Core.Domain.Interfaces.Notificacoes; // Para INotificador
using ApostasApp.Core.Domain.Models.Notificacoes; // Para NotificationDto (usar diretamente)
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using ApostasApp.Core.Application.Services.Interfaces.Usuarios; // Para IUsuarioService

namespace ApostasApp.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : BaseController
    {
        private readonly IUsuarioService _usuarioService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IUsuarioService usuarioService,
                                 ILogger<AccountController> logger,
                                 INotificador notificador, IUnitOfWork uow)
                                 : base(notificador, uow) // Passa notificador e uow para a BaseController
        {
            _usuarioService = usuarioService;
            _logger = logger;
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

            return CustomApiResponse(response); // Retorna a ApiResponse do serviço de forma consistente
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

            return CustomApiResponse(registerResult); // Retorna a ApiResponse do serviço de forma consistente
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
        {
            _logger.LogInformation($"Requisição de Esqueci Minha Senha para: {request.Email}");

            if (!ModelState.IsValid)
            {
                return CustomValidationProblem(ModelState); // Usa o método da BaseController
            }

            var scheme = HttpContext.Request.Scheme;
            var host = HttpContext.Request.Host.ToUriComponent();

            var result = await _usuarioService.EsqueciMinhaSenhaAsync(request.Email, scheme, host);

            if (result.Success)
            {
                _logger.LogInformation($"E-mail de redefinição de senha enviado para {request.Email}.");
            }
            else
            {
                _logger.LogWarning($"Falha na solicitação de redefinição de senha para {request.Email}.");
            }

            return CustomApiResponse(result); // Retorna a ApiResponse do serviço de forma consistente
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request)
        {
            _logger.LogInformation($"Requisição de Redefinição de Senha para UserId: {request.UserId}");

            if (!ModelState.IsValid)
            {
                return CustomValidationProblem(ModelState); // Usa o método da BaseController
            }

            var result = await _usuarioService.RedefinirSenhaAsync(request.UserId, request.Token, request.NewPassword);

            if (result.Success)
            {
                _logger.LogInformation($"Senha redefinida com sucesso para UserId: {request.UserId}.");
            }
            else
            {
                _logger.LogWarning($"Falha na redefinição de senha para UserId: {request.UserId}.");
            }

            return CustomApiResponse(result); // Retorna a ApiResponse do serviço de forma consistente
        }

        [HttpGet("confirm-email")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string code)
        {
            _logger.LogInformation($"Requisição de Confirmação de E-mail para UserId: {userId}");

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(code))
            {
                // Cria uma ApiResponse de falha para retornar de forma consistente
                var apiResponse = new ApiResponse<bool>(false, false);
                // CORRIGIDO: Usando NotificationDto
                Notificador.Handle(new NotificationDto { Codigo = "PARAMETROS_INVALIDOS", Tipo = "Erro", Mensagem = "Parâmetros de confirmação de e-mail inválidos." });
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return CustomApiResponse(apiResponse);
            }

            var result = await _usuarioService.ConfirmEmail(userId, code);

            if (result.Success)
            {
                _logger.LogInformation($"E-mail confirmado com sucesso para UserId: {userId}.");
            }
            else
            {
                _logger.LogWarning($"Falha na confirmação de e-mail para UserId: {userId}.");
            }

            return CustomApiResponse(result); // Retorna a ApiResponse do serviço de forma consistente
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

            return CustomApiResponse(result); // Retorna a ApiResponse do serviço de forma consistente
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
                // Cria uma ApiResponse de falha para retornar de forma consistente
                var apiResponse = new ApiResponse<bool>(false, false);
                // CORRIGIDO: Usando NotificationDto
                Notificador.Handle(new NotificationDto { Codigo = "NAO_AUTENTICADO", Tipo = "Erro", Mensagem = "Usuário não autenticado ou ID do usuário não encontrado no token." });
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return CustomApiResponse(apiResponse);
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

            return CustomApiResponse(result); // Retorna a ApiResponse do serviço de forma consistente
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
                // Cria uma ApiResponse de falha para retornar de forma consistente
                var apiResponse = new ApiResponse<object>(false, null); // Pode ser object ou AuthResult
                // CORRIGIDO: Usando NotificationDto
                Notificador.Handle(new NotificationDto { Codigo = "NAO_AUTENTICADO", Tipo = "Erro", Mensagem = "Usuário não autenticado ou ID do usuário não encontrado no token." });
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return CustomApiResponse(apiResponse);
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

            return CustomApiResponse(result); // Retorna a ApiResponse do serviço de forma consistente
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

            return CustomApiResponse(result); // Retorna a ApiResponse do serviço de forma consistente
        }

        [Authorize]
        [HttpGet("MeuPerfil")]
        public async Task<ActionResult<ApostadorDto>> MeuPerfil()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                // Retorna Unauthorized diretamente, pois não há ApiResponse para encapsular aqui
                return Unauthorized("Usuário não autenticado ou ID do usuário não encontrado no token.");
            }

            var apostadorProfile = await _usuarioService.GetUsuarioProfileAsync(userId);

            if (apostadorProfile == null)
            {
                // Retorna NotFound diretamente
                return NotFound("Perfil do apostador não encontrado ou não associado.");
            }

            return Ok(apostadorProfile); // Retorna o DTO diretamente
        }
    }
}
