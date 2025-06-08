using ApostasApp.Core.Domain.Models.Base;
using ApostasApp.Core.Domain.Models.RankingRodadas;
using ApostasApp.Core.Domain.Models.Rodadas;

namespace ApostasApp.Core.Domain.Models.Campeonatos
{
    public class Campeonato : Entity
    {
        public Campeonato()
        {             
           EquipesCampeonatos = new List<EquipeCampeonato>();
           ApostadoresCampeonatos = new List<ApostadorCampeonato>();
           Rodadas = new List<Rodada>();
        }

        public string Nome { get; set; }
        public DateTime DataInic { get; set; }
        public DateTime DataFim { get; set; }
        public int NumRodadas { get; set; }
        public TiposCampeonato Tipo { get; set; }
        public bool Ativo { get; set; }

        // Nova propriedade: Custo de Adesão ao Campeonato
        public decimal? CustoAdesao { get; set; } // Ou 'decimal? CustoAdesao' se for opcional e puder ser nulo

        /* EF Relations */
        public ICollection<EquipeCampeonato> EquipesCampeonatos { get; set; }
        public ICollection<ApostadorCampeonato> ApostadoresCampeonatos { get; set; }
        public ICollection<Rodada> Rodadas { get; set; }
    }
}