// Em ApostasApp.Core.Application.DTOs.Financeiro/DepositRequestDto.cs
using System.ComponentModel.DataAnnotations;

namespace ApostasApp.Core.Application.DTOs.Financeiro // Mantenha seu namespace existente
{
    public class DepositarRequestDto
    {

        public Guid ApostadorId { get; set; }
        

        [Required(ErrorMessage = "O valor do depósito é obrigatório.")]
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "O valor do depósito deve ser maior que zero.")]
        public decimal Valor { get; set; }
    }
}