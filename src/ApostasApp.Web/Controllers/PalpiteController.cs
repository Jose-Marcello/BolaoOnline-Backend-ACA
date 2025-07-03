using ApostasApp.Core.Application.Services.Interfaces.Campeonatos;
using ApostasApp.Core.Application.Services.Interfaces.Jogos;
using ApostasApp.Core.Application.Services.Interfaces.RankingRodadas;
using ApostasApp.Core.Application.Services.Interfaces.Rodadas;
using ApostasApp.Core.Application.Services.Interfaces.Palpites; // Para IPalpiteService
using ApostasApp.Core.Domain.Interfaces.Notificacoes;
using ApostasApp.Core.Application.DTOs.Apostas; // Para PalpiteDto, SalvarPalpiteRequestDto
using ApostasApp.Core.Application.Models; // <<-- NOVO: Para ApiResponse -->>
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ApostasApp.Core.Domain.Interfaces; // Para IUnitOfWork
using System; // Para Guid
using System.Collections.Generic;
using System.Linq; // Para Linq
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization; // Para [Authorize]

namespace ApostasApp.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Adicionado para proteger os endpoints por padrão
    public class PalpiteController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IApostadorCampeonatoService _apostadorCampeonatoService;
        private readonly IRodadaService _rodadaService;
        private readonly IRankingRodadaService _rankingRodadaService;
        private readonly IJogoService _jogoService;
        private readonly IPalpiteService _palpiteService;

        public PalpiteController(IMapper mapper,
                                 IApostadorCampeonatoService apostadorCampeonatoService,
                                 IRankingRodadaService rankingRodadaService,
                                 IJogoService jogoService,
                                 IRodadaService rodadaService,
                                 IPalpiteService palpiteService,
                                 INotificador notificador,
                                 IUnitOfWork uow) : base(notificador, uow)
        {
            _mapper = mapper;
            _apostadorCampeonatoService = apostadorCampeonatoService;
            _rankingRodadaService = rankingRodadaService;
            _jogoService = jogoService;
            _rodadaService = rodadaService;
            _palpiteService = palpiteService;
        }

        // ========================================================================================================
        // O método GerarApostasERanking permanece comentado/removido conforme discutido.
        // ========================================================================================================
        /*
        [HttpPost("gerar-apostas-da-rodada/{id:guid}")]
        public async Task<IActionResult> GerarApostasERanking(Guid Id)
        {
            return StatusCode(501, "Esta funcionalidade é administrativa e deve ser implementada em BolãoAdm.");
        }
        */
        // ========================================================================================================

        /// <summary>
        /// Consulta as apostas (palpites) para uma rodada específica.
        /// </summary>
        /// <param name="rodadaId">O ID da rodada para a qual consultar as apostas.</param>
        /// <returns>Um IActionResult contendo uma ApiResponse com a lista de PalpiteDto.</returns>
        [HttpGet("consultar-apostas-da-rodada/{rodadaId:guid}")]
        public async Task<IActionResult> ConsultarApostas(Guid rodadaId)
        {
            var response = await _palpiteService.ObterPalpitesDaRodada(rodadaId);
            return CustomApiResponse(response); // Usa CustomApiResponse
        }

        /// <summary>
        /// Adiciona um novo palpite.
        /// </summary>
        /// <param name="request">O DTO de requisição contendo os dados do palpite.</param>
        /// <returns>Um IActionResult contendo uma ApiResponse com o DTO do palpite adicionado.</returns>
        [HttpPost("adicionar")]
        public async Task<IActionResult> AdicionarPalpite([FromBody] SalvarPalpiteRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return CustomValidationProblem(ModelState); // Usa CustomValidationProblem
            }

            var response = await _palpiteService.AdicionarPalpite(request);
            return CustomApiResponse(response); // Usa CustomApiResponse
        }

        /// <summary>
        /// Atualiza um palpite existente.
        /// </summary>
        /// <param name="id">O ID do palpite a ser atualizado.</param>
        /// <param name="request">O DTO de requisição contendo os novos dados do palpite.</param>
        /// <returns>Um IActionResult contendo uma ApiResponse com o DTO do palpite atualizado.</returns>
        [HttpPut("atualizar/{id:guid}")]
        public async Task<IActionResult> AtualizarPalpite(Guid id, [FromBody] SalvarPalpiteRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return CustomValidationProblem(ModelState); // Usa CustomValidationProblem
            }

            var response = await _palpiteService.AtualizarPalpite(id, request);
            return CustomApiResponse(response); // Usa CustomApiResponse
        }

        /// <summary>
        /// Remove um palpite pelo seu ID.
        /// </summary>
        /// <param name="id">O ID do palpite a ser removido.</param>
        /// <returns>Um IActionResult contendo uma ApiResponse indicando o sucesso da remoção.</returns>
        [HttpDelete("remover/{id:guid}")] // Adicionado rota para remover
        public async Task<IActionResult> RemoverPalpite(Guid id)
        {
            var response = await _palpiteService.RemoverPalpite(id);
            return CustomApiResponse(response); // Usa CustomApiResponse
        }

        /// <summary>
        /// Verifica se existem apostas (palpites) para uma rodada específica.
        /// </summary>
        /// <param name="rodadaId">O ID da rodada.</param>
        /// <returns>Um IActionResult contendo uma ApiResponse indicando se existem palpites.</returns>
        [HttpGet("existe-palpites-rodada/{rodadaId:guid}")] // Adicionado rota para verificar existência
        public async Task<IActionResult> ExistePalpitesParaRodada(Guid rodadaId)
        {
            var response = await _palpiteService.ExistePalpitesParaRodada(rodadaId);
            return CustomApiResponse(response); // Usa CustomApiResponse
        }
    }
}
