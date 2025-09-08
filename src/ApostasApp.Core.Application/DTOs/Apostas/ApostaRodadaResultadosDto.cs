// Arquivo: ApostasApp.Core.Application.DTOs/Apostas/ApostaRodadaResultadosDto.cs

using System;
using System.Collections.Generic;

namespace ApostasApp.Core.Application.DTOs.Apostas
{
    public class ApostaRodadaResultadosDto
    {
        public string ApostaRodadaId { get; set; }
        public int PontuacaoTotalRodada { get; set; }
        public IEnumerable<ApostaJogoResultadosDto> JogosComResultados { get; set; }
    }
}