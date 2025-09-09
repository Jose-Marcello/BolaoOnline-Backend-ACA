// ApostasApp.Core.Application.DTOs.Apostas/ApostaJogoDto.cs
// DTO para enviar os dados dos jogos e palpites existentes do backend para o frontend (visualização).
using System;
using System.Text.Json.Serialization; // Necessário para JsonPropertyName

namespace ApostasApp.Core.Application.DTOs.Apostas
{
    public class ApostasCampeonatoTotaisDto
    {
        public int NumeroDeApostadores { get; set; }
        public decimal ValorTotalArrecadado { get; set; }
    }
}
