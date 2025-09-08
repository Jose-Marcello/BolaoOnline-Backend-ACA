using System;
using System.ComponentModel.DataAnnotations;

namespace ApostasApp.Core.Application.DTOs.ApostasRodada
{
    public class CriarApostaAvulsaRequestDto
    {
        [Required(ErrorMessage = "O ID do apostador é obrigatório.")]
        public string ApostadorId { get; set; }

        [Required(ErrorMessage = "O ID do campeonato é obrigatório.")]
        public string CampeonatoId { get; set; }

        [Required(ErrorMessage = "O ID da rodada é obrigatório.")]
        public string RodadaId { get; set; }

        [Required(ErrorMessage = "O Custo da Aposta é obrigatório.")]
        public Decimal CustoAposta { get; set; }


    }
}