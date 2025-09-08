using System.ComponentModel.DataAnnotations;
using ApostasApp.Core.Domain.Models.Apostadores;
using Microsoft.AspNetCore.Identity; // Necessário para IdentityUser
using System; // Necessário para DateTime

namespace ApostasApp.Core.Domain.Models.Usuarios
{
    /// <summary>
    /// Entidade de domínio que representa um usuário, estendendo as funcionalidades do ASP.NET Core Identity.
    /// Herda de IdentityUser, o que significa que o ID do usuário é do tipo string (padrão do Identity).
    /// </summary>
    public class Usuario : IdentityUser
    {
        [Required]
        [MaxLength(11)]
        [ProtectedPersonalData]
        public string CPF { get; set; }

        [MaxLength(20)]
        [ProtectedPersonalData]
        public string Celular { get; set; }

        [MaxLength(50)]
        [ProtectedPersonalData]
        public string Apelido { get; set; }

        [StringLength(255)]
        public string FotoPerfil { get; set; }

        public bool TermsAccepted { get; set; }

        // Propriedade de navegação para a entidade Apostador (se um usuário pode ser um apostador)
        public Apostador Apostador { get; set; }

        public DateTime? RegistrationDate { get; set; }
        public DateTime? LastLoginDate { get; set; }

        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }

        /// <summary>
        /// Construtor padrão sem parâmetros, necessário para o ASP.NET Core Identity e Entity Framework Core.
        /// </summary>
        public Usuario() : base() { }

        /// <summary>
        /// Construtor para criar uma nova instância de Usuario com dados básicos.
        /// </summary>
        /// <param name="apelido">O apelido do usuário.</param>
        /// <param name="email">O email do usuário (será usado como UserName).</param>
        /// <param name="cpf">O CPF do usuário.</param>
        /// <param name="celular">O número de celular do usuário (opcional).</param>
        public Usuario(string apelido, string email, string cpf, string celular = null) : base(email) // O construtor base de IdentityUser define UserName
        {
            Apelido = apelido;
            Email = email; // Definir o Email também
            CPF = cpf;
            Celular = celular;
            RegistrationDate = DateTime.Now; // Define a data de registro ao criar

            // *** NOVO: Inicializa as propriedades do Refresh Token ***
            RefreshToken = null; // Ou string.Empty, dependendo da sua preferência para valores nulos
            RefreshTokenExpiryTime = DateTime.MinValue; // Ou DateTime.UtcNow; se preferir um valor inicial real
            // ******************************************************
        }
    }
}
