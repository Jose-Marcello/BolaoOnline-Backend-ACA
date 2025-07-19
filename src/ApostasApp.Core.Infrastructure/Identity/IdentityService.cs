// Localização: ApostasApp.Core.Infrastructure.Identity/IdentityService.cs
using ApostasApp.Core.Domain.Interfaces.Identity;
using ApostasApp.Core.Domain.Interfaces.Notificacoes;
using ApostasApp.Core.Domain.Models.Identity; // Para AuthResult, LoginResult
using ApostasApp.Core.Domain.Models.Notificacoes; // AGORA USAR Notificacao
using ApostasApp.Core.Domain.Models.Usuarios; // Para a classe Usuario
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
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

namespace ApostasApp.Core.Infrastructure.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;
        private readonly INotificador _notificador;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<IdentityService> _logger;
        private readonly IEmailSender _emailSender;
        private readonly string _apiBaseUrl; // Propriedade para a URL base da API

        public IdentityService(
            UserManager<Usuario> userManager,
            SignInManager<Usuario> signInManager,
            INotificador notificador,
            IEmailSender emailSender,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            ILogger<IdentityService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _notificador = notificador;
            _emailSender = emailSender;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;


            // <<-- CORREÇÃO FINAL: Atribuição correta da URL base da API -->>
            _apiBaseUrl = _configuration["ApiSettings:BaseUrl"];
            if (string.IsNullOrEmpty(_apiBaseUrl))
            {
                _logger.LogError("ApiSettings:BaseUrl não configurado no appsettings.json. O link de confirmação pode estar incorreto.");
            }
        }

        public async Task<Usuario> GetLoggedInUserAsync()
        {
            var userPrincipal = _httpContextAccessor.HttpContext?.User;
            if (userPrincipal == null) return null;
            return await _userManager.GetUserAsync(userPrincipal);
        }

        public async Task<string> GetLoggedInUserIdAsync()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public async Task<AuthResult> RegisterUserAsync(string email, string password, string apelido, string cpf, string celular)
        {
            var user = new Usuario
            {
                UserName = email,
                Email = email,
                Apelido = apelido,
                CPF = cpf,
                Celular = celular,
                EmailConfirmed = true, // false,
                RegistrationDate = DateTime.Now,
                // <<-- INICIALIZAÇÃO DO REFRESH TOKEN E EXPIRATION TIME AQUI -->>
                RefreshToken = Guid.NewGuid().ToString(), // Gera um GUID único
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7)// Expira em 7 dias (ajuste conforme sua política)               
            };   

        var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                // Mapeia IdentityError para Notificacao
                var notifications = result.Errors.Select(error => new Notificacao(null, "Erro", error.Description, error.Code)).ToList();
                foreach (var notif in notifications)
                {
                    _notificador.Handle(notif); // Passa Notificacao
                }
                // AuthResult.Failure agora espera List<Notificacao>
                return AuthResult.Failure(notifications);
            }
            return AuthResult.Succeeded(user.Id, user.Email, user.UserName);
        }

        public async Task<bool> SendConfirmationEmailAsync(Usuario user, string scheme, string host)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            // <<-- CORREÇÃO CRÍTICA: USA A URL BASE DA API E A ROTA CORRETA DO CONTROLADOR -->>
            var confirmationLink = $"{_apiBaseUrl}/confirm-email?userId={user.Id}&code={Uri.EscapeDataString(code)}";
            // A rota do seu AccountController é [HttpGet("confirm-email")]

            try
            {
                var subject = "Confirme seu e-mail - Bolão Online";
                var htmlMessage = $"<p>Olá {user.Apelido},</p><p>Por favor, confirme sua conta clicando neste link:</p><p><a href='{confirmationLink}'>Confirmar E-mail</a></p><p>Se você não solicitou este e-mail, por favor, ignore-o.</p>";

                await _emailSender.SendEmailAsync(user.Email, subject, htmlMessage);

                _notificador.Handle(new Notificacao(null, "Sucesso", $"E-mail de confirmação enviado para {user.Email}.", null));
                _logger.LogInformation($"E-mail de confirmação enviado para {user.Email}. Link: {confirmationLink}");
                return true;
            }
            catch (Exception ex)
            {
                _notificador.Handle(new Notificacao(null, "Erro", $"Falha ao enviar e-mail de confirmação para {user.Email}: {ex.Message}", null));
                _logger.LogError(ex, $"EXCEÇÃO NO ENVIO DE E-MAIL DE CONFIRMAÇÃO para {user.Email}.");
                return false;
            }
        }

        public async Task<Usuario> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<Usuario> GetUserByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<bool> ApelidoExisteAsync(string apelido)
        {
            return await _userManager.Users.AnyAsync(u => u.Apelido == apelido);
        }

        public async Task<LoginResult> LoginAsync(string email, string password, bool rememberMe)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                // Passa Notificacao para o notificador
                var notifs = new List<Notificacao> { new Notificacao(null, "Erro", "Usuário ou senha inválidos.", null) };
                _notificador.Handle(notifs.First()); // Ou use o Handle(List<Notificacao>) se ele estiver na interface
                _logger.LogWarning($"Login falhou para '{email}': Usuário não encontrado.");
                return LoginResult.Failed(notifs); // Retorna List<Notificacao>
            }

            if (_userManager.Options.SignIn.RequireConfirmedAccount && !user.EmailConfirmed)
            {
                // Passa Notificacao para o notificador
                var notifs = new List<Notificacao> { new Notificacao(null, "Erro", "Seu e-mail ainda não foi confirmado.", null) };
                _notificador.Handle(notifs.First());
                _logger.LogWarning($"Login falhou para '{email}': E-mail não confirmado.");
                return LoginResult.Failed(notifs); // Retorna List<Notificacao>
            }

            var result = await _signInManager.PasswordSignInAsync(email, password, rememberMe, lockoutOnFailure: true);

            if (result.RequiresTwoFactor)
            {
                // Passa Notificacao para o notificador
                _notificador.Handle(new Notificacao(null, "Alerta", "Autenticação de dois fatores necessária.", null));
                _logger.LogInformation($"Login para '{email}' requer 2FA.");
                return LoginResult.TwoFactorRequired();
            }

            if (result.IsLockedOut)
            {
                // Passa Notificacao para o notificador
                _notificador.Handle(new Notificacao(null, "Erro", "A conta está bloqueada devido a várias tentativas de login inválidas. Por favor, tente novamente mais tarde.", null));
                _logger.LogWarning($"Login falhou para '{email}': Conta bloqueada.");
                return LoginResult.LockedOut();
            }

            if (result.IsNotAllowed)
            {
                // Passa Notificacao para o notificador
                var notifs = new List<Notificacao> { new Notificacao(null, "Erro", "Você não tem permissão para fazer login.", null) };
                _notificador.Handle(notifs.First());
                _logger.LogWarning($"Login falhou para '{email}': Não permitido.");
                return LoginResult.Failed(notifs); // Retorna List<Notificacao>
            }

            if (!result.Succeeded)
            {
                // Passa Notificacao para o notificador
                var notifs = new List<Notificacao> { new Notificacao(null, "Erro", "Usuário ou senha inválidos.", null) };
                _notificador.Handle(notifs.First());
                _logger.LogWarning($"Login falhou para '{email}': Credenciais inválidas (SignInManager result.Succeeded é false).");
                return LoginResult.Failed(notifs); // Retorna List<Notificacao>
            }

            // O LOGIN FOI BEM-SUCEDIDO AQUI!
            var jwtSecret = _configuration["Jwt:SecretKey"];
            var jwtIssuer = _configuration["Jwt:Issuer"];
            var jwtAudience = _configuration["Jwt:Audience"];

            double jwtExpiresInMinutes = 60;
            if (!double.TryParse(_configuration["Jwt:ExpiresInMinutes"], out jwtExpiresInMinutes))
            {
                _logger.LogWarning("Configuração 'Jwt:ExpiresInMinutes' não encontrada ou inválida. Usando padrão de 60 minutos.");
            }

            _logger.LogInformation($"[JWT_GERA] SecretKey lida: '{(string.IsNullOrWhiteSpace(jwtSecret) ? "[VAZIA]" : "******")}'");
            _logger.LogInformation($"[JWT_GERA] Issuer lido: '{jwtIssuer}'");
            _logger.LogInformation($"[JWT_GERA] Audience lida: '{jwtAudience}'");
            _logger.LogInformation($"[JWT_GERA] Expira em minutos (da configuração): {jwtExpiresInMinutes}");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtSecret);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName),
            };
            if (!string.IsNullOrEmpty(user.Apelido))
            {
                claims.Add(new Claim("apelido", user.Apelido));
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var expires = DateTime.UtcNow.AddMinutes(jwtExpiresInMinutes);

            _logger.LogInformation($"[JWT_GERA] Token gerado, expira em (UTC): {expires.ToString("o")}");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = jwtIssuer,
                Audience = jwtAudience,
                Expires = expires,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var encodedToken = tokenHandler.WriteToken(token);

            var refreshToken = Guid.NewGuid().ToString();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            user.LastLoginDate = DateTime.Now;
            await _userManager.UpdateAsync(user);

            _logger.LogInformation($"[IdentityService] Login bem-sucedido para '{email}'. Token JWT e RefreshToken gerados e retornados.");

            return LoginResult.Succeeded(encodedToken, refreshToken, tokenDescriptor.Expires.Value);
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("Usuário deslogado via LogoutAsync.");
        }

        public async Task SignOutUserAsync()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("Usuário deslogado via SignOutUserAsync.");
        }

        public async Task<bool> SignInUserAsync(string email, string password, bool isPersistent, bool lockoutOnFailure)
        {
            var result = await _signInManager.PasswordSignInAsync(email, password, isPersistent, lockoutOnFailure);
            return result.Succeeded;
        }

        public async Task<bool> ForgotPasswordAsync(string email, string scheme, string host)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || (_userManager.Options.SignIn.RequireConfirmedAccount && !user.EmailConfirmed))
            {
                return true; // Evita enumeração de usuários
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = $"{scheme}://{host}/Account/ResetPassword?userId={user.Id}&code={Uri.EscapeDataString(code)}";
            // Passa Notificacao para o notificador
            _notificador.Handle(new Notificacao(null, "Sucesso", $"Link de redefinição de senha gerado para {user.Email}: {resetLink}", null));
            _logger.LogInformation($"Link de redefinição de senha para {user.Email}: {resetLink}");
            return true;
        }

        public async Task<bool> ResetPasswordAsync(Usuario user, string token, string newPassword)
        {
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    // Mapeia IdentityError para Notificacao
                    _notificador.Handle(new Notificacao(null, "Erro", error.Description, error.Code));
                }
                return false;
            }
            return true;
        }

        public async Task<AuthResult> GenerateChangeEmailTokenAsync(string userId, string newEmail)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                // Retorna AuthResult.Failure com List<Notificacao>
                return AuthResult.Failure(new List<Notificacao> { new Notificacao(null, "Erro", "Usuário não encontrado para geração de token.", null) });
            }

            var token = await _userManager.GenerateChangeEmailTokenAsync(user, newEmail);
            // Passa Notificacao para o notificador
            _notificador.Handle(new Notificacao(null, "Sucesso", $"Token de alteração de e-mail gerado para {newEmail}: {token}", null));
            _logger.LogInformation($"Token de alteração de e-mail para {newEmail}: {token}");

            return AuthResult.Succeeded(user.Id, user.Email, user.UserName);
        }

        public async Task<AuthResult> ChangeEmailAsync(string userId, string newEmail, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                // Retorna AuthResult.Failure com List<Notificacao>
                return AuthResult.Failure(new List<Notificacao> { new Notificacao(null, "Erro", "Usuário não encontrado para alteração de e-mail.", null) });
            }

            var result = await _userManager.ChangeEmailAsync(user, newEmail, code);
            if (!result.Succeeded)
            {
                // Mapeia IdentityError para Notificacao
                var notifications = result.Errors.Select(e => new Notificacao(null, "Erro", e.Description, e.Code)).ToList();
                foreach (var notif in notifications)
                {
                    _notificador.Handle(notif);
                }
                return AuthResult.Failure(notifications);
            }

            user.Email = newEmail;
            user.NormalizedEmail = _userManager.NormalizeEmail(newEmail);
            user.UserName = newEmail;
            user.NormalizedUserName = _userManager.NormalizeName(newEmail);
            await _userManager.UpdateAsync(user);

            return AuthResult.Succeeded(user.Id, user.Email, user.UserName);
        }

        public async Task<bool> ChangePasswordAsync(Usuario user, string currentPassword, string newPassword)
        {
            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (result.Succeeded)
            {
                return true;
            }

            foreach (var error in result.Errors)
            {
                // Mapeia IdentityError para Notificacao
                _notificador.Handle(new Notificacao(null, "Erro", error.Description, error.Code));
            }
            return false;
        }
    }
}
