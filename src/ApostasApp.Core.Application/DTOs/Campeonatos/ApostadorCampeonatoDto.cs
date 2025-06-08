using ApostasApp.Core.Application.DTOs.Apostadores; // Para ApostadorDto
using ApostasApp.Core.Application.DTOs.Campeonatos; // Para CampeonatoDto
using System;

namespace ApostasApp.Core.Application.DTOs.ApostadorCampeonatos
{
    /// <summary>
    /// DTO para representar a associação entre um Apostador e um Campeonato.
    /// Inclui informações básicas do apostador e do campeonato, além de detalhes da relação.
    /// </summary>
    public class ApostadorCampeonatoDto
    {
        public Guid Id { get; set; } // ID da entidade ApostadorCampeonato
        public DateTime DataInscricao { get; set; }
        public int PontuacaoAtual { get; set; }

        public ApostadorDto Apostador { get; set; } // DTO do Apostador associado
        public CampeonatoDto Campeonato { get; set; } // DTO do Campeonato associado
    }
}
