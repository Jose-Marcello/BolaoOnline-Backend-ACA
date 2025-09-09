// ApostasApp.Core.Application.DTOs/ApostadorCampeonatos/ApostadorCampeonatoDto.cs
using ApostasApp.Core.Application.DTOs.Apostadores; // Para ApostadorDto
using ApostasApp.Core.Application.DTOs.Campeonatos; // Para CampeonatoDto
using System;

namespace ApostasApp.Core.Application.DTOs.ApostadorCampeonatos
{
    public class ApostadorCampeonatoDto
    {
        public string Id { get; set; } // ID da entidade de junção
        public string ApostadorId { get; set; }
        public string CampeonatoId { get; set; }

        // DTOs aninhados para exibir nome do apostador e do campeonato
        public ApostadorDto Apostador { get; set; }
        public CampeonatoDto Campeonato { get; set; }

        public DateTime DataInscricao { get; set; }
        public decimal SaldoAtual { get; set; } // Saldo do apostador dentro deste campeonato
        // Outras propriedades relevantes da entidade ApostadorCampeonato
    }
}