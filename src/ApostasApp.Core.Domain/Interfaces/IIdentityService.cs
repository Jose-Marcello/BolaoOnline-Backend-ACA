// Localização: ApostasApp.Core.Domain.Interfaces.Identity\IIdentityService.cs
using ApostasApp.Core.Domain.Models.Identity; // Para LoginResult e AuthResult
using ApostasApp.Core.Domain.Models.Usuarios; // Para Usuario
using System.Security.Claims; // Para AuthResult (se AuthResult usa Claims, embora não diretamente na interface)
using System.Threading.Tasks;
using System; // Para DateTime (se usado em LoginResult/AuthResult)
using System.Collections.Generic; // Para List (se usado em LoginResult/AuthResult)

namespace ApostasApp.Core.Domain.Interfaces.Identity
{
    public interface IIdentityService
    {
        // Métodos relacionados a usuário
        Task<Usuario> GetLoggedInUserAsync();
        Task<string> GetLoggedInUserIdAsync();
        Task<AuthResult> RegisterUserAsync(string email, string password, string apelido, string cpf,
                                           string celular, string fotoPerfil, string nomeCompleto, 
                                           bool termsAccepted ); // Retorna AuthResult       


        Task<bool> SendConfirmationEmailAsync(Usuario user, string scheme, string host);
        Task<Usuario> GetUserByEmailAsync(string email);
        Task<Usuario> GetUserByIdAsync(string userId);
        Task<bool> ApelidoExisteAsync(string apelido);

        // Métodos de Autenticação
        Task<LoginResult> LoginAsync(string email, string password, bool rememberMe); // Retorna LoginResult
        Task LogoutAsync(); // Seu método existente
        Task<bool> SignInUserAsync(string email, string password, bool isPersistent, bool lockoutOnFailure);
        Task SignOutUserAsync(); // Seu método existente

        // Métodos de Redefinição de Senha e Confirmação de E-mail
        //Task<bool> ForgotPasswordAsync(string email, string scheme, string host);
        //Task<bool> ForgotPasswordAsync(string email, string baseUrl);
        Task<string> ForgotPasswordAsync(string email, string baseUrl); // DEVE retornar string (o link)

    Task<bool> ResetPasswordAsync(Usuario user, string token, string newPassword);

        // Métodos de Alteração de E-mail
        Task<AuthResult> GenerateChangeEmailTokenAsync(string userId, string newEmail); // Retorna AuthResult
        Task<AuthResult> ChangeEmailAsync(string userId, string newEmail, string code); // Retorna AuthResult

        // Assinatura do método ChangePasswordAsync
        Task<bool> ChangePasswordAsync(Usuario user, string currentPassword, string newPassword);
    }
}
