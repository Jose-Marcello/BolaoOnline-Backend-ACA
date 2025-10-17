// Localização: ApostasApp.Core.Application.Services.Usuarios/UsuarioService.cs

// Usings necessários
using ApostasApp.Core.Application.DTOs.Apostadores;
using ApostasApp.Core.Application.DTOs.Financeiro;
using ApostasApp.Core.Application.DTOs.Usuarios;
using ApostasApp.Core.Application.Models; // Para ApiResponse
using ApostasApp.Core.Application.Services.Interfaces.Apostadores;
using ApostasApp.Core.Application.Services.Interfaces.Usuarios;
using ApostasApp.Core.Application.Utils;
using ApostasApp.Core.Domain.Interfaces;
using ApostasApp.Core.Domain.Interfaces.Apostadores;
using ApostasApp.Core.Domain.Interfaces.Identity;
using ApostasApp.Core.Domain.Interfaces.Notificacoes;
using ApostasApp.Core.Domain.Models.Apostadores;
using ApostasApp.Core.Domain.Models.Financeiro;
using ApostasApp.Core.Domain.Models.Identity; // MUITO IMPORTANTE: ESTE USING DEVE ESTAR AQUI!
using ApostasApp.Core.Domain.Models.Notificacoes; // Para Notificacao (classe de domínio)
using ApostasApp.Core.Domain.Models.Usuarios; // Para a classe Usuario
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ApostasApp.Core.Application.Services.Usuarios
{
    public class UsuarioService : BaseService, IUsuarioService
    {
        private readonly IIdentityService _identityService;
        private readonly IApostadorRepository _apostadorRepository;
        private readonly IApostadorService _apostadorService;
        private readonly UserManager<Usuario> _userManager;
        private readonly ILogger<UsuarioService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public UsuarioService(IIdentityService identityService,
                                     IApostadorRepository apostadorRepository,
                                     IApostadorService apostadorService,
                                     INotificador notificador,
                                     IUnitOfWork uow,
                                     UserManager<Usuario> userManager,
                                     ILogger<UsuarioService> logger,
                                     IConfiguration configuration,
                                     IMapper mapper)
            : base(notificador, uow)
        {
            _identityService = identityService;
            _apostadorRepository = apostadorRepository;
            _apostadorService = apostadorService;
            _userManager = userManager;
            _logger = logger;
            _configuration = configuration;
            _mapper = mapper;
        }

        // Localização: ApostasApp.Core.Application.Services.Usuarios/UsuarioService.cs

        // Localização: ApostasApp.Core.Application.Services.Usuarios/UsuarioService.cs

        // Localização: ApostasApp.Core.Application.Services.Usuarios/UsuarioService.cs

        public async Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto request)
        {
            var apiResponse = new ApiResponse<LoginResponseDto> { Notifications = new List<NotificationDto>() };
            var responseData = new LoginResponseDto { LoginSucesso = false };

            _logger.LogInformation("Iniciando LoginAsync no UsuarioService.");

            try
            {
                // PASSO 1: Busca o usuário pelo e-mail.
                var user = await _identityService.GetUserByEmailAsync(request.Email);

                // Se o usuário não for encontrado, falha o login com mensagem genérica.
                if (user == null)
                {
                    Notificar("Erro", "Usuário ou senha inválidos.");
                    apiResponse.Success = false;
                    apiResponse.Notifications.AddRange(ObterNotificacoesParaResposta());
                    apiResponse.Data = responseData;
                    return apiResponse;
                }

                // PASSO 2: VERIFICAÇÃO CRUCIAL - Checa se o e-mail está confirmado.
                if (_userManager.Options.SignIn.RequireConfirmedAccount && !user.EmailConfirmed)
                {
                    Notificar("Erro", "Sua conta ainda não foi confirmada. Por favor, verifique sua caixa de entrada.");
                    apiResponse.Success = false;
                    apiResponse.Notifications.AddRange(ObterNotificacoesParaResposta());
                    apiResponse.Data = responseData;
                    return apiResponse;
                }

                // PASSO 3: Tenta fazer o login com a senha.
                // A sua lógica original que eu removi está certa, vamos usá-la.
                var loginResult = await _identityService.LoginAsync(request.Email, request.Password, request.IsPersistent);

                if (loginResult.Success)
                {
                    // Login bem-sucedido.
                    responseData.LoginSucesso = true;
                    responseData.Token = loginResult.Token;
                    responseData.RefreshToken = loginResult.RefreshToken;
                    responseData.Expiration = loginResult.Expiration;
                    responseData.Apelido = user.Apelido;
                    responseData.Email = user.Email;
                    responseData.UserId = user.Id;

                    user.LastLoginDate = DateTime.Now;
                    await _userManager.UpdateAsync(user);

                    Notificar("Sucesso", "Login realizado com sucesso.");
                    apiResponse.Success = true;
                    apiResponse.Data = responseData;
                }
                else
                {
                    // O login falhou por outros motivos.
                    if (loginResult.Notifications != null && loginResult.Notifications.Any())
                    {
                        apiResponse.Notifications.AddRange(loginResult.Notifications
                            .Select(n => new NotificationDto { Codigo = n.Codigo, Tipo = n.Tipo, Mensagem = n.Mensagem, NomeCampo = n.NomeCampo }));
                    }
                    else
                    {
                        Notificar("Erro", "Usuário ou senha inválidos.");
                    }

                    apiResponse.Success = false;
                    apiResponse.Data = responseData;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"EXCEÇÃO NO LOGIN (LoginAsync Service): {ex.Message}");
                Notificar("Erro", "Ocorreu um erro inesperado durante o login. Por favor, tente novamente mais tarde.");
                apiResponse.Success = false;
                apiResponse.Data = responseData;
            }

            apiResponse.Notifications.AddRange(ObterNotificacoesParaResposta());
            return apiResponse;
        }

        public async Task<ApiResponse<RegisterResponse>> RegisterAsync(RegisterRequestDto request)
        {
            var apiResponse = new ApiResponse<RegisterResponse>();
            try
            {
                var registrationSuccess = await RegistrarNovoUsuario(request);
                //var registrationSuccess = await RegistrarNovoUsuario(
                //    request.Email, request.Password, request.Apelido, request.Cpf, 
                //    request.Celular, request.Scheme, request.Host, request.NomeCompleto);

                if (registrationSuccess)
                {
                    var user = await _identityService.GetUserByEmailAsync(request.Email);
                    if (user != null)
                    {
                        apiResponse.Success = true;
                        apiResponse.Data = new RegisterResponse
                        {
                            UserId = user.Id,
                            Email = user.Email,
                            Apelido = user.Apelido
                        };
                        Notificar("Sucesso", "Registro realizado com sucesso!");
                    }
                    else
                    {
                        Notificar("Erro", "Usuário registrado, mas detalhes não encontrados para retorno.");
                    }
                }
                else
                {
                    Notificar("Erro", "Falha ao registrar usuário. Verifique os dados informados.");
                }
            }
            catch (Exception ex)
            {
                Notificar("Erro", $"Ocorreu um erro inesperado durante o registro: {ex.Message}");
                _logger.LogError(ex, $"EXCEÇÃO NO REGISTRO (RegisterAsync Service): {ex.Message}");
            }

            apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
            if (!apiResponse.Success && (apiResponse.Notifications == null || !apiResponse.Notifications.Any()))
            {
                apiResponse.Notifications.Add(new NotificationDto { Codigo = "GENERIC_ERROR", Tipo = "Erro", Mensagem = "Ocorreu um erro inesperado." });
            }

            return apiResponse;
        }


    // Localização: ApostasApp.Core.Application.Services.Usuarios/UsuarioService.cs (Implementação)
    // NOTE: Assume-se que UsuarioService herda de BaseService.

    public async Task<ApiResponse<string>> EsqueciMinhaSenhaAsync(string email, string baseUrl)
    {
      // A chamada ao IdentityService (que agora retorna o link/token como string)
      var resetLink = await _identityService.ForgotPasswordAsync(email, baseUrl);

      // 1. Lógica para o caso de o IdentityService retornar NULL/String vazia (medida de segurança)
      if (string.IsNullOrEmpty(resetLink))
      {
        // Notificação de segurança (não revela se o usuário existe)
        Notificar("Sucesso", "Se o e-mail estiver cadastrado, as instruções para redefinição de senha foram enviadas.");

        // Retorna ApiResponse de sucesso, mas com Data nula (string) e as notificações.
        // O método CustomResponse do BaseService será chamado no Controller.
        return new ApiResponse<string>(true, "Solicitação processada.", null, ObterNotificacoesParaResposta().ToList());
      }

      // 2. Lógica para DEBUG CRÍTICO (O Token foi gerado e está no link)

      // Notificamos que o e-mail foi enviado (mesmo que seja mock)
      Notificar("Sucesso", "Instruções para redefinição de senha enviadas (Link gerado para DEBUG).");

      // Retorna o ApiResponse com o link como Data (string).
      // A Controller irá pegar esta Data e retorná-la como Status 200 OK no corpo da API.
      return new ApiResponse<string>(true, "Link de redefinição obtido.", resetLink, ObterNotificacoesParaResposta().ToList());
    }

    /*
    public async Task<ApiResponse<bool>> EsqueciMinhaSenhaAsync(string email, string scheme, string host)
    {
        var apiResponse = new ApiResponse<bool>();
        try
        {
            var result = await _identityService.ForgotPasswordAsync(email, scheme, host);
            if (result)
            {
                apiResponse.Success = true;
                apiResponse.Data = true;
                Notificar("Sucesso", "Instruções para redefinição de senha enviadas para o seu e-mail.");
            }
            else
            {
                if (!ObterNotificacoesParaResposta().Any())
                {
                    Notificar("Erro", "Falha ao solicitar redefinição de senha. Verifique o e-mail ou tente novamente.");
                }
            }
        }
        catch (Exception ex)
        {
            Notificar("Erro", $"Ocorreu um erro inesperado: {ex.Message}");
            _logger.LogError(ex, $"EXCEÇÃO NO ESQUECI MINHA SENHA: {ex.Message}");
        }
        apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
        return apiResponse;
    }
    */


    public async Task<ApiResponse<bool>> RedefinirSenhaAsync(string userId, string token, string newPassword)
        {
            var apiResponse = new ApiResponse<bool>();
            try
            {
                var user = await _identityService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    Notificar("Erro", "Usuário não encontrado para redefinição de senha.");
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }

                var result = await _identityService.ResetPasswordAsync(user, token, newPassword);
                if (result)
                {
                    apiResponse.Success = true;
                    apiResponse.Data = true;
                    Notificar("Sucesso", "Senha redefinida com sucesso!");
                }
                else
                {
                    if (!ObterNotificacoesParaResposta().Any())
                    {
                        Notificar("Erro", "Falha ao redefinir senha. Verifique se o token é válido ou se a senha atende aos requisitos.");
                    }
                }
            }
            catch (Exception ex)
            {
                Notificar("Erro", $"Ocorreu um erro inesperado: {ex.Message}");
                _logger.LogError(ex, $"EXCEÇÃO NA REDEFINIÇÃO DE SENHA: {ex.Message}");
            }
            apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
            return apiResponse;
        }

        public async Task<ApiResponse<bool>> ConfirmEmail(string userId, string stringcode)
        {
            var apiResponse = new ApiResponse<bool>();
            try
            {
                // Corrigido: Usar GetUserByIdAsync em vez de GetUserByIdByIdAsync
                var user = await _identityService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    Notificar("Erro", "Usuário não encontrado para confirmação de e-mail.");
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }

                // Decodifica a URL e remove qualquer espaço indesejado
                string decodedCode = System.Net.WebUtility.UrlDecode(stringcode).Replace(" ", "+");

                var identityResult = await _userManager.ConfirmEmailAsync(user, decodedCode);

                if (identityResult.Succeeded)
                {
                    if (user.EmailConfirmed == false)
                    {
                        user.EmailConfirmed = true;
                        var updateResult = await _userManager.UpdateAsync(user);
                        if (!updateResult.Succeeded)
                        {
                            foreach (var error in updateResult.Errors)
                            {
                                Notificar("Erro", $"Erro ao persistir confirmação de e-mail: {error.Description} (Código: {error.Code})");
                            }
                        }
                    }
                    apiResponse.Success = true;
                    apiResponse.Data = true;
                    Notificar("Sucesso", "Seu e-mail foi confirmado com sucesso!");
                }
                else
                {
                    foreach (var error in identityResult.Errors)
                    {
                        Notificar("Erro", $"Falha na confirmação de e-mail: {error.Description} (Código: {error.Code})");
                    }
                }
            }
            catch (Exception ex)
            {
                Notificar("Erro", $"Ocorreu um erro inesperado: {ex.Message}");
                _logger.LogError(ex, $"EXCEÇÃO NA CONFIRMAÇÃO DE E-MAIL: {ex.Message}");
            }
            apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
            return apiResponse;
        }

        public async Task<ApiResponse<bool>> ResendEmailConfirmationAsync(string email, string scheme, string host)
        {
            var apiResponse = new ApiResponse<bool>();
            try
            {
                var user = await _identityService.GetUserByEmailAsync(email);
                if (user == null)
                {
                    Notificar("Alerta", "Usuário não encontrado.");
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }

                if (user.EmailConfirmed)
                {
                    Notificar("Alerta", "Seu e-mail já está confirmado.");
                    apiResponse.Success = true;
                    apiResponse.Data = true;
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }

                var emailSent = await _identityService.SendConfirmationEmailAsync(user, scheme, host);

                if (emailSent)
                {
                    apiResponse.Success = true;
                    apiResponse.Data = true;
                    Notificar("Sucesso", "Um novo e-mail de confirmação foi enviado para você.");
                }
                else
                {
                    if (!ObterNotificacoesParaResposta().Any())
                    {
                        Notificar("Erro", "Não foi possível reenviar o e-mail de confirmação. Tente novamente.");
                    }
                }
            }
            catch (Exception ex)
            {
                Notificar("Erro", $"Ocorreu um erro inesperado: {ex.Message}");
                _logger.LogError(ex, $"EXCEÇÃO NO REENVIO DE CONFIRMAÇÃO DE E-MAIL: {ex.Message}");
            }
            apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
            return apiResponse;
        }

        public async Task<ApiResponse<bool>> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            var apiResponse = new ApiResponse<bool>();
            try
            {
                var user = await _identityService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    Notificar("Erro", "Usuário não encontrado para alteração de senha.");
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }

                var result = await _identityService.ChangePasswordAsync(user, currentPassword, newPassword);

                if (result)
                {
                    apiResponse.Success = true;
                    apiResponse.Data = true;
                    Notificar("Sucesso", "Senha alterada com sucesso!");
                }
                else
                {
                    if (!ObterNotificacoesParaResposta().Any())
                    {
                        Notificar("Erro", "Falha ao alterar senha. Verifique a senha atual e os requisitos da nova senha.");
                    }
                }
            }
            catch (Exception ex)
            {
                Notificar("Erro", $"Ocorreu um erro inesperado: {ex.Message}");
                _logger.LogError(ex, $"EXCEÇÃO NA ALTERAÇÃO DE SENHA: {ex.Message}");
            }
            apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
            return apiResponse;
        }

        public async Task<ApiResponse<AuthResult>> ChangeEmail(string userId, string newEmail)
        {
            var apiResponse = new ApiResponse<AuthResult>();
            try
            {
                var result = await _identityService.GenerateChangeEmailTokenAsync(userId, newEmail);
                if (result.Success)
                {
                    apiResponse.Success = true;
                    apiResponse.Data = result;
                    Notificar("Sucesso", "E-mail de confirmação de alteração enviado para o novo endereço.");
                }
                else
                {
                    // Mapear Notificacao para NotificationDto
                    apiResponse.Notifications = result.Notifications?
                        .Select(n => new NotificationDto { Codigo = n.Codigo, Tipo = n.Tipo, Mensagem = n.Mensagem, NomeCampo = n.NomeCampo })
                        .ToList() ?? new List<NotificationDto>();

                    if (!apiResponse.Notifications.Any()) // Verifica se ainda não há notificações
                    {
                        Notificar("Erro", "Falha ao iniciar alteração de e-mail.");
                        apiResponse.Notifications.AddRange(ObterNotificacoesParaResposta()); // Adiciona notificações do serviço base
                    }
                    apiResponse.Success = false;
                    apiResponse.Data = null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"EXCEÇÃO NA ALTERAÇÃO DE E-MAIL: {ex.Message}");
                Notificar("Erro", $"Ocorreu um erro inesperado: {ex.Message}");
                apiResponse.Success = false;
                apiResponse.Data = null;
            }
            // Concatenar notificações apenas se apiResponse.Notifications não for null
            if (apiResponse.Notifications == null) apiResponse.Notifications = new List<NotificationDto>();
            apiResponse.Notifications.AddRange(ObterNotificacoesParaResposta());
            return apiResponse;
        }

        public async Task<ApiResponse<bool>> ConfirmChangeEmail(string userId, string newEmail, string code)
        {
            var apiResponse = new ApiResponse<bool>();
            try
            {
                var result = await _identityService.ChangeEmailAsync(userId, newEmail, code);
                if (result.Success)
                {
                    apiResponse.Success = true;
                    apiResponse.Data = true;
                    Notificar("Sucesso", "E-mail alterado com sucesso!");
                }
                else
                {
                    // Mapear Notificacao para NotificationDto
                    apiResponse.Notifications = result.Notifications?
                        .Select(n => new NotificationDto { Codigo = n.Codigo, Tipo = n.Tipo, Mensagem = n.Mensagem, NomeCampo = n.NomeCampo })
                        .ToList() ?? new List<NotificationDto>();

                    if (!apiResponse.Notifications.Any()) // Verifica se ainda não há notificações
                    {
                        Notificar("Erro", "Falha ao confirmar alteração de e-mail.");
                        apiResponse.Notifications.AddRange(ObterNotificacoesParaResposta()); // Adiciona notificações do serviço base
                    }
                    apiResponse.Success = false;
                    apiResponse.Data = false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"EXCEÇÃO NA CONFIRMAÇÃO DE ALTERAÇÃO DE E-MAIL: {ex.Message}");
                Notificar("Erro", $"Ocorreu um erro inesperado: {ex.Message}");
                apiResponse.Success = false;
                apiResponse.Data = false;
            }
            // Concatenar notificações apenas se apiResponse.Notifications não for null
            if (apiResponse.Notifications == null) apiResponse.Notifications = new List<NotificationDto>();
            apiResponse.Notifications.AddRange(ObterNotificacoesParaResposta());
            return apiResponse;
        }

        public async Task RealizarLogout()
        {
            try
            {
                await _identityService.SignOutUserAsync();
                Notificar("Sucesso", "Logout realizado com sucesso.");
                _logger.LogInformation("Usuário deslogado com sucesso via Logout.");
            }
            catch (Exception ex)
            {
                Notificar("Erro", $"Ocorreu um erro inesperado durante o logout: {ex.Message}");
                _logger.LogError(ex, $"EXCEÇÃO NO LOGOUT: {ex.Message}");
            }
        }

        public async Task<Usuario> GetLoggedInUser()
        {
            var user = await _identityService.GetLoggedInUserAsync();
            if (user == null)
            {
                Notificar("Alerta", "Usuário não logado ou sessão expirada.");
                _logger.LogWarning("Tentativa de obter usuário logado falhou: Usuário não logado ou sessão expirada.");
            }
            return user;
        }

        public async Task<string> GetLoggedInUserId()
        {
            return await _identityService.GetLoggedInUserIdAsync();
        }

        //public async Task<bool> RegistrarNovoUsuario(string email, string password, string apelido, string cpf, string celular,
        //                                     string scheme, string host, string nomeCompleto, bool termsAccepted)
        public async Task<bool> RegistrarNovoUsuario(RegisterRequestDto request)
        {
            // A validação via FluentValidation já foi executada antes deste método ser chamado.
            // As verificações de obrigatoriedade de apelido, email, etc., já estão garantidas.

            // Obtenha os dados limpos do DTO
            var cleanedCpf = request.Cpf.CleanNumbers();
            var cleanedCelular = request.Celular.CleanNumbers();

            // 1. Tenta registrar o usuário no Identity (isso DEVE persistir no AspNetUsers)
            // A chamada agora utiliza as propriedades do DTO e inclui a nova propriedade.
            var registrationResult = await _identityService.RegisterUserAsync(
                request.Email,                
                request.Password,                
                request.Apelido,
                cleanedCpf,
                cleanedCelular,                
                request.FotoPerfil,
                request.NomeCompleto,
                request.TermsAccepted
            );

            if (!registrationResult.Success)
            {
                if (registrationResult.Notifications != null && registrationResult.Notifications.Any())
                {
                    foreach (var notif in registrationResult.Notifications)
                    {
                        Notificar(notif.Tipo, notif.Mensagem, notif.NomeCampo);
                    }
                }
                else
                {
                    Notificar("Erro", "Falha ao registrar usuário no Identity. Verifique os dados informados.");
                }
                _logger.LogError($"Falha ao registrar usuário '{request.Email}' no Identity.");
                return false;
            }

            // Se o registro no IdentityService foi bem-sucedido, o usuário já está no AspNetUsers.
            // Agora, tenta criar o Apostador e persistir.
            Usuario newUser = null;
            try
            {
                newUser = await _identityService.GetUserByEmailAsync(request.Email);
                if (newUser == null)
                {
                    Notificar("Erro", "Usuário recém-criado não encontrado no Identity após registro. Contate o suporte.");
                    _logger.LogError($"Usuário '{request.Email}' criado mas não encontrado no Identity após registro.");
                    return false;
                }

                var apostador = new Apostador
                {
                    Id = Guid.NewGuid(),
                    UsuarioId = newUser.Id, // Associa ao ID do usuário Identity
                    NomeCompleto = request.NomeCompleto,
                    Status = StatusApostador.AguardandoAssociacao
                };

                apostador.Saldo = new Saldo(apostador.Id, 0.00M);

                _apostadorRepository.Adicionar(apostador);

                // 2. Tenta persistir o Apostador e o Saldo (via UnitOfWork)
                if (await CommitAsync())
                {
                    var emailSent = await _identityService.SendConfirmationEmailAsync(newUser, request.Scheme, request.Host);
                    



                    if (emailSent)
                    {
                        Notificar("Sucesso", "Registro realizado com sucesso! Um e-mail de confirmação foi enviado.");
                        _logger.LogInformation($"Usuário '{request.Email}' registrado e e-mail de confirmação enviado.");
                        return true;
                    }
                    else
                    {
                        Notificar("Erro", "Usuário registrado, mas falha ao enviar e-mail de confirmação. Contate o suporte.");
                        _logger.LogError($"Usuário '{request.Email}' registrado, mas falha ao enviar e-mail de confirmação.");
                        return false;
                    }
                }
                else
                {
                    Notificar("Erro", "Não foi possível persistir o registro do apostador. Tente novamente.");
                    _logger.LogError($"Falha ao persistir registro do apostador para '{request.Email}'.");

                    if (newUser != null)
                    {
                        var deleteResult = await _userManager.DeleteAsync(newUser);
                        if (!deleteResult.Succeeded)
                        {
                            _logger.LogError($"Falha ao reverter criação do usuário '{request.Email}' após falha na criação do apostador. Erros: {string.Join(", ", deleteResult.Errors.Select(e => e.Description))}");
                            Notificar("Erro", "Falha na reversão do usuário Identity após exceção. Contate o suporte.");
                        }
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {

                // AQUI ESTÁ O LOG ESCANDALOSO
                // Isso irá registrar a exceção completa, incluindo a mensagem, o StackTrace e as InnerExceptions
                _logger.LogError(ex, "ERRO CRÍTICO NO REGISTRO DE USUÁRIO: {ErrorMessage}", ex.Message);

                // Agora, retorne o erro de forma apropriada
                Notificar("Erro", "Ocorreu um erro inesperado no registro. Verifique os logs do servidor.");
              

                Notificar("Erro", $"Ocorreu um erro inesperado durante o registro do apostador: {ex.Message}");
                _logger.LogError(ex, $"EXCEÇÃO NO REGISTRO DO APOSTADOR (RegistrarNovoUsuario): {ex.Message}");

                if (newUser != null)
                {
                    var deleteResult = await _userManager.DeleteAsync(newUser);
                    if (!deleteResult.Succeeded)
                    {
                        _logger.LogError($"Falha ao reverter criação do usuário '{request.Email}' após exceção na criação do apostador. Erros: {string.Join(", ", deleteResult.Errors.Select(e => e.Description))}");
                        Notificar("Erro", "Falha na reversão do usuário Identity após exceção. Contate o suporte.");
                    }
                }
                return false;
            }
        }


       
        public async Task<UsuarioLoginResult> Login(string email, string password, bool rememberMe)
        {
            var domainLoginResult = await _identityService.LoginAsync(email, password, rememberMe);

            var usuarioLoginResultDto = new UsuarioLoginResult
            {
                Success = domainLoginResult.Success,
                // Mapear Notificacao para NotificationDto
                Notifications = domainLoginResult.Notifications?
                    .Select(n => new NotificationDto { Codigo = n.Codigo, Tipo = n.Tipo, Mensagem = n.Mensagem, NomeCampo = n.NomeCampo })
                    .ToList() ?? new List<NotificationDto>()
            };

            if (!usuarioLoginResultDto.Success)
            {
                if (domainLoginResult.Notifications != null && domainLoginResult.Notifications.Any())
                {
                    foreach (var notif in domainLoginResult.Notifications)
                    {
                        Notificar(notif.Tipo, notif.Mensagem, notif.NomeCampo);
                    }
                }
                else if (!ObterNotificacoesParaResposta().Any())
                {
                    Notificar("Erro", "Credenciais inválidas ou erro desconhecido.");
                }
                _logger.LogWarning($"Login falhou para '{email}'.");
            }
            else
            {
                var user = await _identityService.GetUserByEmailAsync(email);
                if (user != null)
                {
                    user.LastLoginDate = DateTime.Now;
                    var updateResult = await _userManager.UpdateAsync(user);
                    if (!updateResult.Succeeded)
                    {
                        _logger.LogError($"Falha ao atualizar LastLoginDate para '{email}'. Erros: {string.Join(", ", updateResult.Errors.Select(e => e.Description))}");
                    }
                }
                Notificar("Sucesso", "Login realizado com sucesso!");
                _logger.LogInformation($"Login bem-sucedido para '{email}'.");
            }
            return usuarioLoginResultDto;
        }

        public async Task<bool> RealizarLogin(string email, string password, bool isPersistent, bool lockoutOnFailure)
        {
            var loginResult = await _identityService.SignInUserAsync(email, password, isPersistent, lockoutOnFailure);
            if (!loginResult)
            {
                if (!ObterNotificacoesParaResposta().Any())
                {
                    Notificar("Erro", "Credenciais inválidas ou conta bloqueada.");
                }
                _logger.LogWarning($"Login falhou para '{email}': Credenciais inválidas ou conta bloqueada.");
            }
            else
            {
                var user = await _identityService.GetUserByEmailAsync(email);
                if (user != null)
                {
                    user.LastLoginDate = DateTime.Now;
                    var updateResult = await _userManager.UpdateAsync(user);
                    if (!updateResult.Succeeded)
                    {
                        _logger.LogError($"Falha ao atualizar LastLoginDate para '{email}'. Erros: {string.Join(", ", updateResult.Errors.Select(e => e.Description))}");
                    }
                }
                Notificar("Sucesso", "Login realizado com sucesso!");
                _logger.LogInformation($"Login bem-sucedido para '{email}'.");
            }
            return loginResult;
        }

        public async Task<ApostadorDto> GetUsuarioProfileAsync(string userId)
        {
            var applicationUser = await _userManager.FindByIdAsync(userId);
            if (applicationUser == null)
            {
                Notificar("Alerta", "Usuário do sistema não encontrado.");
                return null;
            }

            var apostadorEntity = await _apostadorService.GetApostadorByUserIdAsync(userId);

            if (apostadorEntity == null)
            {
                return new ApostadorDto
                {
                    UsuarioId = userId,
                    Apelido = applicationUser.Apelido,
                    Email = applicationUser.Email,
                    Status = StatusApostador.AguardandoAssociacao.ToString()
                };
            }

            return new ApostadorDto
            {
                Id = apostadorEntity.Id.ToString(),
                UsuarioId = apostadorEntity.UsuarioId,
                Apelido = applicationUser.Apelido,
                Email = applicationUser.Email,
                Status = apostadorEntity.Status.ToString(),
                Saldo = new SaldoDto
                {
                    Valor = apostadorEntity.Saldo != null ? apostadorEntity.Saldo.Valor : 0.00M,
                    DataUltimaAtualizacao = apostadorEntity.Saldo != null ? apostadorEntity.Saldo.DataUltimaAtualizacao : DateTime.MinValue
                }
            };
        }

        public async Task<bool> ApelidoExiste(string apelido)
        {
            return await _identityService.ApelidoExisteAsync(apelido);
        }

        public async Task<Usuario> ObterUsuarioPorId(Guid userId)
        {
            var user = await _identityService.GetUserByIdAsync(userId.ToString());
            if (user == null)
            {
                Notificar("Alerta", "Usuário não encontrado.");
                _logger.LogWarning($"Tentativa de obter usuário por ID '{userId}' falhou: Usuário não encontrado.");
            }
            return user;
        }

        public async Task<Usuario> ObterUsuarioPorUsuarioId(string userId)
        {
            var user = await _identityService.GetUserByIdAsync(userId);
            if (user == null)
            {
                Notificar("Alerta", "Usuário não encontrado.");
                _logger.LogWarning($"Tentativa de obter usuário por ID '{userId}' falhou: Usuário não encontrado.");
            }
            return user;
        }

        // Método ObterUsuarioPorEmail (reintegrado e corrigido para usar user.Email)
        public async Task<Usuario> ObterUsuarioPorEmail(string email)
        {
            var user = await _identityService.GetUserByEmailAsync(email);
            if (user == null)
            {
                Notificar("Alerta", "Usuário não encontrado.");
                _logger.LogWarning($"Tentativa de obter usuário por e-mail '{email}' falhou: Usuário não encontrado.");
            }
            return user;
        }


        public async Task<object> GenerateTestHashAsync()
        {
           // Apenas repassa a chamada e o resultado do IdentityService
           // Sem lógica de negócio, apenas roteamento de dados
            return await _identityService.GenerateTestHashAsync(
                   "josemarcellogardeldealemar@gmail.com",
                   "NovaSenha@2025"
            );
        }

    }
}
