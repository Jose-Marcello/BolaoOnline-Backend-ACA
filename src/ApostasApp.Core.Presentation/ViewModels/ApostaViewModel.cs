using ApostasApp.Core.Domain.Models.Apostas;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ApostasApp.Core.Presentation.ViewModels
{
    public class ApostaViewModel
    {
        [Key]
        public Guid Id { get; set; }

        [DisplayName("Jogo")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public Guid JogoId { get; set; }

        [DisplayName("Apostador")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public Guid ApostadorCampeonatoId { get; set; }

        [DisplayName("Data/Hora")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        //[Required(ErrorMessage = "O campo {0} é obrigatório")]        
        public DateTime DataHoraAposta { get; set; } //= DateTime.Now; //inicialmente nula

        [Range(0, 20)]
        public int PlacarApostaCasa { get; set; }


        [Range(0, 20)]
        public int PlacarApostaVisita { get; set; }

        public bool Enviada { get; set; } = false;

        public int Pontos { get; set; }

        //[ForeignKey("CampeonatoId")]
        public JogoViewModel Jogo { get; set; }

        //alterar para Apostador do Campeonato (para não trazer confusão)
        public ApostadorCampeonatoViewModel ApostadorCampeonato { get; set; }

        public List<Aposta> Apostas { get; set; }

    }
}