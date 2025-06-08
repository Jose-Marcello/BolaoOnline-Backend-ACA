using System;

namespace ApostasApp.Application.DTOs.Jogos
{
    /// <summary>
    /// DTO para transferir dados de Jogo da camada de Aplicação para a camada de Apresentação.
    /// </summary>
    public class JogoDto
    {
        public Guid Id { get; set; }
        public Guid RodadaId { get; set; }
        public Guid EquipeCasaId { get; set; }
        public string EquipeCasaNome { get; set; } // Adicionado para evitar carregar a entidade completa
        public string EquipeCasaLogoUrl { get; set; } // Exemplo
        public Guid EquipeForaId { get; set; }
        public string EquipeForaNome { get; set; } // Adicionado
        public string EquipeForaLogoUrl { get; set; } // Exemplo
        public Guid EstadioId { get; set; }
        public string EstadioNome { get; set; } // Adicionado
        public DateTime DataHora { get; set; }
        public string Status { get; set; } // Ex: "Agendado", "Em Andamento", "Finalizado"
        public int? PlacarCasa { get; set; }
        public int? PlacarFora { get; set; }
    }
}
