// Localização: ApostasApp.Core.Web/Controllers/PalpiteController.cs

using ApostasApp.Core.Application.Services.Interfaces.Campeonatos;
using ApostasApp.Core.Application.Services.Interfaces.Jogos;
using ApostasApp.Core.Application.Services.Interfaces.RankingRodadas;
using ApostasApp.Core.Application.Services.Interfaces.Rodadas;
using ApostasApp.Core.Application.Services.Interfaces.Palpites; // Para IPalpiteService
using ApostasApp.Core.Domain.Interfaces.Notificacoes;
using ApostasApp.Core.Application.DTOs.Apostas; // Para PalpiteDto, SalvarPalpiteRequestDto
using ApostasApp.Core.Application.Models; // Para ApiResponse
using ApostasApp.Core.Web.Controllers; // Para BaseController
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ApostasApp.Core.Domain.Interfaces; // Para IUnitOfWork (se ainda for necessário para DI, mas não para BaseController)
using System; // Para Guid
using System.Collections.Generic;
using System.Linq; // Para Linq
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization; // Para [Authorize]
using Microsoft.AspNetCore.Mvc.ModelBinding; // Para ModelStateDictionary

namespace ApostasApp.Core.Web.Controllers
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
                                 INotificador notificador
                                 /* REMOVIDO: IUnitOfWork uow, pois BaseController não o recebe mais no construtor */)
            : base(notificador) // Passa apenas o notificador para a BaseController
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
            return CustomResponse(response); // Usa CustomResponse
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
            return CustomResponse(response); // Usa CustomResponse
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
            return CustomResponse(response); // Usa CustomResponse
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
            return CustomResponse(response); // Usa CustomResponse
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
            return CustomResponse(response); // Usa CustomResponse
        }
    }
}
