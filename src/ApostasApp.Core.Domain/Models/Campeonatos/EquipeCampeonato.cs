using ApostasApp.Core.Domain.Models.Base;
using ApostasApp.Core.Domain.Models.Equipes;
using ApostasApp.Core.Domain.Models.Jogos;
using ApostasApp.Core.Domain.Models.Rodadas;


namespace ApostasApp.Core.Domain.Models.Campeonatos
{
    //Será uma entity, mas a chave primária não será o Guid Id e sim
    //uma chave composta pelos Id de Equipe+campeonato (ver o mapping)
    public class EquipeCampeonato : Entity
    {
        public EquipeCampeonato()
        {
            JogosCasa = new List<Jogo>();
            JogosVisitante = new List<Jogo>();             
        }

        //não vai ser mais :chave primária composta : EquipeId,CampeonatoId
        //tem que ter um mecanismo (index) para garantir a unicidade
        //a chave será o Guid Id da entidade de junção

        public Guid CampeonatoId { get; set; }
        //[Index("IUQ_CampeonatoEquipe_CampeonatoId_EquipeId", IsUnique = true)]

        public Guid EquipeId { get; set; }

        public Campeonato Campeonato { get; set; }
        public Equipe Equipe { get; set; }

        public ICollection<Jogo> JogosCasa { get; set; }
        public ICollection<Jogo> JogosVisitante { get; set; }




    }
}
