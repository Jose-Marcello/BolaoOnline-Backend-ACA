using System;

namespace ApostasApp.Core.Application.DTOs.Equipes
{
    public class EquipeCampeonatoDto
    {
        public string Id { get; set; }        // ID da associação, já está como string (correto)
        public string EquipeId { get; set; }    // << Mudar de Guid para string
        public string CampeonatoId { get; set; } // << Mudar de Guid para string

        public EquipeDto Equipe { get; set; }
    }
}