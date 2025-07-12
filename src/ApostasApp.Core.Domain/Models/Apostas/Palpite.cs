// Localização: ApostasApp.Core.Domain.Models.Apostas/Palpite.cs
using ApostasApp.Core.Domain.Models.Base;
using ApostasApp.Core.Domain.Models.Jogos;
using ApostasApp.Core.Domain.Models.Apostas; // Para ApostaRodada
using System; // Adicionado para Guid

namespace ApostasApp.Core.Domain.Models.Apostas
{
    public class Palpite : Entity
    {
        // Construtor padrão (necessário para Entity Framework)
        public Palpite() { }

        // <<-- NOVO CONSTRUTOR -->>
        public Palpite(Guid apostaRodadaId, Guid jogoId)
        {
            ApostaRodadaId = apostaRodadaId;
            JogoId = jogoId;
            Pontos = 0; // Inicializa pontos como zero por padrão
        }

        public Guid JogoId { get; set; }
        public Guid ApostaRodadaId { get; set; } // FK para a ApostaRodada

        public int? PlacarApostaCasa { get; set; } // <<-- Nomes corretos das propriedades -->>
        public int? PlacarApostaVisita { get; set; } // <<-- Nomes corretos das propriedades -->>

        public int Pontos { get; set; }

        /* EF Relations */
        public Jogo Jogo { get; set; }
        public ApostaRodada ApostaRodada { get; set; } // Navegação para ApostaRodada
    }
}
