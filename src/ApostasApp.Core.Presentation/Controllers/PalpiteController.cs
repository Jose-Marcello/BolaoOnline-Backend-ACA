using ApostasApp.Core.Application.Services.Interfaces.Campeonatos;
using ApostasApp.Core.Application.Services.Interfaces.Jogos;
using ApostasApp.Core.Application.Services.Interfaces.RankingRodadas;
using ApostasApp.Core.Application.Services.Interfaces.Rodadas;
using ApostasApp.Core.Application.Services.Interfaces.Palpites; // Para IPalpiteService
using ApostasApp.Core.Domain.Interfaces.Notificacoes;
using ApostasApp.Core.Application.DTOs.Apostas; // Para PalpiteDto, SalvarPalpiteRequestDto
using AutoMapper;
using Microsoft.AspNetCore.Mvc;


namespace ApostasApp.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
                                 INotificador notificador) : base(notificador)
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
        /// <returns>Uma lista de PalpiteDto.</returns>
        [HttpGet("consultar-apostas-da-rodada/{rodadaId:guid}")] // Rota agora aceita o ID da rodada
        public async Task<IActionResult> ConsultarApostas(Guid rodadaId)
        {
            // Opcional: Verificar se a rodada existe, se isso for uma regra de negócio importante aqui.
            // var rodadaExistente = await _rodadaService.ObterRodadaPorId(rodadaId);
            // if (rodadaExistente == null)
            // {
            //     Notificar("Alerta", "Rodada não encontrada.");
            //     return CustomResponse(); // Ou NotFound com notificações
            // }

            // O serviço de palpites já retorna PalpiteDto, usando o rodadaId fornecido.
            var palpitesDto = await _palpiteService.ObterPalpitesDaRodada(rodadaId);

            // Verifica se o serviço adicionou alguma notificação (ex: nenhum palpite encontrado)
            if (TemNotificacao())
            {
                return CustomResponse(); // Retorna BadRequest com as notificações
            }
            else
            {
                return CustomResponse(palpitesDto); // Retorna Ok com os DTOs diretamente
            }
        }

        /// <summary>
        /// Adiciona um novo palpite.
        /// </summary>
        /// <param name="request">O DTO de requisição contendo os dados do palpite.</param>
        /// <returns>O DTO do palpite adicionado, ou BadRequest/InternalServerError em caso de falha.</returns>
        [HttpPost("adicionar")]
        // [Authorize] // Adicione este atributo se o usuário precisar estar autenticado
        public async Task<IActionResult> AdicionarPalpite([FromBody] SalvarPalpiteRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return CustomResponse(ModelState);
            }

            var palpiteAdicionado = await _palpiteService.AdicionarPalpite(request);

            if (TemNotificacao())
            {
                return CustomResponse();
            }
            else if (palpiteAdicionado == null)
            {
                Notificar("Erro", "Ocorreu um erro interno ao adicionar o palpite.");
                return StatusCode(500, ObterTodasNotificacoes()); // Retorno para erro inesperado
            }
            else
            {
                return CustomResponse(palpiteAdicionado);
            }
        }

        /// <summary>
        /// Atualiza um palpite existente.
        /// </summary>
        /// <param name="id">O ID do palpite a ser atualizado.</param>
        /// <param name="request">O DTO de requisição contendo os novos dados do palpite.</param>
        /// <returns>O DTO do palpite atualizado, ou NotFound/BadRequest/InternalServerError em caso de falha.</returns>
        [HttpPut("atualizar/{id:guid}")]
        // [Authorize] // Adicione este atributo se o usuário precisar estar autenticado
        public async Task<IActionResult> AtualizarPalpite(Guid id, [FromBody] SalvarPalpiteRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return CustomResponse(ModelState);
            }

            var palpiteAtualizado = await _palpiteService.AtualizarPalpite(id, request);

            if (TemNotificacao())
            {
                return CustomResponse();
            }
            else if (palpiteAtualizado == null)
            {
                Notificar("Erro", "Ocorreu um erro interno ao atualizar o palpite ou o palpite não foi encontrado.");
                return NotFound(ObterTodasNotificacoes());
            }
            else
            {
                return CustomResponse(palpiteAtualizado);
            }
        }

    }
}