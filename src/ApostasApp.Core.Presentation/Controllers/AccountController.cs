using ApostasApp.Core.Application.DTOs.Usuarios;
using ApostasApp.Core.Domain.Interfaces.Notificacoes;
using ApostasApp.Core.Domain.Interfaces.Usuarios;
using ApostasApp.Core.Domain.Models.Configuracoes;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging; // <<<--- ADICIONE ESTE USING
using System;
using System.Threading.Tasks;

namespace ApostasApp.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : BaseController
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IMapper _mapper;
        private readonly FrontendUrlsSettings _frontendUrls;
        private readonly ILogger<AccountController> _logger; // <<<--- NOVA PROPRIEDADE PARA LOGGING

        public AccountController(IUsuarioService usuarioService,
                                 INotificador notificador,
                                 IMapper mapper,
                                 IOptions<FrontendUrlsSettings> frontendUrlsOptions,
                                 ILogger<AccountController> logger) : base(notificador) // <<<--- NOVO PARÂMETRO PARA LOGGING
        {
            _usuarioService = usuarioService;
            _mapper = mapper;
            _frontendUrls = frontendUrlsOptions.Value;
            _logger = logger; // <<<--- ATRIBUINDO O LOGGER
        }

        /// <summary>
        /// Registra um novo usuário no sistema.
        /// </summary>
        /// <param name="request">Os dados de registro do usuário.</param>
        /// <returns>Um status HTTP indicando sucesso ou falha, com notificações.</returns>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            _logger.LogInformation($"Requisição de Registro recebida para: {request.Email}"); // Log de entrada
            if (!ModelState.IsValid)
            {
                return CustomResponse(ModelState);
            }

            var registrationSucceeded = await _usuarioService.RegistrarNovoUsuario(
                request.Email,
                request.Password,
                request.Apelido,
                request.CPF.Replace(".", "").Replace("-", ""),
                request.Celular,
                Request.Scheme,
                Request.Host.ToString()
            );

            if (TemNotificacao())
            {
                _logger.LogWarning($"Registro de {request.Email} falhou com notificações."); // Log de falha
                return CustomResponse();
            }

            Notificar("Sucesso", "Registro realizado com sucesso. Verifique seu e-mail para confirmação.");
            _logger.LogInformation($"Registro de {request.Email} realizado com sucesso."); // Log de sucesso
            return CustomResponse();
        }

        /// <summary>
        /// Confirma o e-mail de um usuário.
        /// </summary>
        /// <param name="userId">O ID do usuário.</param>
        /// <param name="code">O código de confirmação.</param>
        /// <returns>Status HTTP indicando sucesso ou falha.</returns>
        [HttpGet("confirm-email")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string code)
        {
            _logger.LogInformation($"Requisição de Confirmação de E-mail recebida. UserId: {userId}, Code: {code}"); // <<<--- LOG AQUI!
            // Definir um breakpoint AQUI (na linha abaixo)!
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(code))
            {
                Notificar("Erro", "Parâmetros de confirmação inválidos.");
                _logger.LogError($"Parâmetros de confirmação inválidos. UserId: {userId}, Code: {code}"); // Log de erro
                return Redirect($"{_frontendUrls.BaseUrl}/error");
            }

            var confirmed = await _usuarioService.ConfirmEmail(userId, code);

            if (TemNotificacao())
            {
                _logger.LogError($"Confirmação de e-mail para UserId {userId} falhou com notificações."); // Log de erro
                return Redirect($"{_frontendUrls.BaseUrl}/error");
            }

            Notificar("Sucesso", "E-mail confirmado com sucesso.");
            _logger.LogInformation($"E-mail para UserId {userId} confirmado com sucesso."); // Log de sucesso
            return Redirect($"{_frontendUrls.BaseUrl}/login");
        }

        // ... (restante do código do seu AccountController.cs) ...
        /// <summary>
        /// Tenta realizar o login de um usuário.
        /// </summary>
        /// <param name="request">Credenciais de login.</param>
        /// <returns>Um status HTTP indicando sucesso ou falha no login, e talvez um token JWT.</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            _logger.LogInformation($"Requisição de Login recebida para: {request.Email}");
            if (!ModelState.IsValid)
            {
                return CustomResponse(ModelState);
            }

            var loginResult = await _usuarioService.Login(
                request.Email,
                request.Password,
                request.IsPersistent
            );

            if (TemNotificacao() || !loginResult.Success)
            {
                _logger.LogWarning($"Login de {request.Email} falhou.");
                return CustomResponse();
            }

            Notificar("Sucesso", "Login realizado com sucesso.");
            _logger.LogInformation($"Login de {request.Email} realizado com sucesso.");
            return CustomResponse();
        }

        /// <summary>
        /// Realiza o logout do usuário.
        /// </summary>
        /// <returns>Status HTTP de sucesso.</returns>
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            _logger.LogInformation($"Requisição de Logout recebida.");
            await _usuarioService.RealizarLogout();
            if (TemNotificacao())
            {
                _logger.LogWarning($"Logout falhou com notificações.");
                return CustomResponse();
            }
            Notificar("Sucesso", "Logout realizado com sucesso.");
            _logger.LogInformation($"Logout realizado com sucesso.");
            return CustomResponse();
        }

        /// <summary>
        /// Inicia o processo de redefinição de senha.
        /// </summary>
        /// <param name="request">O email do usuário.</param>
        /// <returns>Status HTTP indicando se o e-mail foi enviado (sem revelar se o usuário existe).</returns>
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
        {
            _logger.LogInformation($"Requisição de ForgotPassword recebida para: {request.Email}");
            if (!ModelState.IsValid)
            {
                return CustomResponse(ModelState);
            }

            await _usuarioService.EsqueciMinhaSenhaAsync(request.Email, Request.Scheme, Request.Host.ToString());

            Notificar("Sucesso", "Se o email estiver cadastrado, um link para redefinição de senha foi enviado.");
            _logger.LogInformation($"Link de redefinição de senha para {request.Email} processado.");
            return CustomResponse();
        }

        /// <summary>
        /// Redefine a senha do usuário usando um token.
        /// </summary>
        /// <param name="request">Dados para redefinição de senha (userId, token, nova senha).</param>
        /// <returns>Status HTTP indicando sucesso ou falha.</returns>
        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request)
        {
            _logger.LogInformation($"Requisição de ResetPassword recebida para UserId: {request.UserId}");
            if (!ModelState.IsValid)
            {
                return CustomResponse(ModelState);
            }

            var resetSucceeded = await _usuarioService.RedefinirSenhaAsync(
                request.UserId,
                request.Token,
                request.NewPassword
            );

            if (TemNotificacao())
            {
                _logger.LogError($"Redefinição de senha para UserId {request.UserId} falhou com notificações.");
                return Redirect($"{_frontendUrls.BaseUrl}/error");
            }

            Notificar("Sucesso", "Senha redefinida com sucesso.");
            _logger.LogInformation($"Senha redefinida com sucesso para UserId {request.UserId}.");
            return Redirect($"{_frontendUrls.BaseUrl}/login");
        }

        /// <summary>
        /// Reenvia o e-mail de confirmação.
        /// </summary>
        /// <param name="request">O email do usuário.</param>
        /// <returns>Status HTTP indicando sucesso ou falha.</returns>
        [HttpPost("resend-email-confirmation")]
        [AllowAnonymous]
        public async Task<IActionResult> ResendEmailConfirmation([FromBody] ResendEmailConfirmationRequestDto request)
        {
            _logger.LogInformation($"Requisição de ResendEmailConfirmation recebida para: {request.Email}");
            if (!ModelState.IsValid)
            {
                return CustomResponse(ModelState);
            }

            var resendSucceeded = await _usuarioService.ResendEmailConfirmationAsync(request.Email, Request.Scheme, Request.Host.ToString());

            if (TemNotificacao())
            {
                _logger.LogError($"Reenvio de e-mail de confirmação para {request.Email} falhou com notificações.");
                return CustomResponse();
            }

            Notificar("Sucesso", "E-mail de confirmação reenviado com sucesso.");
            _logger.LogInformation($"E-mail de confirmação reenviado com sucesso para {request.Email}.");
            return CustomResponse();
        }

        /// <summary>
        /// Obtém informações do perfil do usuário logado.
        /// </summary>
        /// <returns>O DTO do usuário logado.</returns>
        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetUserProfile()
        {
            _logger.LogInformation($"Requisição de GetUserProfile recebida.");
            var user = await _usuarioService.GetLoggedInUser();

            if (user == null)
            {
                Notificar("Erro", "Usuário não encontrado.");
                _logger.LogError("Perfil do usuário não encontrado.");
                return CustomResponse();
            }

            var userProfileDto = _mapper.Map<UsuarioProfileDto>(user);
            _logger.LogInformation($"Perfil do usuário {user.Email} obtido com sucesso.");
            return CustomResponse(userProfileDto);
        }
    }
}
