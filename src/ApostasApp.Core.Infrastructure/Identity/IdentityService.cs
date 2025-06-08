using ApostasApp.Core.Domain.Interfaces.Identity;
using ApostasApp.Core.Domain.Interfaces.Notificacoes;
using ApostasApp.Core.Domain.Models.Usuarios;
using ApostasApp.Core.Domain.Models.Notificacoes;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Collections.Generic;

namespace ApostasApp.Core.Infrastructure.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;
        private readonly ILogger<IdentityService> _logger;
        private readonly INotificador _notificador;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEmailSender _emailSender;

        public IdentityService(UserManager<Usuario> userManager,
                               SignInManager<Usuario> signInManager,
                               ILogger<IdentityService> logger,
                               INotificador notificador,
                               IHttpContextAccessor httpContextAccessor,
                               IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _notificador = notificador;
            _httpContextAccessor = httpContextAccessor;
            _emailSender = emailSender;
        }

        public async Task<Usuario> GetLoggedInUserAsync()
        {
            var principal = _httpContextAccessor.HttpContext?.User;
            if (principal == null)
            {
                _notificador.Handle(new Notificacao("Alerta", "Nenhum usuário logado detectado."));
                return null;
            }
            return await _userManager.GetUserAsync(principal);
        }

        public Task<string> GetLoggedInUserIdAsync()
        {
            return Task.FromResult(_httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }

        public async Task<bool> RegisterUserAsync(string email, string password, string apelido, string cpf, string celular)
        {
            var user = new Usuario
            {
                UserName = email,
                Email = email,
                Apelido = apelido,
                CPF = cpf,
                Celular = celular,
                RegistrationDate = System.DateTime.Now,
                LastLoginDate = null
            };

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    _notificador.Handle(new Notificacao("Erro", error.Description));
                }
                _logger.LogError($"Falha no registro do usuário {email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                return false;
            }
            return true;
        }

        public async Task<Usuario> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(Usuario user)
        {
            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        /// <summary>
        /// Envia um e-mail de confirmação para o usuário.
        /// </summary>
        public async Task<bool> SendConfirmationEmailAsync(Usuario user, string scheme, string host)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = $"{scheme}://{host}/api/Account/confirm-email?userId={user.Id}&code={WebUtility.UrlEncode(token)}";
            var subject = "Confirme seu e-mail para o Bolão Online";
            var message = $"Por favor, confirme sua conta clicando neste link: <a href='{callbackUrl}'>clique aqui</a>.";

            try
            {
                await _emailSender.SendEmailAsync(user.Email, subject, message);
                _logger.LogInformation($"Email de confirmação ENVIADO REALMENTE para {user.Email} com link: {callbackUrl}");
                return true;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, $"Falha REAL ao enviar email de confirmação para {user.Email}.");
                _notificador.Handle(new Notificacao("Erro", $"Falha ao enviar e-mail de confirmação para {user.Email}. Verifique seu e-mail e tente novamente."));
                return false;
            }
        }

        public async Task<Usuario> GetUserByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        /// <summary>
        /// Confirma o e-mail do usuário usando o token.
        /// Garante que os erros do Identity sejam notificados.
        /// </summary>
        public async Task<bool> ConfirmEmailAsync(Usuario user, string code)
        {
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    _notificador.Handle(new Notificacao("Erro", $"Falha na confirmação de e-mail: {error.Description}"));
                }
                _logger.LogError($"Falha na confirmação de email para {user.Email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
            return result.Succeeded;
        }

        public async Task<LoginResult> LoginAsync(string email, string password, bool rememberMe)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                _notificador.Handle(new Notificacao("Erro", "Usuário não encontrado."));
                return LoginResult.Failure(new List<Notificacao> { new Notificacao("Erro", "Usuário não encontrado.") });
            }

            if (!user.EmailConfirmed && _userManager.Options.SignIn.RequireConfirmedAccount)
            {
                _notificador.Handle(new Notificacao("Erro", "Seu e-mail ainda não foi confirmado."));
                return LoginResult.Failure(new List<Notificacao> { new Notificacao("Erro", "Seu e-mail ainda não foi confirmado.") });
            }

            var result = await _signInManager.PasswordSignInAsync(user, password, rememberMe, lockoutOnFailure: true);

            if (!result.Succeeded)
            {
                var errors = new List<Notificacao>();
                if (result.IsLockedOut)
                {
                    errors.Add(new Notificacao("Erro", "Conta bloqueada. Tente novamente mais tarde."));
                }
                else if (result.IsNotAllowed)
                {
                    errors.Add(new Notificacao("Erro", "Login não permitido."));
                }
                else if (result.RequiresTwoFactor)
                {
                    errors.Add(new Notificacao("Alerta", "Login requer autenticação de dois fatores."));
                }
                else
                {
                    errors.Add(new Notificacao("Erro", "Credenciais inválidas."));
                }
                _notificador.Handle(errors);
                _logger.LogError($"Falha no login para {email}: {result.ToString()}");
                return LoginResult.Failure(errors);
            }

            return LoginResult.Succeeded(user.Id, user.Email, user.UserName);
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<bool> SignInUserAsync(string email, string password, bool isPersistent, bool lockoutOnFailure)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                _notificador.Handle(new Notificacao("Erro", "Usuário não encontrado."));
                return false;
            }

            if (!user.EmailConfirmed)
            {
                _notificador.Handle(new Notificacao("Erro", "Seu e-mail ainda não foi confirmado."));
                return false;
            }

            var result = await _signInManager.PasswordSignInAsync(user, password, isPersistent, lockoutOnFailure);

            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    _notificador.Handle(new Notificacao("Erro", "Conta bloqueada. Tente novamente mais tarde."));
                }
                else if (result.IsNotAllowed)
                {
                    _notificador.Handle(new Notificacao("Erro", "Login não permitido."));
                }
                else if (result.RequiresTwoFactor)
                {
                    _notificador.Handle(new Notificacao("Alerta", "Login requer autenticação de dois fatores."));
                }
                else
                {
                    _notificador.Handle(new Notificacao("Erro", "Credenciais inválidas."));
                }
                _logger.LogError($"Falha no login para {email}: {result.ToString()}");
                return false;
            }
            return true;
        }

        public async Task SignOutUserAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<bool> UpdateUserAsync(Usuario user)
        {
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    _notificador.Handle(new Notificacao("Erro", error.Description));
                }
                _logger.LogError($"Falha na atualização do usuário {user.Email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
            return result.Succeeded;
        }

        public async Task<bool> ApelidoExisteAsync(string apelido)
        {
            return await _userManager.Users.AnyAsync(u => u.Apelido == apelido);
        }

        public async Task<string> GeneratePasswordResetTokenAsync(Usuario user)
        {
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        /// <summary>
        /// Envia um e-mail de redefinição de senha para o usuário.
        /// </summary>
        public async Task<bool> SendPasswordResetEmailAsync(Usuario user, string scheme, string host)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = $"{scheme}://{host}/api/Account/reset-password?userId={user.Id}&token={WebUtility.UrlEncode(token)}";
            var subject = "Redefinição de Senha para o Bolão Online";
            var message = $"Por favor, redefina sua senha clicando neste link: <a href='{callbackUrl}'>clique aqui</a>.";

            try
            {
                await _emailSender.SendEmailAsync(user.Email, subject, message);
                _logger.LogInformation($"Email de redefinição de senha ENVIADO REALMENTE para {user.Email} com link: {callbackUrl}.");
                return true;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, $"Falha REAL ao enviar email de redefinição de senha para {user.Email}.");
                _notificador.Handle(new Notificacao("Erro", $"Falha ao enviar e-mail de redefinição de senha para {user.Email}. Tente novamente."));
                return false;
            }
        }

        public async Task<bool> ResetPasswordAsync(Usuario user, string token, string newPassword)
        {
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    _notificador.Handle(new Notificacao("Erro", error.Description));
                }
                _logger.LogError($"Falha na redefinição de senha para {user.Email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
            return result.Succeeded;
        }

        public async Task<bool> ForgotPasswordAsync(string email, string scheme, string host)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                _notificador.Handle(new Notificacao("Sucesso", "Se o email estiver cadastrado, um link para redefinição de senha foi enviado."));
                return true;
            }

            return await SendPasswordResetEmailAsync(user, scheme, host);
        }

        public async Task<AuthResult> GenerateChangeEmailTokenAsync(string userId, string newEmail)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _notificador.Handle(new Notificacao("Erro", "Usuário não encontrado."));
                return AuthResult.Failure(new List<Notificacao> { new Notificacao("Erro", "Usuário não encontrado.") });
            }

            var token = await _userManager.GenerateChangeEmailTokenAsync(user, newEmail);
            _logger.LogInformation($"Simulando envio de email de mudança de e-mail para {newEmail} com token: {token}");
            _notificador.Handle(new Notificacao("Sucesso", $"E-mail de mudança de e-mail simulado enviado para {newEmail}."));
            return AuthResult.Succeeded(user.Id, user.Email, user.UserName);
        }

        public async Task<AuthResult> ChangeEmailAsync(string userId, string newEmail, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _notificador.Handle(new Notificacao("Erro", "Usuário não encontrado."));
                return AuthResult.Failure(new List<Notificacao> { new Notificacao("Erro", "Usuário não encontrado.") });
            }

            var result = await _userManager.ChangeEmailAsync(user, newEmail, code);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    _notificador.Handle(new Notificacao("Erro", error.Description));
                }
                return AuthResult.Failure(result.Errors.Select(e => new Notificacao("Erro", e.Description)).ToList());
            }
            _notificador.Handle(new Notificacao("Sucesso", "E-mail alterado com sucesso."));
            return AuthResult.Succeeded(user.Id, newEmail, user.UserName);
        }
    }
}
