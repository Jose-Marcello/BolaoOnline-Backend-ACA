using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ApostasApp.Core.Presentation.ViewModels
{
    public class ApostadorCampeonatoViewModel
    {
        [Key]
        public Guid Id { get; set; }

        [DisplayName("Campeonato")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public Guid CampeonatoId { get; set; }

        [DisplayName("Apostador")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public Guid ApostadorId { get; set; }

        public CampeonatoViewModel Campeonato { get; set; }
        public IEnumerable<CampeonatoViewModel> Campeonatos { get; set; }
        public ApostadorViewModel Apostador { get; set; }

        public IEnumerable<ApostadorViewModel> Apostadores { get; set; }


    }
}