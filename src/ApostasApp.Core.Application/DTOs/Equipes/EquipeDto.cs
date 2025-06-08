using System;

namespace ApostasApp.Core.Application.DTOs.Equipes
{
    /// <summary>
    /// DTO para representar informações básicas de uma Equipe, incluindo escudo e sigla.
    /// </summary>
    public class EquipeDto
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Apelido { get; set; }
        public string Escudo { get; set; } // Caminho ou URL da imagem do escudo da equipe
        public string Sigla { get; set; }  // Sigla da equipe (ex: FLA, COR, SAO)
    }
}
