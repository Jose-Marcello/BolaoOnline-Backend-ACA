using ApostasApp.Core.Domain.Models.Base;
using ApostasApp.Core.Domain.Models.Campeonatos;
using ApostasApp.Core.Domain.Models.Jogos;
using DApostasApp.Core.Domain.Models.RankingRodadas;


namespace ApostasApp.Core.Domain.Models.Rodadas{ 
    
    public class Rodada : Entity
    {
        public Guid CampeonatoId { get; set; }

        public int NumeroRodada { get; set; }

        public DateTime DataInic { get; set; }
       
        public DateTime DataFim { get; set; }

        public int NumJogos { get; set; }

        //public bool Ativa { get; set; }

        public StatusRodada Status { get; set; }
        //public bool Status { get; set; }


        /* EF Relations */
        public Campeonato Campeonato { get; set; }
       
        public IEnumerable<Jogo> JogosRodada { get; set; }

        public IEnumerable<RankingRodada> RankingRodadas { get; set; }


    }
}