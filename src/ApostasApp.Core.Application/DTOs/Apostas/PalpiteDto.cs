using ApostasApp.Application.DTOs.Jogos;
using ApostasApp.Core.Application.DTOs.ApostadorCampeonatos; // Para ApostadorCampeonatoDto
using ApostasApp.Core.Application.DTOs.Jogos; // Para JogoDto
using System;

namespace ApostasApp.Core.Application.DTOs.Apostas
{
    /// <summary>
    /// DTO para representar um palpite/aposta.
    /// Contém informações básicas do palpite, do jogo associado e do apostador no campeonato.
    /// </summary>
    public class PalpiteDto
    {
        public Guid Id { get; set; }
        public Guid JogoId { get; set; }
        public JogoDto Jogo { get; set; } // DTO do Jogo (se for incluído pelo serviço)

        public Guid ApostadorCampeonatoId { get; set; }
        public ApostadorCampeonatoDto ApostadorCampeonato { get; set; } // DTO do Apostador no Campeonato (se for incluído pelo serviço)

        public int? PalpiteCasa { get; set; } // Placar do palpite para o time da casa
        public int? PalpiteVisitante { get; set; } // Placar do palpite para o time visitante
        public DateTime? DataHoraAposta { get; set; } // Data e hora em que o palpite foi feito
        public int PontuacaoPalpite { get; set; } // Pontuação obtida por este palpite (calculada)
    }
}
