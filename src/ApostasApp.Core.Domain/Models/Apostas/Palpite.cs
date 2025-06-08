using ApostasApp.Core.Domain.Models.Base;
// using ApostasApp.Core.Domain.Models.Campeonatos; // <-- Não precisa mais desta
using ApostasApp.Core.Domain.Models.Jogos;
using ApostasApp.Core.Domain.Models.Apostas; // Para ApostaRodada

namespace ApostasApp.Core.Domain.Models.Apostas // Ou ApostasApp.Core.Domain.Models.Palpites se você mover
{
    public class Palpite : Entity
    {
        public Guid JogoId { get; set; }

        public Guid ApostaRodadaId { get; set; } // FK para a ApostaRodada

        //public DateTime? DataHoraAposta { get; set; }
        public int? PlacarApostaCasa { get; set; }
        public int? PlacarApostaVisita { get; set; }

        // 'Enviada' foi movida para ApostaRodada

        public int Pontos { get; set; }

        /* EF Relations */
        public Jogo Jogo { get; set; }
        public ApostaRodada ApostaRodada { get; set; } // Navegação para ApostaRodada
    }
}