using ApostasApp.Core.Domain.Models.Base;
using ApostasApp.Core.Domain.Models.Campeonatos;
using ApostasApp.Core.Domain.Models.Rodadas;
using ApostasApp.Core.Domain.Models.Ufs;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;


 
namespace ApostasApp.Core.Domain.Models.Equipes

{     
    public class Equipe : Entity
    {
        public Equipe()
        {
            EquipesCampeonatos = new List<EquipeCampeonato>();
        }
     
        public string Nome { get; set; }
        public string Sigla { get; set; }

        //sair daqui - ir p/ viewModel ?
        [NotMapped]
        [DisplayName("Imagem do Escudo")]
        //public IFormFile EscudoUpload { get; set; }
       
        public string Escudo { get; set; }
       
        public TiposEquipe Tipo { get; set; }

        public Guid UfId { get; set; }

        /* EF Relations */
        public Uf Uf { get; set; }
        
        public ICollection<EquipeCampeonato> EquipesCampeonatos { get; set; }          



    }
}