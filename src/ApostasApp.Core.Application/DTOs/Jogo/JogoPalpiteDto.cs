namespace ApostasApp.Core.Application.DTOs.Jogos
{
  public class JogoPalpiteDto : JogoDto
  {
    // Campos específicos para a tela de apostas
    public int? PlacarApostaCasa { get; set; }
    public int? PlacarApostaVisita { get; set; }

    // Auxiliar para exibição amigável no Front-end
    public string DiaSemana => DataHora.ToString("dddd");
    public DateTime DataHoraReal { get; set; } // Tipo DateTime para o Angular entender
    public string HoraJogo { get; set; }       // Mantém a string para exibição direta se preferir
  }
}
