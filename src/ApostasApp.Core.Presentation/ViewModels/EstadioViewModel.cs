using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ApostasApp.Core.Presentation.ViewModels
{
    public class EstadioViewModel
    {
        [Key]
        public Guid Id { get; set; }

        [DisplayName("Nome do Estadio")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(60, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres", MinimumLength = 2)]
        public string Nome { get; set; }

        [DisplayName("UF")]
        public Guid UfId { get; set; }

        [DisplayName("UF")]
        public UfViewModel Uf { get; set; }

        public IEnumerable<UfViewModel> Ufs { get; set; }

        //public IEnumerable<JogoViewModel> Jogos { get; set; }      

    }
}