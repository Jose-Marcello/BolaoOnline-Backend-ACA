using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using Microsoft.AspNetCore.Http;
using ApostasApp.Core.Domain.Models.Ufs;
using ApostasApp.Core.Domain.Models.Base;
using ApostasApp.Core.Domain.Models.Campeonatos;

 
namespace ApostasApp.Core.Domain.Models.Equipes

{     
    public class Equipe : Entity
    {       
        public string Nome { get; set; }
        public string Sigla { get; set; }

        //sair daqui - ir p/ viewModel ?
        [NotMapped]
        [DisplayName("Imagem do Escudo")]
        public IFormFile EscudoUpload { get; set; }
       
        public string Escudo { get; set; }
       
        public TiposEquipe Tipo { get; set; }

        public Guid UfId { get; set; }

        /* EF Relations */
        public Uf Uf { get; set; }
        
        public IEnumerable<EquipeCampeonato> EquipesCampeonatos { get; set; }          



    }
}