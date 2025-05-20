using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ApostasApp.Core.Presentation.ViewModels
{
    public class UfViewModel
    {
        [Key]
        public Guid Id { get; set; }

        [DisplayName("Nome da UF")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string Nome { get; set; }

        [DisplayName("Sigla da UF")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string Sigla { get; set; }

        public IEnumerable<EquipeViewModel> Equipes { get; set; }
        public IEnumerable<EstadioViewModel> Estadios { get; set; }

    }
}