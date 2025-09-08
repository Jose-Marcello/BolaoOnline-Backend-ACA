// ApostasApp.Core.Application.Services.Interfaces.Usuarios/IUsuarioService.cs
using ApostasApp.Core.Application.DTOs.Apostadores;
using ApostasApp.Core.Application.DTOs.Usuarios;
using ApostasApp.Core.Application.Models; // Para ApiResponse
using ApostasApp.Core.Domain.Models.Identity; // DESCOMENTADO: Para AuthResult, LoginResponseDto, RegisterResponse, UsuarioLoginResult
using ApostasApp.Core.Domain.Models.Usuarios; // Para Usuario
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApostasApp.Core.Application.Services.Interfaces.Usuarios
{
    public interface IUsuarioService
    {
        // Métodos que retornam ApiResponse ou ApiResponse<T>
        Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto request);
        Task<ApiResponse<RegisterResponse>> RegisterAsync(RegisterRequestDto request);
        //Task<ApiResponse<bool>> EsqueciMinhaSenhaAsync(string email, string scheme, string host);
        Task<ApiResponse<bool>> EsqueciMinhaSenhaAsync(string email, string baseUrl);

        Task<ApiResponse<bool>> RedefinirSenhaAsync(string userId, string token, string newPassword);
        Task<ApiResponse<bool>> ConfirmEmail(string userId, string code);
        Task<ApiResponse<bool>> ResendEmailConfirmationAsync(string email, string scheme, string host);
        Task<ApiResponse<bool>> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
        Task<ApiResponse<AuthResult>> ChangeEmail(string userId, string newEmail);
        Task<ApiResponse<bool>> ConfirmChangeEmail(string userId, string newEmail, string code);

        // Métodos que retornam entidades de domínio ou booleanos/strings diretamente
        Task<Usuario> GetLoggedInUser();
        Task<string> GetLoggedInUserId();
        //Task<bool> RegistrarNovoUsuario(string email, string password, string apelido, string cpf, 
        //                                string celular, string scheme, string host, string nomeCompleto);
        Task<bool> RegistrarNovoUsuario(RegisterRequestDto request);
        Task<UsuarioLoginResult> Login(string email, string password, bool rememberMe);
        //Task Logout();
        Task<bool> RealizarLogin(string email, string password, bool isPersistent, bool lockoutOnFailure);
        Task RealizarLogout();
        Task<ApostadorDto> GetUsuarioProfileAsync(string userId);
        Task<bool> ApelidoExiste(string apelido);
        Task<Usuario> ObterUsuarioPorId(Guid userId);
        Task<Usuario> ObterUsuarioPorUsuarioId(string userId);
        Task<Usuario> ObterUsuarioPorEmail(string email);
        // O método GerarJwtToken deve ser private ou não estar na interface se for auxiliar interno.
    }
}
