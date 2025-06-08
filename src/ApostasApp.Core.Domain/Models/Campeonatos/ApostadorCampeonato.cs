using ApostasApp.Core.Domain.Models.Apostadores;
using ApostasApp.Core.Domain.Models.Apostas;

// using ApostasApp.Core.Domain.Models.Apostas; // Não precisamos mais dessa importação para 'Aposta' aqui
using ApostasApp.Core.Domain.Models.Base;
using ApostasApp.Core.Domain.Models.RankingRodadas; // Assumindo que o namespace correto é este

namespace ApostasApp.Core.Domain.Models.Campeonatos
{
    public class ApostadorCampeonato : Entity
    {
        public ApostadorCampeonato()
        {
            RankingRodadas = new List<RankingRodada>();
        }

        // NOVO Construtor para criar uma adesão
        public ApostadorCampeonato(Guid apostadorId, Guid campeonatoId) : this() // Chama o construtor padrão para inicializar coleções
        {
            ApostadorId = apostadorId;
            CampeonatoId = campeonatoId;
            DataInscricao = DateTime.Now; // A data de inscrição é o momento da criação
            CustoAdesaoPago = false;      // Inicializa como falso; será true APENAS se o pagamento for bem-sucedido e persistido.
            Pontuacao = 0;                // Valor inicial para a pontuação
            Posicao = 0;                  // Valor inicial para a posição
        }

        public Guid CampeonatoId { get; set; }
        public Guid ApostadorId { get; set; }

        public Campeonato Campeonato { get; set; }
        public Apostador Apostador { get; set; }

        public int Pontuacao { get; set; } // Pontuação total acumulada neste campeonato
        public int Posicao { get; set; }

        // Novas propriedades
        public DateTime DataInscricao { get; set; } // Data em que o apostador se inscreveu/aderiu ao campeonato
        public bool CustoAdesaoPago { get; set; } // Indica se o custo de adesão ao campeonato foi pago (true/false)

        /* EF Relations */
        // public IEnumerable<Aposta> Apostas { get; set; } // <-- REMOVER ESTA LINHA!
        public ICollection<RankingRodada> RankingRodadas { get; set; } // Relação para o ranking por rodada
    }
}