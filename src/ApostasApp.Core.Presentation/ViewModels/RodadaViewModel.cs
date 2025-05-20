using ApostasApp.Core.Domain.Models.Rodadas;
using ApostasApp.Core.Presentation.Extensions;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ApostasApp.Core.Presentation.ViewModels
{
    public class RodadaViewModel
    {
        [Key]
        public Guid Id { get; set; }

        [DisplayName("Campeonato")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public Guid CampeonatoId { get; set; }

        [DisplayName("Rodada No")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [Range(1, 38)] //isso tem que melhorar
        public string NumeroRodada { get; set; }

        [DisplayName("Data Inicial")]

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [DateLessThan("DataFim", ErrorMessage = "A Data Inicial tem que ser MENOR do que a DATA FINAL !!")]

        public DateTime DataInic { get; set; } = DateTime.Today;

        [DisplayName("Data Final")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        //[DateGreaterThan("DataInicial")]
        public DateTime DataFim { get; set; } = DateTime.Today;

        [DisplayName("Número de Jogos")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [Range(1, 10)]  //isso tem que melhorar
        public int NumJogos { get; set; }
        [DisplayName("Ativa?")]
        public bool Ativa { get; set; }

        [DisplayName("Status da Rodada")]
        public StatusRodada Status { get; set; }

        //[ForeignKey("CampeonatoId")]
        public CampeonatoViewModel Campeonato { get; set; }

        public IEnumerable<CampeonatoViewModel> Campeonatos { get; set; }

    }
}