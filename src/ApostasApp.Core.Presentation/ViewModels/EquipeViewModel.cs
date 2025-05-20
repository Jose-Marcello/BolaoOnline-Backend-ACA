using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApostasApp.Core.Presentation.ViewModels
{
    public class EquipeViewModel
    {
        [Key]
        public Guid Id { get; set; }

        [DisplayName("Equipe")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(40, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres", MinimumLength = 2)]
        public string Nome { get; set; }

        [DisplayName("Sigla")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(3, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres", MinimumLength = 3)]
        public string Sigla { get; set; }


        [NotMapped]
        //[DisplayName("Escudo da Equipe")]
        public IFormFile EscudoUpload { get; set; }

        public string Escudo { get; set; }

        public string Tipo { get; set; }

        [DisplayName("UF")]
        public Guid UfId { get; set; }

        public UfViewModel Uf { get; set; }

        public IEnumerable<UfViewModel> Ufs { get; set; }

        public IEnumerable<JogoViewModel> Jogos { get; set; }      

    }
}