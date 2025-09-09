using System;

namespace ApostasApp.Core.Application.DTOs.Equipes
{
    /// <summary>
    /// DTO para representar informações básicas de uma Equipe, incluindo escudo e sigla.
    /// </summary>
    public class EquipeDto
    {
        public string Id { get; set; }
        public string Nome { get; set; }
        public string Tipo { get; set; }
        public string Escudo { get; set; } // Caminho ou URL da imagem do escudo da equipe
        public string Sigla { get; set; }  // Sigla da equipe (ex: FLA, COR, SAO)

        public int UfId { get; set; } // Mantido para referência a UF (Estado)




    }
}
