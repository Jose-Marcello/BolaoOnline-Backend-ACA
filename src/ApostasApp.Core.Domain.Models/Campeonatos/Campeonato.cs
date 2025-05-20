using ApostasApp.Core.Domain.Models.Base;
using ApostasApp.Core.Domain.Models.Rodadas;

namespace ApostasApp.Core.Domain.Models.Campeonatos
{
    public class Campeonato : Entity
    {
        public string Nome { get; set; }

        public DateTime DataInic { get; set; }

        public DateTime DataFim { get; set; }

        public int NumRodadas { get; set; }

        public TiposCampeonato Tipo { get; set; }
        public bool Ativo { get; set; }

        // Nova propriedade para armazenar o Guid como string(retirada)
        //public string IdGuidString { get; set; }

        /* EF Relations */
        public IEnumerable<EquipeCampeonato> EquipesCampeonatos { get; set; }
        public IEnumerable<ApostadorCampeonato> ApostadoresCampeonatos { get; set; }
        //public object Rodadas { get; set; }
        public IEnumerable<Rodada> Rodadas { get; set; }

    }
}