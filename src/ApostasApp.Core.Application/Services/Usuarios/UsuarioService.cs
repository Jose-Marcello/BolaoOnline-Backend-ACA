// UsuarioService.cs
// Localização: ApostasApp.Core.Application.Services.Usuarios

// Usings necessários
using ApostasApp.Core.Application.DTOs.Apostadores;
using ApostasApp.Core.Application.DTOs.Financeiro;
using ApostasApp.Core.Application.DTOs.Usuarios;
using ApostasApp.Core.Application.Models; // Para ApiResponse
using ApostasApp.Core.Application.Services.Base;
using ApostasApp.Core.Application.Services.Interfaces.Apostadores;
using ApostasApp.Core.Domain.Interfaces;
using ApostasApp.Core.Domain.Interfaces.Apostadores;
using ApostasApp.Core.Domain.Interfaces.Identity;
using ApostasApp.Core.Domain.Interfaces.Notificacoes;
using ApostasApp.Core.Domain.Interfaces.Usuarios;
using ApostasApp.Core.Domain.Models.Apostadores;
//using ApostasApp.Core.Domain.Models.Enums; // VERIFIQUE ESTE NAMESPACE PARA StatusApostador
using ApostasApp.Core.Domain.Models.Financeiro;
using ApostasApp.Core.Domain.Models.Notificacoes;
using ApostasApp.Core.Domain.Models.Usuarios;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
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

        public async Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto request)
        {
            var apiResponse = new ApiResponse<LoginResponseDto>(false, null);
            var responseData = new LoginResponseDto { LoginSucesso = false };

            try
            {
                var loginResult = await _identityService.LoginAsync(request.Email, request.Password, request.IsPersistent);

                responseData.LoginSucesso = loginResult.Success;
                responseData.Token = loginResult.Token;
                responseData.RefreshToken = loginResult.RefreshToken;
                responseData.Expiration = loginResult.Expiration;

                var user = await _identityService.GetUserByEmailAsync(request.Email);
                if (user != null)
                {
                    responseData.Apelido = user.Apelido;
                    responseData.Email = user.Email;
                    responseData.UserId = user.Id;
                    user.LastLoginDate = DateTime.Now;
                    await _userManager.UpdateAsync(user);
                }
                else
                {
                    Notificar("USUARIO_NAO_ENCONTRADO_LOGIN", "Erro", "Detalhes do usuário não encontrados após login.");
                    responseData.LoginSucesso = false;
                }

                responseData.Erros = _mapper.Map<List<NotificationDto>>(loginResult.Notifications);

                if (responseData.LoginSucesso)
                {
                    apiResponse.Success = true;
                    Notificar("LOGIN_SUCESSO", "Sucesso", "Login realizado com sucesso.");
                }
                else
                {
                    if (responseData.Erros == null || !responseData.Erros.Any())
                    {
                        Notificar("CREDENCIAIS_INVALIDAS", "Erro", "Credenciais inválidas ou erro desconhecido.");
                    }
                }
            }
            catch (Exception ex)
            {
                Notificar("ERRO_INESPERADO_LOGIN", "Erro", $"Ocorreu um erro inesperado durante o login: {ex.Message}");
                _logger.LogError(ex, $"EXCEÇÃO NO LOGIN (LoginAsync Service): {ex.Message}");
            }

            // <<-- CORRIGIDO: Usando ObterNotificacoesParaResposta -->>
            apiResponse.Notifications = ObterNotificacoesParaResposta().Concat(responseData.Erros ?? new List<NotificationDto>()).ToList();

            if (!apiResponse.Success && (apiResponse.Notifications == null || !apiResponse.Notifications.Any()))
            {
                apiResponse.Notifications.Add(new NotificationDto { Codigo = "GENERIC_ERROR", Tipo = "Erro", Mensagem = "Ocorreu um erro inesperado." });
            }

            apiResponse.Data = responseData;
            return apiResponse;
        }

        public async Task<ApiResponse<RegisterResponse>> RegisterAsync(RegisterRequestDto request)
        {
            var apiResponse = new ApiResponse<RegisterResponse>(false, null);
            try
            {
                var registrationSuccess = await RegistrarNovoUsuario(
                    request.Email, request.Password, request.Apelido, request.Cpf, request.Celular, request.Scheme, request.Host);

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
                        Notificar("REGISTRO_SUCESSO", "Sucesso", "Registro realizado com sucesso!");
                    }
                    else
                    {
                        Notificar("USUARIO_NAO_ENCONTRADO_REGISTRO", "Erro", "Usuário registrado, mas detalhes não encontrados para retorno.");
                    }
                }
                else
                {
                    Notificar("FALHA_REGISTRO", "Erro", "Falha ao registrar usuário. Verifique os dados informados.");
                }
            }
            catch (Exception ex)
            {
                Notificar("ERRO_INESPERADO_REGISTRO", "Erro", $"Ocorreu um erro inesperado durante o registro: {ex.Message}");
                _logger.LogError(ex, $"EXCEÇÃO NO REGISTRO (RegisterAsync Service): {ex.Message}");
            }

            // <<-- CORRIGIDO: Usando ObterNotificacoesParaResposta -->>
            apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
            if (!apiResponse.Success && (apiResponse.Notifications == null || !apiResponse.Notifications.Any()))
            {
                apiResponse.Notifications.Add(new NotificationDto { Codigo = "GENERIC_ERROR", Tipo = "Erro", Mensagem = "Ocorreu um erro inesperado." });
            }

            return apiResponse;
        }

        public async Task<ApiResponse<bool>> EsqueciMinhaSenhaAsync(string email, string scheme, string host)
        {
            var apiResponse = new ApiResponse<bool>(false, false);
            try
            {
                var result = await _identityService.ForgotPasswordAsync(email, scheme, host);
                if (result)
                {
                    apiResponse.Success = true;
                    apiResponse.Data = true;
                    Notificar("ESQUECI_SENHA_SUCESSO", "Sucesso", "Instruções para redefinição de senha enviadas para o seu e-mail.");
                }
                else
                {
                    if (!TemNotificacao())
                    {
                        Notificar("ESQUECI_SENHA_FALHA", "Erro", "Falha ao solicitar redefinição de senha. Verifique o e-mail ou tente novamente.");
                    }
                }
            }
            catch (Exception ex)
            {
                Notificar("ERRO_INESPERADO_ESQUECI_SENHA", "Erro", $"Ocorreu um erro inesperado: {ex.Message}");
                _logger.LogError(ex, $"EXCEÇÃO NO ESQUECI MINHA SENHA: {ex.Message}");
            }
            // <<-- CORRIGIDO: Usando ObterNotificacoesParaResposta -->>
            apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
            return apiResponse;
        }

        public async Task<ApiResponse<bool>> RedefinirSenhaAsync(string userId, string token, string newPassword)
        {
            var apiResponse = new ApiResponse<bool>(false, false);
            try
            {
                var user = await _identityService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    Notificar("USUARIO_NAO_ENCONTRADO_REDEFINIR", "Erro", "Usuário não encontrado para redefinição de senha.");
                    // <<-- CORRIGIDO: Usando ObterNotificacoesParaResposta -->>
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }

                var result = await _identityService.ResetPasswordAsync(user, token, newPassword);
                if (result)
                {
                    apiResponse.Success = true;
                    apiResponse.Data = true;
                    Notificar("REDEFINIR_SENHA_SUCESSO", "Sucesso", "Senha redefinida com sucesso!");
                }
                else
                {
                    if (!TemNotificacao())
                    {
                        Notificar("REDEFINIR_SENHA_FALHA", "Erro", "Falha ao redefinir senha. Verifique se o token é válido ou se a senha atende aos requisitos.");
                    }
                }
            }
            catch (Exception ex)
            {
                Notificar("ERRO_INESPERADO_REDEFINIR_SENHA", "Erro", $"Ocorreu um erro inesperado: {ex.Message}");
                _logger.LogError(ex, $"EXCEÇÃO NA REDEFINIÇÃO DE SENHA: {ex.Message}");
            }
            // <<-- CORRIGIDO: Usando ObterNotificacoesParaResposta -->>
            apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
            return apiResponse;
        }

        public async Task<ApiResponse<bool>> ConfirmEmail(string userId, string code)
        {
            var apiResponse = new ApiResponse<bool>(false, false);
            try
            {
                var user = await _identityService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    Notificar("USUARIO_NAO_ENCONTRADO_CONFIRMAR_EMAIL", "Erro", "Usuário não encontrado para confirmação de e-mail.");
                    // <<-- CORRIGIDO: Usando ObterNotificacoesParaResposta -->>
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }

                var identityResult = await _userManager.ConfirmEmailAsync(user, code);

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
                                Notificar("ERRO_PERSISTIR_CONFIRMACAO_EMAIL", "Erro", $"Erro ao persistir confirmação de e-mail: {error.Description}", error.Code);
                            }
                        }
                    }
                    apiResponse.Success = true;
                    apiResponse.Data = true;
                    Notificar("CONFIRMACAO_EMAIL_SUCESSO", "Sucesso", "Seu e-mail foi confirmado com sucesso!");
                }
                else
                {
                    foreach (var error in identityResult.Errors)
                    {
                        Notificar("FALHA_CONFIRMACAO_EMAIL", "Erro", $"Falha na confirmação de e-mail: {error.Description}", error.Code);
                    }
                }
            }
            catch (Exception ex)
            {
                Notificar("ERRO_INESPERADO_CONFIRMACAO_EMAIL", "Erro", $"Ocorreu um erro inesperado: {ex.Message}");
                _logger.LogError(ex, $"EXCEÇÃO NA CONFIRMAÇÃO DE E-MAIL: {ex.Message}");
            }
            // <<-- CORRIGIDO: Usando ObterNotificacoesParaResposta -->>
            apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
            return apiResponse;
        }

        public async Task<ApiResponse<bool>> ResendEmailConfirmationAsync(string email, string scheme, string host)
        {
            var apiResponse = new ApiResponse<bool>(false, false);
            try
            {
                var user = await _identityService.GetUserByEmailAsync(email);
                if (user == null)
                {
                    Notificar("USUARIO_NAO_ENCONTRADO_REENVIO", "Alerta", "Usuário não encontrado.");
                    // <<-- CORRIGIDO: Usando ObterNotificacoesParaResposta -->>
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }

                if (user.EmailConfirmed)
                {
                    Notificar("EMAIL_JA_CONFIRMADO", "Alerta", "Seu e-mail já está confirmado.");
                    apiResponse.Success = true;
                    apiResponse.Data = true;
                    // <<-- CORRIGIDO: Usando ObterNotificacoesParaResposta -->>
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }

                var emailSent = await _identityService.SendConfirmationEmailAsync(user, scheme, host);

                if (emailSent)
                {
                    apiResponse.Success = true;
                    apiResponse.Data = true;
                    Notificar("REENVIO_EMAIL_SUCESSO", "Sucesso", "Um novo e-mail de confirmação foi enviado para você.");
                }
                else
                {
                    if (!TemNotificacao())
                    {
                        Notificar("REENVIO_EMAIL_FALHA", "Erro", "Não foi possível reenviar o e-mail de confirmação. Tente novamente.");
                    }
                }
            }
            catch (Exception ex)
            {
                Notificar("ERRO_INESPERADO_REENVIO_EMAIL", "Erro", $"Ocorreu um erro inesperado: {ex.Message}");
                _logger.LogError(ex, $"EXCEÇÃO NO REENVIO DE CONFIRMAÇÃO DE E-MAIL: {ex.Message}");
            }
            // <<-- CORRIGIDO: Usando ObterNotificacoesParaResposta -->>
            apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
            return apiResponse;
        }

        public async Task<ApiResponse<bool>> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            var apiResponse = new ApiResponse<bool>(false, false);
            try
            {
                var user = await _identityService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    Notificar("USUARIO_NAO_ENCONTRADO_ALTERAR_SENHA", "Erro", "Usuário não encontrado para alteração de senha.");
                    // <<-- CORRIGIDO: Usando ObterNotificacoesParaResposta -->>
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }

                var result = await _identityService.ChangePasswordAsync(user, currentPassword, newPassword);

                if (result)
                {
                    apiResponse.Success = true;
                    apiResponse.Data = true;
                    Notificar("ALTERACAO_SENHA_SUCESSO", "Sucesso", "Senha alterada com sucesso!");
                }
                else
                {
                    if (!TemNotificacao())
                    {
                        Notificar("ALTERACAO_SENHA_FALHA", "Erro", "Falha ao alterar senha. Verifique a senha atual e os requisitos da nova senha.");
                    }
                }
            }
            catch (Exception ex)
            {
                Notificar("ERRO_INESPERADO_ALTERAR_SENHA", "Erro", $"Ocorreu um erro inesperado: {ex.Message}");
                _logger.LogError(ex, $"EXCEÇÃO NA ALTERAÇÃO DE SENHA: {ex.Message}");
            }
            // <<-- CORRIGIDO: Usando ObterNotificacoesParaResposta -->>
            apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
            return apiResponse;
        }

        public async Task<ApiResponse<AuthResult>> ChangeEmail(string userId, string newEmail)
        {
            var apiResponse = new ApiResponse<AuthResult>(false, null);
            try
            {
                var result = await _identityService.GenerateChangeEmailTokenAsync(userId, newEmail);
                if (result.Success)
                {
                    apiResponse.Success = true;
                    apiResponse.Data = result;
                    Notificar("ALTERACAO_EMAIL_SOLICITACAO_SUCESSO", "Sucesso", "E-mail de confirmação de alteração enviado para o novo endereço.");
                }
                else
                {
                    if (!TemNotificacao())
                    {
                        Notificar("ALTERACAO_EMAIL_SOLICITACAO_FALHA", "Erro", "Falha ao iniciar alteração de e-mail.");
                    }
                }
            }
            catch (Exception ex)
            {
                Notificar("ERRO_INESPERADO_ALTERACAO_EMAIL_SOLICITACAO", "Erro", $"Ocorreu um erro inesperado: {ex.Message}");
                _logger.LogError(ex, $"EXCEÇÃO NA ALTERAÇÃO DE E-MAIL: {ex.Message}");
            }
            // <<-- CORRIGIDO: Usando ObterNotificacoesParaResposta -->>
            apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
            return apiResponse;
        }

        public async Task<ApiResponse<bool>> ConfirmChangeEmail(string userId, string newEmail, string code)
        {
            var apiResponse = new ApiResponse<bool>(false, false);
            try
            {
                var result = await _identityService.ChangeEmailAsync(userId, newEmail, code);
                if (result.Success)
                {
                    apiResponse.Success = true;
                    apiResponse.Data = true;
                    Notificar("CONFIRMACAO_ALTERACAO_EMAIL_SUCESSO", "Sucesso", "E-mail alterado com sucesso!");
                }
                else
                {
                    if (!TemNotificacao())
                    {
                        Notificar("CONFIRMACAO_ALTERACAO_EMAIL_FALHA", "Erro", "Falha ao confirmar alteração de e-mail.");
                    }
                }
            }
            catch (Exception ex)
            {
                Notificar("ERRO_INESPERADO_CONFIRMACAO_ALTERACAO_EMAIL", "Erro", $"Ocorreu um erro inesperado: {ex.Message}");
                _logger.LogError(ex, $"EXCEÇÃO NA CONFIRMAÇÃO DE ALTERAÇÃO DE E-MAIL: {ex.Message}");
            }
            // <<-- CORRIGIDO: Usando ObterNotificacoesParaResposta -->>
            apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
            return apiResponse;
        }

        public async Task<Usuario> GetLoggedInUser()
        {
            var user = await _identityService.GetLoggedInUserAsync();
            if (user == null)
            {
                Notificar("USUARIO_NAO_LOGADO", "Alerta", "Usuário não logado ou sessão expirada.");
                _logger.LogWarning("Tentativa de obter usuário logado falhou: Usuário não logado ou sessão expirada.");
            }
            return user;
        }

        public async Task<string> GetLoggedInUserId()
        {
            return await _identityService.GetLoggedInUserIdAsync();
        }

        public async Task<bool> RegistrarNovoUsuario(string email, string password, string apelido, string cpf, string celular, string scheme, string host)
        {
            if (string.IsNullOrWhiteSpace(apelido))
            {
                Notificar("APELIDO_OBRIGATORIO", "Erro", "Apelido é obrigatório.", "Apelido");
                _logger.LogError("Falha no registro: Apelido é obrigatório.");
                return false;
            }
            if (string.IsNullOrWhiteSpace(celular))
            {
                Notificar("CELULAR_OBRIGATORIO", "Erro", "Celular é obrigatório.", "Celular");
                _logger.LogError("Falha no registro: Celular é obrigatório.");
                return false;
            }

            var registrationResult = await _identityService.RegisterUserAsync(email, password, apelido, cpf, celular);

            if (!registrationResult.Success)
            {
                if (registrationResult.Notifications != null && registrationResult.Notifications.Any())
                {
                    foreach (var notif in registrationResult.Notifications)
                    {
                        Notificar(notif.Codigo, notif.Tipo, notif.Mensagem, notif.NomeCampo);
                    }
                }
                else if (!TemNotificacao())
                {
                    Notificar("FALHA_REGISTRO_GENERICO", "Erro", "Falha ao registrar usuário. Verifique os dados informados.");
                }
                _logger.LogError($"Falha ao registrar usuário '{email}'.");
                return false;
            }

            var newUser = await _identityService.GetUserByEmailAsync(email);
            if (newUser == null)
            {
                Notificar("USUARIO_CRIADO_NAO_ENCONTRADO", "Erro", "Usuário recém-criado não encontrado no Identity após registro. Contate o suporte.");
                _logger.LogError($"Usuário '{email}' criado mas não encontrado no Identity após registro.");
                return false;
            }

            var apostador = new Apostador
            {
                Id = Guid.NewGuid(),
                UsuarioId = newUser.Id,
                Email = email,
                NomeCompleto = apelido,
                Status = StatusApostador.AguardandoAssociacao
            };

            apostador.Saldo = new Saldo(apostador.Id, 0.00M);

            _apostadorRepository.Adicionar(apostador);

            if (await CommitAsync())
            {
                var emailSent = await _identityService.SendConfirmationEmailAsync(newUser, scheme, host);

                if (emailSent)
                {
                    Notificar("EMAIL_CONFIRMACAO_ENVIADO", "Sucesso", "Registro realizado com sucesso! Um e-mail de confirmação foi enviado.");
                    _logger.LogInformation($"Usuário '{email}' registrado e e-mail de confirmação enviado.");
                    return true;
                }
                else
                {
                    Notificar("FALHA_ENVIO_EMAIL_CONFIRMACAO", "Erro", "Usuário registrado, mas falha ao enviar e-mail de confirmação. Contate o suporte.");
                    _logger.LogError($"Usuário '{email}' registrado, mas falha ao enviar e-mail de confirmação.");
                    return false;
                }
            }
            else
            {
                Notificar("FALHA_PERSISTENCIA_APOSTADOR", "Erro", "Não foi possível persistir o registro do apostador. Tente novamente.");
                _logger.LogError($"Falha ao persistir registro do apostador para '{email}'.");
                return false;
            }
        }

        public async Task<UsuarioLoginResult> Login(string email, string password, bool rememberMe)
        {
            var domainLoginResult = await _identityService.LoginAsync(email, password, rememberMe);

            var usuarioLoginResultDto = new UsuarioLoginResult
            {
                Success = domainLoginResult.Success,
                Notifications = _mapper.Map<List<NotificationDto>>(domainLoginResult.Notifications) ?? new List<NotificationDto>()
            };

            if (!usuarioLoginResultDto.Success)
            {
                if (domainLoginResult.Notifications != null && domainLoginResult.Notifications.Any())
                {
                    foreach (var notif in domainLoginResult.Notifications)
                    {
                        Notificar(notif.Codigo, notif.Tipo, notif.Mensagem, notif.NomeCampo);
                    }
                }
                else if (!TemNotificacao())
                {
                    Notificar("CREDENCIAIS_INVALIDAS_GENERICO", "Erro", "Credenciais inválidas ou erro desconhecido.");
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
                Notificar("LOGIN_SUCESSO_GENERICO", "Sucesso", "Login realizado com sucesso!");
                _logger.LogInformation($"Login bem-sucedido para '{email}'.");
            }
            return usuarioLoginResultDto;
        }

        public async Task Logout()
        {
            await _identityService.SignOutUserAsync();
            Notificar("LOGOUT_SUCESSO", "Sucesso", "Logout realizado com sucesso.");
            _logger.LogInformation("Usuário deslogado com sucesso via RealizarLogout.");
        }

        public async Task<bool> RealizarLogin(string email, string password, bool isPersistent, bool lockoutOnFailure)
        {
            var loginResult = await _identityService.SignInUserAsync(email, password, isPersistent, lockoutOnFailure);
            if (!loginResult)
            {
                if (!TemNotificacao())
                {
                    Notificar("LOGIN_FALHA_GENERICO", "Erro", "Credenciais inválidas ou conta bloqueada.");
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
                Notificar("REALIZAR_LOGIN_SUCESSO", "Sucesso", "Login realizado com sucesso!");
                _logger.LogInformation($"Login bem-sucedido para '{email}'.");
            }
            return loginResult;
        }

        public async Task RealizarLogout()
        {
            await _identityService.SignOutUserAsync();
            Notificar("REALIZAR_LOGOUT_SUCESSO", "Sucesso", "Logout realizado com sucesso.");
            _logger.LogInformation("Usuário deslogado com sucesso via RealizarLogout.");
        }

        public async Task<ApostadorDto> GetUsuarioProfileAsync(string userId)
        {
            var applicationUser = await _userManager.FindByIdAsync(userId);
            if (applicationUser == null)
            {
                Notificar("USUARIO_SISTEMA_NAO_ENCONTRADO", "Alerta", "Usuário do sistema não encontrado.");
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
                Notificar("USUARIO_NAO_ENCONTRADO_POR_ID", "Alerta", "Usuário não encontrado.");
                _logger.LogWarning($"Tentativa de obter usuário por ID '{userId}' falhou: Usuário não encontrado.");
            }
            return user;
        }

        public async Task<Usuario> ObterUsuarioPorUsuarioId(string userId)
        {
            var user = await _identityService.GetUserByIdAsync(userId);
            if (user == null)
            {
                Notificar("USUARIO_NAO_ENCONTRADO_POR_USUARIOID", "Alerta", "Usuário não encontrado.");
                _logger.LogWarning($"Tentativa de obter usuário por ID '{userId}' falhou: Usuário não encontrado.");
            }
            return user;
        }

        public async Task<Usuario> ObterUsuarioPorEmail(string email)
        {
            var user = await _identityService.GetUserByEmailAsync(email);
            if (user == null)
            {
                Notificar("USUARIO_NAO_ENCONTRADO_POR_EMAIL", "Alerta", "Usuário não encontrado.");
                _logger.LogWarning($"Tentativa de obter usuário por e-mail '{email}' falhou: Usuário não encontrado.");
            }
            return user;
        }

        private async Task<string> GerarJwtToken(Usuario user)
        {
            var jwtSecret = _configuration["Jwt:SecretKey"];
            var jwtIssuer = _configuration["Jwt:Issuer"];
            var jwtAudience = _configuration["Jwt:Audience"];
            if (!double.TryParse(_configuration["Jwt:ExpiresInMinutes"], out double jwtExpiresInMinutes))
            {
                jwtExpiresInMinutes = 60;
                _logger.LogWarning("Configuração 'Jwt:ExpiresInMinutes' não encontrada ou inválida. Usando padrão de 60 minutos.");
            }

            _logger.LogInformation($"[JWT_GERA] SecretKey lida: '{jwtSecret}'");
            _logger.LogInformation($"[JWT_GERA] Issuer lido: '{jwtIssuer}'");
            _logger.LogInformation($"[JWT_GERA] Audience lida: '{jwtAudience}'");
            _logger.LogInformation($"[JWT_GERA] Expira em minutos: {jwtExpiresInMinutes}");

            var claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Apelido)
            };
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles) { claims.Add(new Claim(ClaimTypes.Role, role)); }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddMinutes(jwtExpiresInMinutes);

            _logger.LogInformation($"[JWT_GERA] Token gerado, expira em: {expires.ToString("o")}");

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
