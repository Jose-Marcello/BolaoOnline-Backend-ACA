namespace ApostasApp.Core.Presentation.ViewModels
{
    public class ApostasPorRodadaEApostadorViewModel
    {
        // Propriedades para identificar o apostador e o campeonato
        public Guid ApostadorCampeonatoId { get; set; }
        public string ApostadorApelido { get; set; }
        public string CampeonatoNome { get; set; }

        // Propriedades para identificar a rodada
        public Guid RodadaId { get; set; }
        public int NumeroRodada { get; set; }

        // Propriedades para o status e data/hora da aposta (geral da rodada)
        public string StatusEnvioAposta { get; set; }
        public string DataAposta { get; set; } // Representação formatada da data
        public string HoraAposta { get; set; } // Representação formatada da hora

        // Se você precisar de uma lista de apostas diretamente no ViewModel para outros fins,
        // você pode adicioná-la aqui. No entanto, para o DataTable, os dados são buscados via AJAX.
        // public List<ApostaViewModel> Apostas { get; set; }
    }

    // Exemplo de um ViewModel para uma única aposta, caso você precise de uma lista delas no futuro.
    // Este não é diretamente usado pelo DataTable via AJAX, mas seria útil se você passasse
    // a lista de apostas diretamente para a View.
  
}
