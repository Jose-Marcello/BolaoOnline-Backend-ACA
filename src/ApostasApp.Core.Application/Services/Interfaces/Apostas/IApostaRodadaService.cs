using ApostasApp.Core.Application.DTOs.Apostas;

namespace ApostasApp.Core.Application.Services.Interfaces.Apostas
{
    public interface IApostaRodadaService
    {
       
        Task<ApostaRodadaStatusDto> ObterStatusApostaRodadaParaUsuario(Guid rodadaId, Guid apostadorCampeonatoId);
              
        Task<IEnumerable<ApostaJogoDto>> ObterApostasDoApostadorNaRodadaParaEdicao(Guid rodadaId, Guid apostadorCampeonatoId);

                Task<IEnumerable<ApostaJogoVisualizacaoDto>> ObterApostasDoApostadorNaRodadaParaVisualizacao(Guid rodadaId, Guid apostadorCampeonatoId);

        Task<bool> SalvarApostas(SalvarApostaRequestDto salvarApostaDto);
    }
}
