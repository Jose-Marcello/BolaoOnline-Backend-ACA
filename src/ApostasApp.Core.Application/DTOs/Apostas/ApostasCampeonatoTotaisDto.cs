public class ApostasCampeonatoTotaisDto
{
  // SETOR 1: VINCULADAS
  public int QuantVinculados { get; set; }
  public decimal ArrecadacaoVinculados { get; set; }
  public decimal PremioFinalCampeonato { get; set; }

  // SETOR 2: CORRENTE (EM DISPUTA)
  public string RodadaCorrenteId { get; set; } // Ex: "01"
  public int QuantApostasCorrentes { get; set; }
  public decimal ArrecadacaoCorrente { get; set; }
  public decimal PremioCorrente { get; set; }

  // SETOR 3: EM APOSTA (CRESCENDO)
  public string RodadasEmApostaIds { get; set; } // Ex: "03, 04"
  public int QuantApostasAvulsas { get; set; }
  public decimal ArrecadacaoAvulsaRodada { get; set; }
  public decimal PremioLiquidoRodada { get; set; }
}
