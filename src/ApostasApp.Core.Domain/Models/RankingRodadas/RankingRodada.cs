using ApostasApp.Core.Domain.Models.Apostadores;
using ApostasApp.Core.Domain.Models.Apostas;
using ApostasApp.Core.Domain.Models.Base;
using ApostasApp.Core.Domain.Models.Campeonatos;
using ApostasApp.Core.Domain.Models.Rodadas;
namespace ApostasApp.Core.Domain.Models.RankingRodadas
{
  public class RankingRodada : Entity
  {
    public Guid RodadaId { get; set; }

    // Novo campo essencial para individualizar cada aposta no ranking
    public Guid ApostaRodadaId { get; set; }

    // Alterado para Guid? (nullable) porque avulsas não têm esse vínculo
    public Guid? ApostadorCampeonatoId { get; set; }

    public Guid ApostadorId { get; set; }
    public int Pontuacao { get; set; }
    public int Posicao { get; set; }
    public DateTime DataAtualizacao { get; set; }

    // Propriedades de Navegação
    public Rodada Rodada { get; set; }
    public ApostaRodada ApostaRodada { get; set; } // Adicionar esta navegação
    public ApostadorCampeonato ApostadorCampeonato { get; set; }
    public Apostador Apostador { get; set; }
  }
}
