// Exemplo: C:\Projetos\BolaoOnline\ApostasApp.Core.Application.DTOs.Usuarios\RegisterResponse.cs
namespace ApostasApp.Core.Application.DTOs.Usuarios
{
    public class RegisterResponse
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Apelido { get; set; }
        // Adicione outras propriedades que você deseja retornar após o registro
    }
}