using ApostasApp.Core.Domain.Models.Campeonatos;
using ApostasApp.Core.Presentation.Extensions;
using ApostasApp.Core.Presentation.ViewModels;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ApostasApp.Core.Presentation.ViewModels
{
    public class CampeonatoViewModel
    {
        [Key]
        public Guid Id { get; set; }

        [DisplayName("Nome do Campeonato")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(100, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres", MinimumLength = 2)]
        public string Nome { get; set; }

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


        [DisplayName("Número de Rodadas")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [Range(0, 100)]
        public int NumRodadas { get; set; }

        [DisplayName("Tipo de Campeonato")]
        public TiposCampeonato Tipo { get; set; }

        [DisplayName("Ativo?")]
        public bool Ativo { get; set; }

        public IEnumerable<RodadaViewModel> Rodadas { get; set; }

        //para testes com lógica MASTERDETAIL

        /*public List<Campeonato> Campeonatos { get; set; }
        
        public Campeonato SelectedCampeonato { get; set; }
        public EquipeCampeonato SelectedEquipe { get; set; }

        public EntradaDados EntradaDados { get; set; }
        public ModoExibicao ModoExibicao { get; set; }*/

    }
}
