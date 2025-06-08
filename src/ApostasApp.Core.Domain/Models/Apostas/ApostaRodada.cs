using ApostasApp.Core.Domain.Models.Apostadores; // Para Apostador
using ApostasApp.Core.Domain.Models.Base; // Assumindo que Entity está aqui
using ApostasApp.Core.Domain.Models.Rodadas;
using System.ComponentModel;     // Para Rodada
//using ApostasApp.Core.Domain.Models.Palpites; // Será adicionado depois, quando 'Aposta' virar 'Palpite'

namespace ApostasApp.Core.Domain.Models.Apostas // Ou um novo namespace, ex: ApostasApp.Core.Domain.Models.ApostasRodada
{
    public class ApostaRodada : Entity
    {
        public ApostaRodada()
        {
            Palpites = new List<Palpite>();
        }

        // Chaves Estrangeiras para as entidades relacionadas
        public Guid ApostadorCampeonatoId { get; set; } // Quem fez esta submissão de palpites
        public Guid RodadaId { get; set; }    // Para qual rodada esta submissão de palpites é

        [DisplayName("Identificador da Aposta")]
        public string IdentificadorAposta { get; set; } // Ex: "Aposta #1", "Aposta Premiada" etc.
                                                        // Pode ser gerado automaticamente (sequencial) ou permitir input do usuário.

        // Informações da Submissão
        public DateTime? DataHoraSubmissao { get; set; } // Data e hora em que esta "cesta" de palpites foi enviada/registrada
        public bool EhApostaCampeonato { get; set; }    // Indica se esta submissão de palpites conta para o Campeonato (true/false)
        public bool EhApostaIsolada { get; set; }       // Indica se esta submissão de palpites é uma aposta avulsa/isolada (true/false)
        public decimal? CustoPagoApostaRodada { get; set; } // O custo *efetivamente pago* por esta ApostaRodada (se for isolada e tiver custo)
        public int PontuacaoTotalRodada { get; set; }   // A pontuação total que o apostador fez nesta ApostaRodada (soma dos palpites)
        public bool Enviada { get; set; }               // Indica se a submissão de palpites foi finalizada/enviada

        /* EF Relations */
        public Apostador ApostadorCampeonato { get; set; } // Navegação para o Apostador
        public Rodada Rodada { get; set; }       // Navegação para a Rodada

        // Coleção de Palpites individuais (a antiga "Aposta" renomeada)
        public IEnumerable<Palpite> Palpites { get; set; } // Será adicionado depois que Aposta virar Palpite
    }
}