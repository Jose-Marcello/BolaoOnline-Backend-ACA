// ApostadorController.cs
// NOVO controlador para lidar com dados gerais do apostador.
// Ele fornecerá os endpoints que o frontend Angular está a procurar.

using Microsoft.AspNetCore.Mvc;
using ApostasApp.Core.Domain.Interfaces.Notificacoes;
using ApostasApp.Core.Domain.Interfaces.Usuarios;
using ApostasApp.Core.Application.DTOs.Apostadores;
using ApostasApp.Core.Application.DTOs.Campeonatos;
using ApostasApp.Core.Application.DTOs.Rodadas;
using ApostasApp.Core.Application.DTOs.Financeiro;
using ApostasApp.Core.Application.Services.Interfaces.Apostadores;
using ApostasApp.Core.Application.Services.Interfaces.Campeonatos;
using ApostasApp.Core.Application.Services.Interfaces.Rodadas;
using Microsoft.AspNetCore.Authorization;
using System.Linq; // Necessário para o método .Any()
using AutoMapper;
using System.Collections.Generic;
using ApostasApp.Core.Domain.Interfaces;
using System.Threading.Tasks;
using System;
using ApostasApp.Core.Application.Models; // Para ApiResponse
using ApostasApp.Core.Domain.Models.Notificacoes; // Para Notificacao (entidade de domínio)

namespace ApostasApp.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Todos os métodos neste controlador exigirão autenticação por padrão, EXCETO os com [AllowAnonymous]
    public class ApostadorController : BaseController
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IApostadorService _apostadorService;
        private readonly IApostadorCampeonatoService _apostadorCampeonatoService;
        private readonly IRodadaService _rodadaService;
        private readonly ICampeonatoService _campeonatoService;
        private readonly IMapper _mapper;

        public ApostadorController(
            INotificador notificador,
            IUnitOfWork uow,
            IUsuarioService usuarioService,
            IApostadorService apostadorService,
            IApostadorCampeonatoService apostadorCampeonatoService,
            IRodadaService rodadaService,
            ICampeonatoService campeonatoService,
            IMapper mapper)
            : base(notificador, uow)
        {
            _usuarioService = usuarioService;
            _apostadorService = apostadorService;
            _apostadorCampeonatoService = apostadorCampeonatoService;
            _rodadaService = rodadaService;
            _campeonatoService = campeonatoService;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtém os dados detalhados do apostador logado, incluindo apelido e saldo.
        /// Corresponde a GET /api/Apostador/Dados
        /// </summary>
        [HttpGet("Dados")]
        public async Task<IActionResult> GetDadosApostador()
        {
            var userId = ObterUsuarioLogadoId();
            if (string.IsNullOrEmpty(userId))
            {
                var apiResponse = new ApiResponse<ApostadorDto>(false, null);
                Notificador.Handle(new Notificacao("USUARIO_NAO_IDENTIFICADO", "Erro", "Usuário não identificado ou token inválido."));
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return CustomApiResponse(apiResponse);
            }

            var apostadorEntity = await _apostadorService.GetApostadorByUserIdAsync(userId);

            if (apostadorEntity == null)
            {
                var apiResponse = new ApiResponse<ApostadorDto>(false, null);
                Notificador.Handle(new Notificacao("DADOS_APOSTADOR_NAO_ENCONTRADOS", "Alerta", "Dados do apostador não encontrados."));
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return CustomApiResponse(apiResponse);
            }

            var apostadorDto = _mapper.Map<ApostadorDto>(apostadorEntity);

            if (apostadorDto.Saldo != null && apostadorDto.Saldo.ApostadorId == Guid.Empty)
            {
                apostadorDto.Saldo.ApostadorId = apostadorEntity.Id;
            }

            return CustomApiResponse(new ApiResponse<ApostadorDto>(true, apostadorDto));
        }

        // ==============================================================================
        // Opcional: Se você ainda quer uma lista de "Meus Campeonatos" para histórico, mantenha este.
        // Caso contrário, pode removê-lo como tínhamos discutido antes.
        /*
        /// <summary>
        /// Obtém os campeonatos aos quais o apostador logado está explicitamente associado (Meus Campeonatos).
        /// Corresponde a GET /api/Apostador/CampeonatosDoApostador
        /// </summary>
        [HttpGet("CampeonatosDoApostador")]
        // Este método CONTINUA protegido por [Authorize] da classe
        public async Task<IActionResult> GetCampeonatosDoApostador()
        {
            var loggedInUser = await _usuarioService.GetLoggedInUser();
            if (loggedInUser == null)
            {
                var apiResponse = new ApiResponse<List<CampeonatoDto>>(false, null);
                Notificador.Handle(new Notificacao("USUARIO_NAO_LOGADO", "Erro", "Usuário não logado ou sessão expirada."));
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return CustomApiResponse(apiResponse);
            }

            var campeonatosEntity = await _apostadorCampeonatoService.ObterTodosCampeonatosDoApostadorPorUsuarioId(loggedInUser.Id);
            var campeonatosDto = _mapper.Map<List<CampeonatoDto>>(campeonatosEntity);

            if (campeonatosDto == null || !campeonatosDto.Any())
            {
                var apiResponse = new ApiResponse<List<CampeonatoDto>>(true, new List<CampeonatoDto>());
                Notificador.Handle(new Notificacao("NENHUM_CAMPEONATO_ASSOCIADO", "Alerta", "Nenhum campeonato associado encontrado para o apostador."));
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return CustomApiResponse(apiResponse);
            }

            return CustomApiResponse(new ApiResponse<List<CampeonatoDto>>(true, campeonatosDto));
        }
        */
        // ==============================================================================


        /// <summary>
        /// Obtém todos os campeonatos ativos disponíveis para o usuário visualizar e potencialmente aderir.
        /// Corresponde a GET /api/Apostador/TodosCampeonatosDisponiveis
        /// </summary>
        [AllowAnonymous] // <--- AGORA ELE SERÁ REALMENTE ANÔNIMO
        [HttpGet("TodosCampeonatosDisponiveis")]
        public async Task<IActionResult> GetTodosCampeonatosDisponiveis()
        {
            string? userId = null; // Inicialize userId como null por padrão

            if (UsuarioEstaAutenticado())
            {
                userId = ObterUsuarioLogadoId();
            }

            var response = await _campeonatoService.GetAvailableCampeonatos(userId);

            // O serviço já deve ter adicionado notificações se algo deu errado
            if (!response.Success) // Se o serviço indicou falha
            {
                // As notificações já estarão em response.Notifications
                return CustomApiResponse(response);
            }

            // <<-- CORRIGIDO: Acessando response.Data antes de chamar Any() -->>
            if (response.Data == null || !response.Data.Any())
            {
                var apiResponse = new ApiResponse<IEnumerable<CampeonatoDto>>(true, new List<CampeonatoDto>());
                Notificador.Handle(new Notificacao("NENHUM_CAMPEONATO_DISPONIVEL", "Alerta", "Nenhum campeonato ativo disponível no momento."));
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return CustomApiResponse(apiResponse);
            }

            return CustomApiResponse(response); // Retorna a resposta de sucesso do serviço
        }

        /// <summary>
        /// Obtém a lista de rodadas em destaque.
        /// Corresponde a GET /api/Apostador/RodadasEmDestaque
        /// </summary>
        [HttpGet("RodadasEmDestaque")]
        [AllowAnonymous] // <--- AGORA ELE SERÁ REALMENTE ANÔNIMO
        public async Task<IActionResult> GetRodadasEmDestaque()
        {
            var rodadasEntity = await _rodadaService.ObterRodadasEmDestaque();

            // <<-- CORRIGIDO: Verificando se o serviço adicionou notificações -->>
            if (Notificador.TemNotificacao())
            {
                var apiResponse = new ApiResponse<IEnumerable<RodadaDto>>(false, null);
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return CustomApiResponse(apiResponse);
            }

            var rodadasDto = _mapper.Map<List<RodadaDto>>(rodadasEntity);

            if (rodadasDto == null || !rodadasDto.Any())
            {
                var apiResponse = new ApiResponse<IEnumerable<RodadaDto>>(true, new List<RodadaDto>());
                Notificador.Handle(new Notificacao("NENHUMA_RODADA_DESTAQUE", "Alerta", "Nenhuma rodada em destaque encontrada."));
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return CustomApiResponse(apiResponse);
            }

            return CustomApiResponse(new ApiResponse<IEnumerable<RodadaDto>>(true, rodadasDto));
        }

        [HttpPost("AderirCampeonato")]
        [Authorize] // Este método CONTINUA protegido por [Authorize]
        public async Task<IActionResult> AderirCampeonato([FromBody] VincularApostadorCampeonatoDto request)
        {
            var userIdString = ObterUsuarioLogadoId();
            var apiResponse = new ApiResponse<bool>(false, false);

            if (string.IsNullOrEmpty(userIdString))
            {
                Notificador.Handle(new Notificacao("USUARIO_NAO_AUTENTICADO", "Erro", "Usuário não autenticado ou token inválido."));
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return CustomApiResponse(apiResponse);
            }

            var apostadorEntity = await _apostadorService.GetApostadorByUserIdAsync(userIdString);
            if (apostadorEntity == null)
            {
                Notificador.Handle(new Notificacao("APOSTADOR_NAO_ENCONTRADO", "Alerta", "Dados do apostador não encontrados."));
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return CustomApiResponse(apiResponse);
            }

            if (!string.IsNullOrEmpty(request.ApostadorId) && request.ApostadorId != apostadorEntity.Id.ToString())
            {
                Notificador.Handle(new Notificacao("ID_APOSTADOR_INVALIDO", "Erro", "ID do apostador na requisição inválido ou não corresponde ao usuário logado."));
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return CustomApiResponse(apiResponse);
            }

            Guid campeonatoIdGuid;
            if (!Guid.TryParse(request.CampeonatoId, out campeonatoIdGuid))
            {
                Notificador.Handle(new Notificacao("ID_CAMPEONATO_INVALIDO", "Erro", "ID do Campeonato fornecido é inválido."));
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return CustomApiResponse(apiResponse);
            }

            var resultadoAdesao = await _campeonatoService.AderirCampeonatoAsync(apostadorEntity.Id, campeonatoIdGuid);

            return CustomApiResponse(resultadoAdesao);
        }
    }
}
