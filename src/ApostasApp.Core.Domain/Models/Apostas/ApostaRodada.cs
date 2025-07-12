// Localização: ApostasApp.Core.Domain.Models.Apostas/ApostaRodada.cs
using ApostasApp.Core.Domain.Models.Apostadores; // Para Apostador
using ApostasApp.Core.Domain.Models.Base; // Assumindo que Entity está aqui
using ApostasApp.Core.Domain.Models.Rodadas;
using ApostasApp.Core.Domain.Models.Campeonatos; // <<-- ADICIONADO: Para ApostadorCampeonato
using System.ComponentModel;
using System; // Adicionado para DateTime
using System.Collections.Generic; // Adicionado para List

namespace ApostasApp.Core.Domain.Models.Apostas
{
    public class ApostaRodada : Entity
    {
        // Construtor padrão (necessário para Entity Framework)
        public ApostaRodada()
        {
            Palpites = new List<Palpite>();
        }

        // <<-- NOVO CONSTRUTOR -->>
        public ApostaRodada(Guid apostadorCampeonatoId, Guid rodadaId) : this() // Chama o construtor padrão para inicializar Palpites
        {
            ApostadorCampeonatoId = apostadorCampeonatoId;
            RodadaId = rodadaId;
            DataCriacao = DateTime.Now; // Inicializa DataCriacao aqui
            Enviada = false; // Garante que a aposta não está enviada por padrão
        }

        // Chaves Estrangeiras para as entidades relacionadas
        public Guid ApostadorCampeonatoId { get; set; } // Quem fez esta submissão de palpites
        public Guid RodadaId { get; set; }    // Para qual rodada esta submissão de palpites é

        [DisplayName("Identificador da Aposta")]
        public string IdentificadorAposta { get; set; } // Ex: "Aposta #1", "Aposta Premiada" etc.

        // Informações da Submissão
        public DateTime DataCriacao { get; set; } // <<-- ADICIONADO: Propriedade DataCriacao -->>
        public DateTime? DataHoraSubmissao { get; set; } // Data e hora em que esta "cesta" de palpites foi enviada/registrada
        public bool EhApostaCampeonato { get; set; }    // Indica se esta submissão de palpites conta para o Campeonato (true/false)
        public bool EhApostaIsolada { get; set; }        // Indica se esta submissão de palpites é uma aposta avulsa/isolada (true/false)
        public decimal? CustoPagoApostaRodada { get; set; } // O custo *efetivamente pago* por esta ApostaRodada (se for isolada e tiver custo)
        public int PontuacaoTotalRodada { get; set; }    // A pontuação total que o apostador fez nesta ApostaRodada (soma dos palpites)
        public bool Enviada { get; set; }                // Indica se a submissão de palpites foi finalizada/enviada

        /* EF Relations */
        public ApostadorCampeonato ApostadorCampeonato { get; set; } // <<-- CORRIGIDO: Tipo para ApostadorCampeonato
        public Rodada Rodada { get; set; }        // Navegação para a Rodada

        // Coleção de Palpites individuais
        public IEnumerable<Palpite> Palpites { get; set; }
    }
}
