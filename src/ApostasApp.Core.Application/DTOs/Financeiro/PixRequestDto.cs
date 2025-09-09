// Exemplo: src/ApostasApp.Core.Application.DTOs/Financeiro/PixRequestDto.cs
using System;

namespace ApostasApp.Core.Application.DTOs.Financeiro
{
    public class PixRequestDto
    {
        public decimal Valor { get; set; }
        public string Chave { get; set; } // Chave do seu estabelecimento/conta
        public string Descricao { get; set; }
        // Dados adicionais para controle, como ID do apostador, se necessário
        public Guid ApostadorId { get; set; }
    }
}