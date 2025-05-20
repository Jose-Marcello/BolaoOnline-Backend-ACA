using ApostasApp.Core.Domain.Models.Apostadores;
using ApostasApp.Core.Domain.Models.Apostas;
using ApostasApp.Core.Domain.Models.Base;
using DApostasApp.Core.Domain.Models.RankingRodadas;

namespace ApostasApp.Core.Domain.Models.Campeonatos{

    public class ApostadorCampeonato : Entity
    {
        public Guid CampeonatoId { get; set; }

        public Guid ApostadorId { get; set; }

        public Campeonato Campeonato { get; set; }
        public Apostador Apostador { get; set; }
        public int Pontuacao { get; set; }
        public int Posicao { get; set; }

        //public IEnumerable<Apostador> Apostadores { get; set; }

        public IEnumerable<Aposta> Apostas { get; set; }

        public IEnumerable<RankingRodada> RankingRodadas { get; set; }

    }
}
