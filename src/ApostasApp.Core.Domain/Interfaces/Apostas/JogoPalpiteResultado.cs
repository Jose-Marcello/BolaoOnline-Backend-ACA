namespace ApostasApp.Core.Domain.Models.Apostas
{
  public class JogoPalpiteResultado
  {
    public string Id { get; set; }
    public string EquipeCasaNome { get; set; }
    public string EquipeCasaEscudoUrl { get; set; }
    public string EquipeVisitanteNome { get; set; }
    public string EquipeVisitanteEscudoUrl { get; set; }
    public string EstadioNome { get; set; }
    public DateTime DataHoraReal { get; set; }
    public string HoraJogo { get; set; }
    public int? PlacarApostaCasa { get; set; }
    public int? PlacarApostaVisita { get; set; }
  }
}
