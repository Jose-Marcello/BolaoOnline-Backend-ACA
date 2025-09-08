// ApostadorDto.cs
// Definição do ApostadorDto.
// Localização: ApostasApp.Core.Application.DTOs.Apostadores

using ApostasApp.Core.Application.DTOs.ApostadorCampeonatos;
using ApostasApp.Core.Application.DTOs.Financeiro; // Para SaldoDto
using ApostasApp.Core.Domain.Models.Usuarios;
using System; // Necessário para Guid

namespace ApostasApp.Core.Application.DTOs.Apostadores
{
    /// <summary>
    /// DTO para representar informações de um Apostador.
    /// Inclui dados do usuário associado (Apelido e Email).
    /// </summary>
    public class ApostadorDto
    {
        public string Id { get; set; } // ID da entidade Apostador (Guid como string)
        public string UsuarioId { get; set; } // ID do usuário do Identity (string)
        public string Apelido { get; set; } // Apelido do Usuário (vindo da entidade Usuario)
        public string Email { get; set; }    // Email do Usuário (vindo da entidade Usuario, mapeado do UserName/Email do IdentityUser)

        public string Celular { get; set; }    //celular do usuário

        public string Status { get; set; }    

        public string FotoPerfil { get; set; } //do usuário

        public SaldoDto Saldo { get; set; } // O saldo é parte do ApostadorDto
        public Usuario Usuario { get; set; } // Um Apostador é um usuario

        public ICollection<ApostadorCampeonatoDto> CampeonatosAderidos { get; set; }
    }
}
