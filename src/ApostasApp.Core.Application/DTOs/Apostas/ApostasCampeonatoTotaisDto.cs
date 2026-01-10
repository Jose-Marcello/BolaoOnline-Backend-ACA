// ApostasApp.Core.Application.DTOs.Apostas/ApostaJogoDto.cs
using System;

namespace ApostasApp.Core.Application.DTOs.Apostas
{
  public class ApostasCampeonatoTotaisDto
  {
    // O que o HTML chama de 'quantApostadoresVinculados'
    public int QuantApostadoresVinculados { get; set; }

    // O que o HTML chama de 'valorArrecadado' (Prêmio Acumulado)
    public decimal ValorArrecadado { get; set; }

    // Para as apostas avulsas que discutimos (Rodapé do card)
    public decimal PremioAvulsoRodada { get; set; }

    // Mantendo os originais para compatibilidade se necessário
    public int NumeroDeApostadores { get; set; }
    public decimal ValorTotalArrecadado { get; set; }
  }
}
