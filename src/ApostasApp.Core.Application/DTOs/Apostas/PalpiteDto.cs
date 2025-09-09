// ApostasApp.Core.Application.DTOs/Palpites/PalpiteDto.cs
using ApostasApp.Core.Application.DTOs.Jogos; // Para JogoDto
using System;

namespace ApostasApp.Core.Application.DTOs.Palpites // <<-- Namespace recomendado -->>
{
    /// <summary>
    /// DTO para representar um palpite individual para um jogo.
    /// </summary>
    public class PalpiteDto
    {
        public string Id { get; set; } // Ajustado para string
        public string JogoId { get; set; } // Ajustado para string
        public string ApostaRodadaId { get; set; } // <<-- Adicionado: FK para ApostaRodada -->>

        public int? PlacarApostaCasa { get; set; } // <<-- Nomenclatura ajustada -->>
        public int? PlacarApostaVisita { get; set; } // <<-- Nomenclatura ajustada -->>
        public int Pontos { get; set; } // <<-- Nomenclatura ajustada -->>

        // DTO aninhado para exibir detalhes do jogo
        public JogoDto Jogo { get; set; } // Referencia o JogoDto atualizado
    }
}
