using ApostasApp.Core.Application.DTOs.Financeiro; // Para SaldoDto, se ainda for usado aqui
using System;

namespace ApostasApp.Core.Application.DTOs.Apostadores
{
    /// <summary>
    /// DTO para representar informações de um Apostador.
    /// Inclui dados do usuário associado (Apelido e Email).
    /// </summary>
    public class ApostadorDto
    {
        public Guid Id { get; set; } // ID da entidade Apostador
        public string UsuarioId { get; set; } // ID do usuário do Identity (string)
        public string Apelido { get; set; } // Apelido do Usuário (vindo da entidade Usuario)
        public string Email { get; set; }   // Email do Usuário (vindo da entidade Usuario)

        public string Status { get; set; } // Status do Apostador (ex: "Ativo", "Inativo")

        // Se o saldo for exibido diretamente no ApostadorDto, use o SaldoDto
        public SaldoDto Saldo { get; set; }
    }
}
