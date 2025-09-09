// ApostasApp.Core.Application.DTOs/ApostasRodada/CriarOuAtualizarApostaRodadaRequestDto.cs
using ApostasApp.Core.Application.DTOs.Apostas.ApostasApp.Core.Application.DTOs.Apostas;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApostasApp.Core.Application.DTOs.ApostasRodada
{
    public class CriarOuAtualizarApostaRodadaRequestDto
    {
        // Se for uma atualização, o ID da ApostaRodada existente
        public string? ApostaRodadaId { get; set; } // Ajustado para string?

        [Required(ErrorMessage = "O ID do apostador no campeonato é obrigatório.")]
        public string ApostadorCampeonatoId { get; set; } // Ajustado para string

        [Required(ErrorMessage = "O ID da rodada é obrigatório.")]
        public string RodadaId { get; set; } // Ajustado para string

        // Lista de palpites para os jogos da rodada
        [Required(ErrorMessage = "Pelo menos um palpite é obrigatório.")]
        public List<ApostaJogoRequestDto> Palpites { get; set; } = new List<ApostaJogoRequestDto>();

        // Opcional: Para identificar a aposta (se o usuário puder nomear)
        public string IdentificadorAposta { get; set; }

        // Opcional: Se a aposta for enviada/finalizada neste momento
        public bool Enviada { get; set; } = false;
    }
}
