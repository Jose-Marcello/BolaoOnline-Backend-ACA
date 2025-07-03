// ApostasApp.Core.Domain.Models.Campeonatos/ApostadorCampeonato.cs
using ApostasApp.Core.Domain.Models.Apostadores;
using ApostasApp.Core.Domain.Models.Base; // Para a classe Entity
using ApostasApp.Core.Domain.Models.RankingRodadas;
using System;
using System.Collections.Generic;

namespace ApostasApp.Core.Domain.Models.Campeonatos
{
    public class ApostadorCampeonato : Entity // Herda de Entity, que deve ter o Guid Id
    {
        public ApostadorCampeonato()
        {
            // O construtor sem parâmetros é importante para o Entity Framework
            // para recriar entidades a partir do banco de dados.
            // Coleções devem ser inicializadas para evitar NullReferenceException.
            RankingRodadas = new List<RankingRodada>();
        }

        // NOVO Construtor para criar uma adesão
        public ApostadorCampeonato(Guid apostadorId, Guid campeonatoId) : this() // Chama o construtor padrão para inicializar coleções
        {
            // O 'Id' da entidade base (Entity) deve ser gerado aqui ou em Entity.
            // Ex: Id = Guid.NewGuid(); (se não for feito na base Entity)
            ApostadorId = apostadorId;
            CampeonatoId = campeonatoId;
            DataInscricao = DateTime.Now; // A data de inscrição é o momento da criação
            CustoAdesaoPago = false;      // Inicializa como falso; será true APENAS se o pagamento for bem-sucedido e persistido.
            Pontuacao = 0;                // Valor inicial para a pontuação
            Posicao = 0;                  // Valor inicial para a posição
        }

        public Guid CampeonatoId { get; set; }
        public Guid ApostadorId { get; set; }

        // Propriedades de navegação (relacionamento com outras entidades)
        public Campeonato Campeonato { get; set; }
        public Apostador Apostador { get; set; }

        public int Pontuacao { get; set; } // Pontuação total acumulada neste campeonato
        public int Posicao { get; set; }

        // Novas propriedades (campos NOT NULL na tabela)
        public DateTime DataInscricao { get; set; } // Data em que o apostador se inscreveu/aderiu ao campeonato
        public bool CustoAdesaoPago { get; set; } // Indica se o custo de adesão ao campeonato foi pago (true/false)

        public ICollection<RankingRodada> RankingRodadas { get; set; } // Relação para o ranking por rodada
    }
}
