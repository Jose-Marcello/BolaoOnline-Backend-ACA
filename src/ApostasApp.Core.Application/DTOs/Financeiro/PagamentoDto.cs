using System;
using System.ComponentModel.DataAnnotations;

namespace ApostasApp.Core.Application.DTOs.Financeiro
{
    /// <summary>
    /// Representa os dados de uma transação de pagamento.
    /// Usado para simular o estado de um pagamento no ambiente de testes.
    /// </summary>
    public class PagamentoDto
    {
        /// <summary>
        /// ID da transação no sistema de pagamento externo (simulado).
        /// </summary>
        [Required]
        public string IdTransacao { get; set; }

        /// <summary>
        /// Valor da transação.
        /// </summary>
        [Required]
        public decimal Valor { get; set; }

        /// <summary>
        /// Status atual da transação (ex: PENDENTE, CONCLUIDO, REJEITADO).
        /// </summary>
        [Required]
        public string Status { get; set; }

        /// <summary>
        /// Data e hora em que a transação foi criada (simulado).
        /// </summary>
        public DateTime DataCriacao { get; set; }

        /// <summary>
        /// Referência externa da transação, se houver.
        /// </summary>
        public string ExternalReference { get; set; }
    }
}