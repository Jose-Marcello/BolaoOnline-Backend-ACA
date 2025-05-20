using ApostasApp.Core.Presentation.ViewModels;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ApostasApp.Core.Presentation.ViewModels
{
    public class RankingRodadaViewModel
    {
        [Key]
        public Guid Id { get; set; }

        [DisplayName("Apostador")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public Guid ApostadorCampeonatoId { get; set; }

        [DisplayName("Rodada No")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]

        public Guid RodadaId { get; set; }

        [DisplayName("Data Atualização")]

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        //[DateLessThan("DataFim", ErrorMessage = "A Data Inicial tem que ser MENOR do que a DATA FINAL !!")]

        public DateTime DataAtualizacao { get; set; } = DateTime.Today;

        [DisplayName("Posição")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        //[Range(1, 10)]  //isso tem que melhorar
        public int Posicao { get; set; }

        [DisplayName("Pontuação")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        //[Range(1, 10)]  //isso tem que melhorar
        public int Pontuacao { get; set; }


        //[ForeignKey("CampeonatoId")]
        public ApostadorCampeonatoViewModel ApostadorCampeonato { get; set; }

        public RodadaViewModel Rodada { get; set; }

        //public IEnumerable<CampeonatoViewModel> Campeonatos { get; set; }      

    }
}