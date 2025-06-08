using System;

namespace ApostasApp.Core.Application.DTOs.Equipes
{
    public class EquipeCampeonatoDto
    {
        public Guid Id { get; set; }
        public Guid EquipeId { get; set; }
        public Guid CampeonatoId { get; set; }

        public EquipeDto Equipe { get; set; }
    }
}
