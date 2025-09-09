namespace ApostasApp.Core.Application.DTOs.Conferencia
{
    public class ConferenciaPalpiteDto
    {
        public string ApelidoApostador { get; set; }
        public string IdentificadorAposta { get; set; }
        //public DateTime DataEnvioAposta { get; set; }
        public string NomeEquipeCasa { get; set; }
        public int PlacarPalpiteCasa { get; set; }
        public int PlacarPalpiteVisita { get; set; }
        public string NomeEquipeVisita { get; set; }
        public string TituloCampeonato { get; set; }
        public int NumeroRodada { get; set; }
        public DateTime DataHoraEnvio { get; set; }

        public DateTime DataJogo { get; set; }
        public string HoraJogo { get; set; }
    }
}