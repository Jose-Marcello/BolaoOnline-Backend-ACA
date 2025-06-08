using ApostasApp.Core.Application.Services.Base;
using ApostasApp.Core.Domain.Interfaces;
using ApostasApp.Core.Domain.Interfaces.Identity;
using ApostasApp.Core.Domain.Interfaces.Notificacoes;
using ApostasApp.Core.Domain.Interfaces.Apostadores;
using ApostasApp.Core.Domain.Interfaces.Usuarios;
using ApostasApp.Core.Domain.Models.Usuarios;
using ApostasApp.Core.Domain.Models.Apostadores;
using ApostasApp.Core.Domain.Models.Notificacoes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Net;
using System.Linq;
using Microsoft.AspNetCore.Identity; // Adicionado para UserManager
using Microsoft.Extensions.Logging; // Adicionado para ILogger

namespace ApostasApp.Core.Application.Services.Usuarios
{
    /// <summary>
    /// Serviço de aplicação para operações relacionadas a usuários.
    /// </summary>
    public class UsuarioService : BaseService, IUsuarioService
    {
        private readonly IIdentityService _identityService;
        private readonly IApostadorRepository _apostadorRepository;
        private readonly UserManager<Usuario> _userManager; // Injetado UserManager para operações diretas
        private readonly ILogger<UsuarioService> _logger; // Injetado ILogger para logs

        /// <summary>
        /// Construtor do UsuarioService.
        /// </summary>
        /// <param name="identityService">Serviço de identidade para operações de usuário.</param>
        /// <param name="apostadorRepository">Repositório de apostadores.</param>
        /// <param name="notificador">Serviço de notificação para alertas e erros.</param>
        /// <param name="uow">Unidade de Trabalho para persistência de dados.</param>
        /// <param name="userManager">Gerenciador de usuários do ASP.NET Identity.</param>
        /// <param name="logger">Logger para registro de informações e erros.</param>
        public UsuarioService(IIdentityService identityService,
                              IApostadorRepository apostadorRepository,
                              INotificador notificador,
                              IUnitOfWork uow,
                              UserManager<Usuario> userManager, // Novo parâmetro para injeção de UserManager
                              ILogger<UsuarioService> logger) // Novo parâmetro para injeção de ILogger
            : base(notificador, uow)
        {
            _identityService = identityService;
            _apostadorRepository = apostadorRepository;
            _userManager = userManager; // Atribuição do UserManager
            _logger = logger; // Atribuição do logger
        }

        /// <summary>
        /// Obtém o usuário logado atualmente.
        /// </summary>
        /// <returns>O objeto Usuario do usuário logado ou null se não houver.</returns>
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

        /// <summary>
        /// Obtém o ID do usuário logado atualmente.
        /// </summary>
        /// <returns>O ID do usuário logado.</returns>
        public async Task<string> GetLoggedInUserId()
        {
            return await _identityService.GetLoggedInUserIdAsync();
        }

        /// <summary>
        /// Registra um novo usuário no sistema.
        /// </summary>
        /// <param name="email">E-mail do usuário.</param>
        /// <param name="password">Senha do usuário.</param>
        /// <param name="apelido">Apelido do usuário.</param>
        /// <param name="cpf">CPF do usuário.</param>
        /// <param name="celular">Número de celular do usuário.</param>
        /// <param name="scheme">Esquema da URL (http/https).</param>
        /// <param name="host">Host da URL.</param>
        /// <returns>True se o registro foi bem-sucedido, false caso contrário.</returns>
        public async Task<bool> RegistrarNovoUsuario(string email, string password, string apelido, string cpf, string celular, string scheme, string host)
        {
            if (string.IsNullOrWhiteSpace(apelido))
            {
                Notificar("Erro", "Apelido é obrigatório.");
                _logger.LogError("Falha no registro: Apelido é obrigatório.");
                return false;
            }
            if (string.IsNullOrWhiteSpace(celular))
            {
                Notificar("Erro", "Celular é obrigatório.");
                _logger.LogError("Falha no registro: Celular é obrigatório.");
                return false;
            }

            var registrationSuccess = await _identityService.RegisterUserAsync(email, password, apelido, cpf, celular);
            if (!registrationSuccess)
            {
                if (!TemNotificacao())
                {
                    Notificar("Erro", "Falha ao registrar usuário. Verifique os dados informados.");
                }
                _logger.LogError($"Falha ao registrar usuário '{email}'.");
                return false;
            }

            var newUser = await _identityService.GetUserByEmailAsync(email);
            if (newUser == null)
            {
                Notificar("Erro", "Usuário recém-criado não encontrado no Identity após registro. Contate o suporte.");
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
            await _apostadorRepository.Adicionar(apostador);

            var saved = await Commit(); // Salva o apostador no banco de dados

            if (saved)
            {
                var emailSent = await _identityService.SendConfirmationEmailAsync(newUser, scheme, host);

                if (emailSent)
                {
                    Notificar("Sucesso", "Registro realizado com sucesso! Um e-mail de confirmação foi enviado.");
                    _logger.LogInformation($"Usuário '{email}' registrado e e-mail de confirmação enviado.");
                    return true;
                }
                else
                {
                    Notificar("Erro", "Usuário registrado, mas falha ao enviar e-mail de confirmação. Contate o suporte.");
                    _logger.LogError($"Usuário '{email}' registrado, mas falha ao enviar e-mail de confirmação.");
                    return false;
                }
            }
            else
            {
                Notificar("Erro", "Não foi possível persistir o registro do apostador. Tente novamente.");
                _logger.LogError($"Falha ao persistir registro do apostador para '{email}'.");
                return false;
            }
        }

        /// <summary>
        /// Realiza o login do usuário.
        /// </summary>
        /// <param name="email">E-mail do usuário.</param>
        /// <param name="password">Senha do usuário.</param>
        /// <param name="rememberMe">Indica se o login deve ser lembrado.</param>
        /// <returns>Objeto LoginResult com o resultado do login.</returns>
        public async Task<LoginResult> Login(string email, string password, bool rememberMe)
        {
            var loginResult = await _identityService.LoginAsync(email, password, rememberMe);
            if (!loginResult.Success)
            {
                foreach (var notif in loginResult.Notifications)
                {
                    Notificar(notif);
                }
                _logger.LogWarning($"Falha no login para '{email}'.");
            }
            return loginResult;
        }

        /// <summary>
        /// Realiza o logout do usuário.
        /// </summary>
        public async Task Logout()
        {
            await _identityService.LogoutAsync();
            Notificar("Sucesso", "Logout realizado com sucesso.");
            _logger.LogInformation("Usuário deslogado com sucesso.");
        }

        /// <summary>
        /// Confirma o e-mail de um usuário.
        /// </summary>
        /// <param name="userId">ID do usuário.</param>
        /// <param name="code">Token de confirmação.</param>
        /// <returns>True se o e-mail foi confirmado, false caso contrário.</returns>
        public async Task<bool> ConfirmEmail(string userId, string code)
        {
            var user = await _identityService.GetUserByIdAsync(userId);
            if (user == null)
            {
                Notificar("Alerta", "Usuário não encontrado para confirmação de e-mail.");
                _logger.LogWarning($"Tentativa de confirmação de e-mail para userId '{userId}' falhou: Usuário não encontrado.");
                return false;
            }

            // ===================================================================================================
            // INÍCIO DO AJUSTE CRÍTICO: VALIDAÇÃO DO TOKEN E GARANTIA DE PERSISTÊNCIA DO EmailConfirmed
            // ===================================================================================================

            // Primeiro, tentar o método padrão do Identity. Ele faz a validação do token.
            // O retorno 'identityResult.Succeeded' indica que o token era válido.
            var identityResult = await _userManager.ConfirmEmailAsync(user, code);

            if (!identityResult.Succeeded)
            {
                // Se o Identity falhou na validação do token, notificar os erros
                foreach (var error in identityResult.Errors)
                {
                    Notificar("Erro", $"Falha na confirmação de e-mail: {error.Description}");
                    _logger.LogError($"Falha na confirmação de e-mail para '{user.Email}' (UserID: {userId}): {error.Description}");
                }
                return false; // Retorna falso se o token for inválido ou outra falha do Identity
            }

            // O token foi validado com sucesso.
            // No entanto, seu debug mostrou que 'EmailConfirmed' não estava sendo marcado corretamente em memória
            // após a chamada interna de 'store.SetEmailConfirmedAsync'.

            // Vamos GARANTIR que EmailConfirmed esteja true no objeto em memória.
            if (user.EmailConfirmed == false)
            {
                user.EmailConfirmed = true; // Força a propriedade a ser true no objeto em memória
                _logger.LogInformation($"EmailConfirmed para usuário '{user.Email}' (UserID: {userId}) forçado para true em memória.");

                // Agora, força o UserManager a atualizar o usuário, o que irá persistir
                // todas as alterações (incluindo EmailConfirmed = true) no banco de dados.
                // Este método faz seu próprio SaveChanges internamente.
                var updateResult = await _userManager.UpdateAsync(user);

                if (!updateResult.Succeeded)
                {
                    foreach (var error in updateResult.Errors)
                    {
                        Notificar("Erro", $"Erro ao persistir confirmação de e-mail: {error.Description}");
                        _logger.LogError($"Erro ao persistir confirmação de e-mail para '{user.Email}' (UserID: {userId}): {error.Description}");
                    }
                    return false; // Retorna falso se a persistência explícita falhar
                }
                _logger.LogInformation($"Usuário '{user.Email}' (UserID: {userId}) atualizado e EmailConfirmed persistido via UpdateAsync.");
            }
            else
            {
                _logger.LogInformation($"Usuário '{user.Email}' (UserID: {userId}) já estava com EmailConfirmed = true em memória. Não foi necessário forçar.");
            }

            // ===================================================================================================
            // FIM DO AJUSTE CRÍTICO
            // ===================================================================================================

            // A chamada a Commit() foi removida daqui, pois _userManager.UpdateAsync() já faz SaveChanges internamente.
            // Chamar Commit() novamente pode causar SaveChanges duplicados ou conflitos de transação.

            _logger.LogInformation($"Usuário '{user.Email}' (UserID: {userId}) confirmado com sucesso.");
            return true;
        }

        /// <summary>
        /// Solicita a redefinição de senha para um usuário.
        /// </summary>
        /// <param name="email">E-mail do usuário.</param>
        /// <param name="scheme">Esquema da URL.</param>
        /// <param name="host">Host da URL.</param>
        /// <returns>True se a solicitação foi bem-sucedida, false caso contrário.</returns>
        public async Task<bool> EsqueciMinhaSenhaAsync(string email, string scheme, string host)
        {
            var result = await _identityService.ForgotPasswordAsync(email, scheme, host);
            if (!result)
            {
                if (!TemNotificacao())
                {
                    Notificar("Erro", "Falha ao solicitar redefinição de senha. Tente novamente.");
                }
                _logger.LogError($"Falha ao solicitar redefinição de senha para '{email}'.");
            }
            return result;
        }

        /// <summary>
        /// Redefine a senha de um usuário.
        /// </summary>
        /// <param name="userId">ID do usuário.</param>
        /// <param name="token">Token de redefinição de senha.</param>
        /// <param name="newPassword">Nova senha.</param>
        /// <returns>True se a senha foi redefinida com sucesso, false caso contrário.</returns>
        public async Task<bool> RedefinirSenhaAsync(string userId, string token, string newPassword)
        {
            var user = await _identityService.GetUserByIdAsync(userId);
            if (user == null)
            {
                Notificar("Erro", "Usuário não encontrado para redefinição de senha.");
                _logger.LogWarning($"Tentativa de redefinição de senha para userId '{userId}' falhou: Usuário não encontrado.");
                return false;
            }
            var result = await _identityService.ResetPasswordAsync(user, token, newPassword);
            if (!result)
            {
                if (!TemNotificacao())
                {
                    Notificar("Erro", "Falha ao redefinir senha. Verifique se o token é válido ou se a senha atende aos requisitos.");
                }
                _logger.LogError($"Falha ao redefinir senha para '{user.Email}' (UserID: {userId}).");
            }
            return result;
        }

        /// <summary>
        /// Reenvia o e-mail de confirmação para um usuário.
        /// </summary>
        /// <param name="email">E-mail do usuário.</param>
        /// <param name="scheme">Esquema da URL.</param>
        /// <param name="host">Host da URL.</param>
        /// <returns>True se o e-mail foi reenviado com sucesso, false caso contrário.</returns>
        public async Task<bool> ResendEmailConfirmationAsync(string email, string scheme, string host)
        {
            var user = await _identityService.GetUserByEmailAsync(email);
            if (user == null)
            {
                Notificar("Alerta", "Usuário não encontrado.");
                _logger.LogWarning($"Tentativa de reenviar confirmação de e-mail para '{email}' falhou: Usuário não encontrado.");
                return false;
            }

            if (user.EmailConfirmed)
            {
                Notificar("Alerta", "Seu e-mail já está confirmado.");
                _logger.LogInformation($"Usuário '{email}' já possui e-mail confirmado. Reenvio não necessário.");
                return true;
            }

            var emailSent = await _identityService.SendConfirmationEmailAsync(user, scheme, host);

            if (emailSent)
            {
                Notificar("Sucesso", "Um novo e-mail de confirmação foi enviado para você.");
                _logger.LogInformation($"Novo e-mail de confirmação enviado para '{email}'.");
                return true;
            }
            else
            {
                if (!TemNotificacao())
                {
                    Notificar("Erro", "Não foi possível reenviar o e-mail de confirmação. Tente novamente.");
                }
                _logger.LogError($"Falha ao reenviar e-mail de confirmação para '{email}'.");
                return false;
            }
        }

        /// <summary>
        /// Inicia o processo de alteração de e-mail do usuário.
        /// </summary>
        /// <param name="userId">ID do usuário.</param>
        /// <param name="newEmail">Novo e-mail.</param>
        /// <returns>AuthResult com o resultado da operação.</returns>
        public async Task<AuthResult> ChangeEmail(string userId, string newEmail)
        {
            var result = await _identityService.GenerateChangeEmailTokenAsync(userId, newEmail);
            if (!result.Success)
            {
                if (!TemNotificacao())
                {
                    Notificar("Erro", "Falha ao iniciar alteração de e-mail.");
                }
                _logger.LogError($"Falha ao iniciar alteração de e-mail para userId '{userId}' com novo e-mail '{newEmail}'.");
            }
            return result;
        }

        /// <summary>
        /// Confirma a alteração de e-mail de um usuário.
        /// </summary>
        /// <param name="userId">ID do usuário.</param>
        /// <param name="newEmail">Novo e-mail.</param>
        /// <param name="code">Token de confirmação.</param>
        /// <returns>True se a alteração foi confirmada, false caso contrário.</returns>
        public async Task<bool> ConfirmChangeEmail(string userId, string newEmail, string code)
        {
            var result = await _identityService.ChangeEmailAsync(userId, newEmail, code);
            if (!result.Success)
            {
                if (!TemNotificacao())
                {
                    Notificar("Erro", "Falha ao confirmar alteração de e-mail.");
                }
                _logger.LogError($"Falha ao confirmar alteração de e-mail para userId '{userId}' para '{newEmail}'.");
            }
            return result.Success;
        }

        /// <summary>
        /// Verifica se um apelido já existe.
        /// </summary>
        /// <param name="apelido">Apelido a ser verificado.</param>
        /// <returns>True se o apelido existe, false caso contrário.</returns>
        public async Task<bool> ApelidoExiste(string apelido)
        {
            return await _identityService.ApelidoExisteAsync(apelido);
        }

        /// <summary>
        /// Obtém um usuário pelo seu ID.
        /// </summary>
        /// <param name="userId">ID do usuário.</param>
        /// <returns>O objeto Usuario ou null se não encontrado.</returns>
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

        /// <summary>
        /// Obtém um usuário pelo seu e-mail.
        /// </summary>
        /// <param name="email">E-mail do usuário.</param>
        /// <returns>O objeto Usuario ou null se não encontrado.</returns>
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

        /// <summary>
        /// Realiza o login do usuário (versão mais detalhada).
        /// </summary>
        /// <param name="email">E-mail do usuário.</param>
        /// <param name="password">Senha do usuário.</param>
        /// <param name="isPersistent">Indica se o cookie de login deve ser persistente.</param>
        /// <param name="lockoutOnFailure">Indica se o bloqueio de conta deve ser ativado em caso de falha.</param>
        /// <returns>True se o login foi bem-sucedido, false caso contrário.</returns>
        public async Task<bool> RealizarLogin(string email, string password, bool isPersistent, bool lockoutOnFailure)
        {
            var loginResult = await _identityService.SignInUserAsync(email, password, isPersistent, lockoutOnFailure);
            if (!loginResult)
            {
                if (!TemNotificacao())
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
                    var updateResult = await _userManager.UpdateAsync(user); // Usar _userManager.UpdateAsync aqui também
                    if (!updateResult.Succeeded)
                    {
                        _logger.LogError($"Falha ao atualizar LastLoginDate para '{email}'. Erros: {string.Join(", ", updateResult.Errors.Select(e => e.Description))}");
                    }
                    // Removido Commit() aqui também, pois UpdateAsync já salva.
                }
                Notificar("Sucesso", "Login realizado com sucesso!");
                _logger.LogInformation($"Login bem-sucedido para '{email}'.");
            }
            return loginResult;
        }

        /// <summary>
        /// Realiza o logout do usuário (versão mais detalhada).
        /// </summary>
        public async Task RealizarLogout()
        {
            await _identityService.SignOutUserAsync();
            Notificar("Sucesso", "Logout realizado com sucesso.");
            _logger.LogInformation("Usuário deslogado com sucesso via RealizarLogout.");
        }
    }
}
