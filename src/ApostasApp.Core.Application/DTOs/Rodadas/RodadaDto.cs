using ApostasApp.Core.Application.DTOs.Campeonatos;
using System;

namespace ApostasApp.Core.Application.DTOs.Rodadas
{
    public class RodadaDto
    {
        public Guid Id { get; set; }
        public int NumeroRodada { get; set; }
        public int NumJogos { get; set; }
        public string Status { get; set; }

        public CampeonatoDto Campeonato { get; set; }
    }
}