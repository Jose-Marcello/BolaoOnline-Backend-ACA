using ApostasApp.Core.Application.DTOs.Palpites;

namespace ApostasApp.Core.Application.DTOs.ApostasRodada
{
  public class ApostaRodadaDto
  {
    public string Id { get; set; }
    public string ApostadorCampeonatoId { get; set; }
    public string RodadaId { get; set; }
    public string IdentificadorAposta { get; set; }
    public DateTime? DataHoraSubmissao { get; set; }
    public bool EhApostaCampeonato { get; set; }
    public bool EhApostaIsolada { get; set; }
    public decimal? CustoPagoApostaRodada { get; set; }
    public int PontuacaoTotalRodada { get; set; }
    public int StatusAposta { get; set; }
    public bool PodeEditar { get; set; }
    public bool Enviada { get; set; }
    public int NumJogosApostados { get; set; }

    // <<-- NOVOS CAMPOS PARA O GRID 1 E CABEÇALHO -->>
    public int NumeroRodada { get; set; }     // Necessário para o Item 1
    public DateTime DataInicio { get; set; }  // Necessário para o Item 1
    public DateTime DataFim { get; set; }     // Necessário para o Item 1
    public string DescricaoRodada { get; set; } // "Rodada X"

    public ApostadorCampeonatos.ApostadorCampeonatoDto ApostadorCampeonato { get; set; }
    public IEnumerable<PalpiteDto> Palpites { get; set; }
  }
}
