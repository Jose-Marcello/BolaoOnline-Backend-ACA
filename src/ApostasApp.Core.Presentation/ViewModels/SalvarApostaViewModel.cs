using System.ComponentModel.DataAnnotations;

namespace ApostasApp.Core.PresentationViewModels
{
    public class SalvarApostaViewModel
    {
        [Key]
        public Guid Id { get; set; }

        [Range(0, 20)]
        public int PlacarApostaCasa { get; set; }

        [Range(0, 20)]
        public int PlacarApostaVisita { get; set; }


    }
}