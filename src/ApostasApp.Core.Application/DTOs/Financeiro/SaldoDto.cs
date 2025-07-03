// SaldoDto.cs
// Localização: ApostasApp.Core.Application.DTOs.Financeiro
using System;
using System.ComponentModel.DataAnnotations;
namespace ApostasApp.Core.Application.DTOs.Financeiro
{
    public class SaldoDto
    {
        public Guid ApostadorId { get; set; }
        [Required(ErrorMessage = "O valor do saldo é obrigatório.")]
        [Range(0, (double)decimal.MaxValue, ErrorMessage = "O valor do saldo deve ser maior ou igual a zero.")]
        public decimal Valor { get; set; }      
        public DateTime DataUltimaAtualizacao { get; set; }
    }
}
