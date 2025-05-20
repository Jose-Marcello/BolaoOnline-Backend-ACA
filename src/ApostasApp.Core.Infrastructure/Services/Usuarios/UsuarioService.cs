using ApostasApp.Core.Domain.Models.Interfaces;
using ApostasApp.Core.Domain.Models.Usuarios;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using ApostasApp.Core.Domain.Models.Results;
using ApostasApp.Core.Domain.Models.Interfaces.Usuarios;
using ApostasApp.Core.Domain.Models.Configuracoes;
using ApostasApp.Core.InfraStructure.Data.Context;
using Microsoft.AspNetCore.Identity.UI.Services;
using ApostasApp.Core.Domain.Models.Notificacoes;

namespace ApostasApp.Core.InfraStructure.Services.Usuarios
{
    public class UsuarioService : BaseService, IUsuarioService
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;
        private readonly LinkGenerator _linkGenerator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISendGridClient _sendGridClient;
        //private readonly IUrlHelper _urlHelper;
        private readonly ILogger<UsuarioService> _logger;
        private MeuDbContext _context;
        private DbContextOptions<MeuDbContext> _options;
        private readonly IOptions<SendGridSettings> _sendGridSettings;
        private readonly INotificador _notificador;


        public UsuarioService(UserManager<Usuario> userManager,
                           SignInManager<Usuario> signInManager,
                           INotificador notificador,
                           LinkGenerator linkGenerator,
                           MeuDbContext context,
                           DbContextOptions<MeuDbContext> options,
                           ISendGridClient sendGridClient,
                           IOptions<SendGridSettings> sendGridSettings,
                           ILogger<UsuarioService> logger,
                           IHttpContextAccessor httpContextAccessor) : base(notificador)
                           //IUrlHelper urlHelper) : base(notificador)
                            //ILogger logger,
        
        {

            _userManager = userManager;
            _signInManager = signInManager;
            _linkGenerator = linkGenerator;
            _httpContextAccessor = httpContextAccessor;
            _sendGridClient = sendGridClient;
            _sendGridSettings = sendGridSettings;
            //_urlHelper = urlHelper;
            _logger = logger;
             _context = context;
            _options = options;
            _notificador = _notificador;
        }

        //public async Task<IdentityResult> RegisterUserAsync(string email, string password)
        public async Task<IdentityResult> RegisterUserAsync(Usuario user, string password)
        {

            /*if (_uow.Usuarios.Buscar(a => a.CPF == apostador.CPF).Result.Any())           
            {
                Notificar("Já existe um apostador com este CPF informado.");
                return;
            }
            if (_uow.Usuarios.Buscar(a => a.Nome == apostador.Nome).Result.Any())            
                {
                    Notificar("Já existe um apostador com este NOME informado.");
                return;
            }
 */
            //var user = new Usuario { UserName = email, Email = email };
            var result = await _userManager.CreateAsync(user, password); // Declare e atribua result

            if (result.Succeeded)
            {

                // Gerar token de confirmação
                //var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);


                //var confirmationLink = _linkGenerator.GetUriByAction(
                // _httpContextAccessor.HttpContext,
                //"ConfirmEmail",
                // "Account",
                //  new { userId = user.Id, token = token },
                //  _httpContextAccessor.HttpContext.Request.Scheme);

                // Enviar email de confirmação
                //await SendConfirmationEmailAsync(user.Email, confirmationLink);
            }

            return result;

        }

        public async Task SendConfirmationEmailAsync(Usuario user, string callbackUrl)
        {
            try
            {
                var from = new EmailAddress(_sendGridSettings.Value.FromEmail, _sendGridSettings.Value.FromName);
                var email = user.Email;
                var to = new EmailAddress(email, user.UserName);
                var subject = "Confirmação de Registro";
                var plainTextContent = $"Por favor, confirme seu registro clicando no link abaixo: {callbackUrl}";
                var htmlContent = $@"
                                 <p>Por favor, confirme seu registro clicando no link abaixo:</p>
                                 <a href=""{callbackUrl}"">Confirmar Registro</a>
                                     ";

                var msg = new SendGridMessage();
                msg.SetFrom(from);
                msg.AddTo(to);
                msg.Subject = subject;

                if (!string.IsNullOrEmpty(plainTextContent))
                {
                    msg.AddContent(MimeType.Text, plainTextContent);
                }
                if (!string.IsNullOrEmpty(htmlContent))
                {
                    msg.AddContent(MimeType.Html, htmlContent);
                }

                var response = await _sendGridClient.SendEmailAsync(msg);

                if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
                {
                    _logger.LogInformation($"E-mail de confirmação enviado para {user.Email}.");
                }
                else
                {
                    var body = await response.Body.ReadAsStringAsync();
                    _logger.LogError($"Falha ao enviar e-mail de confirmação para {user.Email}. StatusCode: {response.StatusCode}, Body: {body}");
                    Notificar("Ocorreu um erro ao enviar o e-mail de confirmação.");
                }
            }
            catch (HttpRequestException ex)
            {
                // Lidar com problemas de conexão
                //_logger.LogError(ex, "Erro de conexão ao enviar email de confirmação.");
                Notificar("Ocorreu um erro de conexão ao enviar o email de confirmação.");
            }
            /*catch (SendGridException ex)
            {
                // Lidar com erros da API do SendGrid
                _logger.LogError(ex, "Erro da API do SendGrid ao enviar email de confirmação.");
                Notificar("Ocorreu um erro ao enviar o email de confirmação.");
            }*/
            catch (Exception ex)
            {
                // Lidar com outros erros
                _logger.LogError(ex, "Erro inesperado ao enviar email de confirmação.");
                Notificar("Ocorreu um erro ao enviar o email de confirmação.");
            }
         }


        public async Task<OperationResult> EsqueciMinhaSenhaAsync(string email, string callbackUrl)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return OperationResult.CreateFailure("Usuário não encontrado.");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var sendGridMessage = new SendGridMessage
            {
                From = new EmailAddress(_sendGridSettings.Value.FromEmail, _sendGridSettings.Value.FromName),
                Subject = "Redefinir Senha",
                HtmlContent = $"Clique <a href='{callbackUrl}'>aqui</a> para redefinir sua senha."
            };
            sendGridMessage.AddTo(email);

            var response = await _sendGridClient.SendEmailAsync(sendGridMessage);

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Body.ReadAsStringAsync();
                _logger.LogError($"Erro ao enviar e-mail de redefinição de senha para {email}: {errorResponse}");
                Notificar("Erro ao enviar o e-mail de redefinição de senha.");
                return OperationResult.CreateFailure("Erro ao enviar o e-mail de redefinição de senha.");
            }

            return OperationResult.CreateSuccess("E-mail de redefinição de senha enviado com sucesso. Por favor, verifique sua caixa de entrada (e spam).");
        }

        public async Task ResendEmailConfirmationAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || user.EmailConfirmed)
            {
                _notificador.Handle(new Notificacao("Este e-mail já foi confirmado ou não existe."));
                return;
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var tokenEncoded = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(token));
            var callbackUrl = _linkGenerator.GetUriByAction(
                httpContext: _httpContextAccessor.HttpContext,
                action: "ConfirmEmail",
                controller: "Account",
                values: new { userId = user.Id, code = tokenEncoded },
                scheme: _httpContextAccessor.HttpContext.Request.Scheme);

            _logger.LogInformation($"[REENVIO EMAIL] Token gerado para o usuário {user.Id}: {token}");
            _logger.LogInformation($"[REENVIO EMAIL] URL de confirmação gerada: {callbackUrl}");

            try
            {
                var from = new EmailAddress(_sendGridSettings.Value.FromEmail, _sendGridSettings.Value.FromName);
                var to = new EmailAddress(email, user.UserName); // Assumindo que UserName está sempre preenchido
                var subject = "Reenvio de Confirmação de Registro"; // Assunto atualizado
                var plainTextContent = $"Por favor, clique no link abaixo para confirmar seu e-mail novamente: {callbackUrl}"; // Texto atualizado
                var htmlContent = $@"
                                <p>Você solicitou o reenvio do e-mail de confirmação.</p>
                                <p>Por favor, clique no link abaixo para confirmar seu e-mail:</p>
                                <p><a href=""{callbackUrl}"">Confirmar E-mail</a></p>
                                "; // HTML atualizado

                var msg = new SendGridMessage();
                msg.SetFrom(from);
                msg.AddTo(to);
                msg.Subject = subject;

                if (!string.IsNullOrEmpty(plainTextContent))
                {
                    msg.AddContent(MimeType.Text, plainTextContent);
                }
                if (!string.IsNullOrEmpty(htmlContent))
                {
                    msg.AddContent(MimeType.Html, htmlContent);
                }

                var response = await _sendGridClient.SendEmailAsync(msg);

                if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
                {
                    _logger.LogInformation($"[REENVIO EMAIL] E-mail de reenvio de confirmação enviado para {email}.");
                }
                else
                {
                    var body = await response.Body.ReadAsStringAsync();
                    _logger.LogError($"[REENVIO EMAIL] Falha ao enviar e-mail de reenvio de confirmação para {email}. StatusCode: {response.StatusCode}, Body: {body}");
                    _notificador.Handle(new Notificacao($"Ocorreu um erro ao reenviar o e-mail de confirmação para {email}."));
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "[REENVIO EMAIL] Erro de conexão ao enviar e-mail de reenvio de confirmação.");
                _notificador.Handle(new Notificacao("Ocorreu um erro de conexão ao reenviar o e-mail de confirmação."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[REENVIO EMAIL] Erro inesperado ao enviar e-mail de reenvio de confirmação.");
                _notificador.Handle(new Notificacao("Ocorreu um erro ao reenviar o e-mail de confirmação."));
            }
        }


        public async Task<IdentityResult> ConfirmEmailAsync(Usuario user, string code)
        {
            // Aqui você usará o UserManager para confirmar o e-mail
            var result = await _userManager.ConfirmEmailAsync(user, code);

            return result;
            


        }
        public async Task<Microsoft.AspNetCore.Identity.SignInResult> LoginUserAsync(string email, string password, bool rememberMe)
        {
            return await _signInManager.PasswordSignInAsync(email, password, rememberMe, lockoutOnFailure: false);
        }
        public async Task LogoutUserAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(Usuario user)
        {
            _logger.LogInformation($"PasswordResetTokenProvider usado para geração: {_userManager.Options.Tokens.PasswordResetTokenProvider?.GetType().FullName}");
            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task<string> GeneratePasswordResetTokenAsync(Usuario user)
        {
            return await _userManager.GeneratePasswordResetTokenAsync(user);

        }

        public async Task<bool> IsUserInRole(Usuario user, string roleName)
        {
            return await _userManager.IsInRoleAsync(user, roleName);
        }
        public async Task<IdentityResult> ResetPasswordAsync(Usuario user, string token, string newPassword)
        {

            var isTokenValid = await _userManager.VerifyUserTokenAsync(
                  user,
                  _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword",    token);

             _logger.LogInformation($"Validade do Token antes de ResetPasswordAsync: {isTokenValid}");


            _logger.LogInformation($"Tipo do PasswordResetTokenProvider usado para verificação (implícito no UserManager): {_userManager.Options.Tokens.ProviderMap[_userManager.Options.Tokens.PasswordResetTokenProvider]?.GetType().FullName}");

            _logger.LogInformation($"PasswordResetTokenProvider usado para verificação (implícito no UserManager): {_userManager.Options.Tokens.PasswordResetTokenProvider?.GetType().FullName}");
            _logger.LogInformation($"ResetPasswordAsync chamado para o usuário com ID: {user?.Id}, Token: {token?.Substring(0, 9)}..., Nova Senha (primeiros 9 chars): {newPassword?.Substring(0, Math.Min(newPassword.Length, 9))}"); // Log da nova senha


            var initialPasswordHash = user.PasswordHash; // Captura o hash original

            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            if (result.Succeeded)
            {

                await _context.SaveChangesAsync();

                // REVERIFICAR SE O PASSWORDHASH MUDOU APÓS A OPERAÇÃO (CONTORNANDO POSSÍVEL BUG INTERNO)
                var updatedUser = await _userManager.FindByIdAsync(user.Id);
                if (updatedUser != null && updatedUser.PasswordHash == initialPasswordHash)
                {
                    _logger.LogError($"POSSÍVEL FALHA SILENCIOSA NA REDEFINIÇÃO DE SENHA (PasswordHash não foi alterado).");
                    return IdentityResult.Failed(new IdentityError { Code = "PasswordResetFailedSilently", Description = "Falha ao redefinir a senha." });
                }
                _logger.LogInformation($"Senha redefinida com sucesso para o usuário com ID: {user.Id}");
            }
            else
            {
                _logger.LogError($"Erro ao redefinir senha para o usuário com ID: {user.Id}. Erros: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            return result;
        }


        public async Task SignInAsync(Usuario user, bool isPersistent) // Implementado
        {
            await _signInManager.SignInAsync(user, isPersistent);
        }


        public async Task<Usuario> FindByEmailAsync(string email) // Implementado
        {
            return await _userManager.FindByEmailAsync(email);
        }


        public async Task<Usuario> FindByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<bool> ApelidoExiste(string apelido)
        {
            return await _userManager.Users.AnyAsync(u => u.Apelido == apelido);
        }

        public async Task<bool> UsuarioExiste(string userName)
        {
            return await _userManager.Users.AnyAsync(u => u.UserName == userName);
        }


        public async Task<Usuario> ObterUsuarioPorId(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

       

        //public async Task<string> GetLoggedInUserId()
        public string GetLoggedInUserId()
        {
            using (var context = new MeuDbContext(_options))
            {
                var user =   _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User).Result;

                if (user != null)
                {
                    return user.Id.ToString();
                }

                return null;
            }
        }

        public async Task<Usuario> GetLoggedInUser()
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);

            if (user != null)
            {
                // O usuário não está autenticado
                return user;
            }

            return null;


        }

        

        // Outras implementações...
    }

}