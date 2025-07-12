// ApostasApp.Core.Application.DTOs.Financeiro/SaldoDto.cs
using System;

namespace ApostasApp.Core.Application.DTOs.Financeiro
{
    public class SaldoDto
    {
        public string Id { get; set; } // ID do Saldo como string
        public string ApostadorId { get; set; } // <<-- CORRIGIDO: ID do Apostador como string -->>
        public decimal Valor { get; set; }
        public DateTime DataUltimaAtualizacao { get; set; }
    }
}
