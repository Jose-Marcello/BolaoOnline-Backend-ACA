// Exemplo: ApostasApp.Core.Domain.Interfaces.Usuarios\IUsuarioService.cs
using System.Threading.Tasks;
using ApostasApp.Core.Application.DTOs.Apostadores;
using ApostasApp.Core.Application.DTOs.Usuarios;
using ApostasApp.Core.Application.Models; // Para ApiResponse
using ApostasApp.Core.Domain.Models.Usuarios; // Para Usuario (se usado em ObterUsuarioPorId/Email)
using System; // Para Guid
using ApostasApp.Core.Domain.Interfaces.Identity; // Para AuthResult (se ainda for usado para ChangeEmail)

namespace ApostasApp.Core.Domain.Interfaces.Usuarios
{
    public interface IUsuarioService
    {
        Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto request);
        Task<ApiResponse<RegisterResponse>> RegisterAsync(RegisterRequestDto request);

        // <<-- AJUSTADO: Métodos de autenticação adicionais com retorno ApiResponse<T> -->>
        Task<ApiResponse<bool>> EsqueciMinhaSenhaAsync(string email, string scheme, string host);
        Task<ApiResponse<bool>> RedefinirSenhaAsync(string userId, string token, string newPassword);
        Task<ApiResponse<bool>> ConfirmEmail(string userId, string code);
        Task<ApiResponse<bool>> ResendEmailConfirmationAsync(string email, string scheme, string host);

        // Novo método para alteração de senha (para usuário logado)
        Task<ApiResponse<bool>> ChangePasswordAsync(string userId, string currentPassword, string newPassword);

        // Mantendo AuthResult para ChangeEmail se for um requisito específico,
        // caso contrário, seria ApiResponse<bool> ou ApiResponse<ChangeEmailResponseDto>
        Task<ApiResponse<AuthResult>> ChangeEmail(string userId, string newEmail);
        Task<ApiResponse<bool>> ConfirmChangeEmail(string userId, string newEmail, string code);

        // Métodos existentes (mantidos como estão se não forem chamados pelo controlador com ApiResponse)
        Task<bool> RegistrarNovoUsuario(string email, string password, string apelido, string cpf, string celular, string scheme, string host);
        Task<Usuario> GetLoggedInUser();
        Task<string> GetLoggedInUserId();
        Task<bool> ApelidoExiste(string apelido);
        Task<Usuario> ObterUsuarioPorId(Guid userId);
        Task<Usuario> ObterUsuarioPorUsuarioId(string userId);
        Task<Usuario> ObterUsuarioPorEmail(string email);
        Task<bool> RealizarLogin(string email, string password, bool isPersistent, bool lockoutOnFailure);
        Task RealizarLogout();
        Task<ApostadorDto> GetUsuarioProfileAsync(string userId);
    }
}
