using ApostasApp.Core.Domain.Models.Usuarios; // Este using traz Usuario, LoginResult, AuthResult
using ApostasApp.Core.Domain.Models.Notificacoes; // Para Notificacao
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;

namespace ApostasApp.Core.Domain.Interfaces.Identity
{
    public interface IIdentityService
    {
        Task<Usuario> GetLoggedInUserAsync();
        Task<string> GetLoggedInUserIdAsync();
        Task<bool> RegisterUserAsync(string email, string password, string apelido, string cpf, string celular);

        Task<bool> SendConfirmationEmailAsync(Usuario user, string scheme, string host);
        Task<bool> ConfirmEmailAsync(Usuario user, string code);

        Task<LoginResult> LoginAsync(string email, string password, bool rememberMe);
        Task LogoutAsync();

        Task<string> GeneratePasswordResetTokenAsync(Usuario user);
        Task<bool> SendPasswordResetEmailAsync(Usuario user, string scheme, string host);
        Task<bool> ResetPasswordAsync(Usuario user, string token, string newPassword);
        Task<bool> ForgotPasswordAsync(string email, string scheme, string host);

        Task<AuthResult> GenerateChangeEmailTokenAsync(string userId, string newEmail);
        Task<AuthResult> ChangeEmailAsync(string userId, string newEmail, string code);

        Task<Usuario> GetUserByEmailAsync(string email);
        Task<Usuario> GetUserByIdAsync(string userId);
        Task<bool> ApelidoExisteAsync(string apelido);
        Task<bool> SignInUserAsync(string email, string password, bool isPersistent, bool lockoutOnFailure);
        Task SignOutUserAsync();
        Task<bool> UpdateUserAsync(Usuario user);
    }
}
