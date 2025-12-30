using ApostasApp.Core.Application.DTOs.Jogos;


namespace ApostasApp.Core.Application.Interfaces
{
  public interface IApostaRodadaAppService
  {
    // Esta interface enxerga o DTO porque ambos estão na Application!
    Task<IEnumerable<JogoPalpiteDto>> ObterJogosComPalpites(Guid apostaId, Guid rodadaId);

    // Outros métodos de aplicação...
  }

}
