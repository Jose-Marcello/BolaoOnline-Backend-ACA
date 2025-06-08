using System;
using System.ComponentModel.DataAnnotations;

namespace ApostasApp.Core.Application.DTOs.Financeiro
{
    /// <summary>
    /// DTO para representar o saldo de um apostador.
    /// A propriedade ApostadorId identifica a qual apostador este saldo pertence.
    /// </summary>
    public class SaldoDto
    {
        // Propriedade para identificar o apostador, alinhado com a entidade Saldo
        public Guid ApostadorId { get; set; }

        // O valor do saldo (para exibição ou inicialização)
        [Required(ErrorMessage = "O valor do saldo é obrigatório.")]
        [Range(0, (double)decimal.MaxValue, ErrorMessage = "O valor do saldo deve ser maior ou igual a zero.")]
        public decimal Valor { get; set; }

        // Moeda (opcional, mas bom ter se for um requisito)
        public string Moeda { get; set; } = "BRL";

        // Data da última atualização do saldo
        public DateTime DataUltimaAtualizacao { get; set; }
    }
}
