// ApostasApp.Core.Application.DTOs.Apostas/SalvarApostaRequestDto.cs
// DTO para receber os dados de uma aposta a ser salva/atualizada
using System;
using System.Collections.Generic;

namespace ApostasApp.Core.Application.DTOs.Apostas
{
    public class SalvarApostaRequestDto
    {
        public Guid ApostadorCampeonatoId { get; set; }
        public Guid RodadaId { get; set; }
        public List<PalpiteRequestDto> Palpites { get; set; } // Coleção de palpites para esta aposta
        public bool EhApostaCampeonato { get; set; }
        public bool EhApostaIsolada { get; set; }
        // Se precisar de IdentificadorAposta ou CustoPagoApostaRodada aqui, adicione.
    }

    // ApostasApp.Core.Application.DTOs.Apostas/PalpiteRequestDto.cs
    // DTO para representar um palpite individual dentro da requisição de salvar aposta
    public class PalpiteRequestDto
    {
        public Guid IdPalpite { get; set; } // ID do Palpite existente (para edição), Guid.Empty para novo
        public Guid IdJogo { get; set; }
        public int? PlacarApostaCasa { get; set; }
        public int? PlacarApostaVisita { get; set; }
    }
}
