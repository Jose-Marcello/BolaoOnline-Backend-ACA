using ApostasApp.Core.Domain.Models.Usuarios; // Este using traz Usuario, LoginResult, AuthResult
using System;
using System.Threading.Tasks;

namespace ApostasApp.Core.Domain.Interfaces.Usuarios
{
    public interface IUsuarioService
    {
        Task<Usuario> GetLoggedInUser();
        Task<string> GetLoggedInUserId();

        Task<bool> RegistrarNovoUsuario(string email, string password, string apelido, string cpf, string celular, string scheme, string host);

        Task<LoginResult> Login(string email, string password, bool rememberMe);
        Task Logout();

        Task<bool> ConfirmEmail(string userId, string code);

        Task<bool> EsqueciMinhaSenhaAsync(string email, string scheme, string host);
        Task<bool> RedefinirSenhaAsync(string userId, string token, string newPassword);

        Task<bool> ResendEmailConfirmationAsync(string email, string scheme, string host);

        Task<AuthResult> ChangeEmail(string userId, string newEmail);
        Task<bool> ConfirmChangeEmail(string userId, string newEmail, string code);

        Task<Usuario> ObterUsuarioPorId(Guid userId);
        Task<Usuario> ObterUsuarioPorEmail(string email);
        Task<bool> ApelidoExiste(string apelido);
        Task<bool> RealizarLogin(string email, string password, bool isPersistent, bool lockoutOnFailure);
        Task RealizarLogout();
    }
}
