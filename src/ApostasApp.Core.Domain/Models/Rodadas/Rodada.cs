using ApostasApp.Core.Domain.Models.Apostas;
using ApostasApp.Core.Domain.Models.Base;
using ApostasApp.Core.Domain.Models.Campeonatos;
using ApostasApp.Core.Domain.Models.Jogos;
using ApostasApp.Core.Domain.Models.RankingRodadas;

namespace ApostasApp.Core.Domain.Models.Rodadas
{

    public class Rodada : Entity
    {
        public Rodada()
        {
            JogosRodada = new List<Jogo>();
            RankingRodadas = new List<RankingRodada>();
            ApostasRodada = new List<ApostaRodada>();
        }

        public Guid CampeonatoId { get; set; }
        public int NumeroRodada { get; set; }
        public DateTime DataInic { get; set; }
        public DateTime DataFim { get; set; }
        public int NumJogos { get; set; }
        public StatusRodada Status { get; set; } // Uso de enum para status é uma ótima prática!

        // Nova propriedade: Custo da Aposta Avulsa nesta Rodada
        public decimal? CustoApostaRodada { get; set; } // Usando 'decimal?' para permitir rodadas gratuitas

        /* EF Relations */
        public Campeonato Campeonato { get; set; }
        public ICollection<Jogo> JogosRodada { get; set; }
        public ICollection<RankingRodada> RankingRodadas { get; set; }
              
        //public IEnumerable<ApostaRodada> ApostasRodada { get; set; } // Coleção de ApostaRodada para esta rodada
        public ICollection<ApostaRodada> ApostasRodada { get; set; }
    }
}