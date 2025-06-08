using System;
using System.ComponentModel.DataAnnotations; // Para validações básicas

namespace ApostasApp.Core.Application.DTOs.Campeonatos
{
    public class VincularApostadorCampeonatoDTO
    {
        [Required(ErrorMessage = "O ID do apostador é obrigatório.")]
        public Guid ApostadorId { get; set; } // Agora usando ApostadorId

        [Required(ErrorMessage = "O ID do campeonato é obrigatório.")]
        public Guid CampeonatoId { get; set; }
    }
}