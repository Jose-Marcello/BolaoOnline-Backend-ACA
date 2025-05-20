using ApostasApp.Core.Domain.Models.Apostas;
using ApostasApp.Core.Domain.Models.Base;
using ApostasApp.Core.Domain.Models.Campeonatos;
using ApostasApp.Core.Domain.Models.Estadios;
using ApostasApp.Core.Domain.Models.Rodadas;

namespace ApostasApp.Core.Domain.Models.Jogos
{     
    public class Jogo : Entity
    {

        public Guid RodadaId { get; set; }

        public DateTime DataJogo { get; set; }
        public TimeSpan HoraJogo { get; set; }

        public Guid EstadioId { get; set; }
        
        public Guid EquipeCasaId { get; set; }
        
        public Guid EquipeVisitanteId { get; set; }

        public int? PlacarCasa { get; set; } = null;
        public int? PlacarVisita { get; set; } = null;

        public StatusJogo Status { get; set; }

        //EF RELATIONS  

        public Rodada Rodada { get; set; }
        public Estadio Estadio { get; set; }

        public EquipeCampeonato EquipeCasa { get; set; }       
        public EquipeCampeonato EquipeVisitante { get; set; }

        public IEnumerable<Aposta> Apostas { get; set; }

    }
}