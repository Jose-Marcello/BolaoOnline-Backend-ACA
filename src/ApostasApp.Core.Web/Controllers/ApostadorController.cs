// Localização: ApostasApp.Core.Web/Controllers/ApostadorController.cs
// NOVO controlador para lidar com dados gerais do apostador.
// Ele fornecerá os endpoints que o frontend Angular está a procurar.

using Microsoft.AspNetCore.Mvc;
using ApostasApp.Core.Domain.Interfaces.Notificacoes;
using ApostasApp.Core.Application.DTOs.Apostadores;
using ApostasApp.Core.Application.DTOs.Campeonatos;
using ApostasApp.Core.Application.DTOs.Rodadas;
using ApostasApp.Core.Application.Services.Interfaces.Apostadores;
using ApostasApp.Core.Application.Services.Interfaces.Campeonatos;
using ApostasApp.Core.Application.Services.Interfaces.Rodadas;
using Microsoft.AspNetCore.Authorization;
using System.Linq; // Necessário para o método .Any()
using AutoMapper;
using System.Collections.Generic;
using ApostasApp.Core.Domain.Interfaces; // Para IUnitOfWork (se ainda for necessário para DI, mas não para BaseController)
using System.Threading.Tasks;
using System;
using ApostasApp.Core.Application.Models; // Para ApiResponse
using ApostasApp.Core.Application.Services.Interfaces.Usuarios; // Para IUsuarioService

namespace ApostasApp.Core.Web.Controllers // Namespace CORRIGIDO para ApostasApp.Core.Web.Controllers
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
            // REMOVIDO: IUnitOfWork uow, pois BaseController não o recebe mais no construtor
            IUsuarioService usuarioService,
            IApostadorService apostadorService,
            IApostadorCampeonatoService apostadorCampeonatoService,
            IRodadaService rodadaService,
            ICampeonatoService campeonatoService,
            IMapper mapper)
            : base(notificador) // Passa apenas o notificador para a BaseController
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
            var userId = ObterUsuarioIdLogado(); // Método do BaseController
            if (string.IsNullOrEmpty(userId))
            {
                // CORRIGIDO: Usando NotificarErro do BaseController
                NotificarErro("Usuário não identificado ou token inválido.", "USUARIO_NAO_IDENTIFICADO");
                return CustomResponse<ApostadorDto>(); // Usa CustomResponse do BaseController
            }

            var apostadorEntity = await _apostadorService.GetApostadorByUserIdAsync(userId);

            if (apostadorEntity == null)
            {
                // CORRIGIDO: Usando NotificarAlerta do BaseController
                NotificarAlerta("Dados do apostador não encontrados.", "DADOS_APOSTADOR_NAO_ENCONTRADOS");
                return CustomResponse<ApostadorDto>(); // Usa CustomResponse do BaseController
            }

            var apostadorDto = _mapper.Map<ApostadorDto>(apostadorEntity);

            if (apostadorDto.Saldo != null && string.IsNullOrEmpty(apostadorDto.Saldo.ApostadorId))
            {
                apostadorDto.Saldo.ApostadorId = apostadorEntity.Id.ToString();
            }

            return CustomResponse(apostadorDto); // Retorna o DTO encapsulado em ApiResponse de sucesso
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
                NotificarErro("Usuário não logado ou sessão expirada.", "USUARIO_NAO_LOGADO");
                return CustomResponse<List<CampeonatoDto>>();
            }

            var campeonatosEntity = await _apostadorCampeonatoService.ObterTodosCampeonatosDoApostadorPorUsuarioId(loggedInUser.Id);
            var campeonatosDto = _mapper.Map<List<CampeonatoDto>>(campeonatosEntity);

            if (campeonatosDto == null || !campeonatosDto.Any())
            {
                NotificarAlerta("Nenhum campeonato associado encontrado para o apostador.", "NENHUM_CAMPEONATO_ASSOCIADO");
                return CustomResponse<List<CampeonatoDto>>(new List<CampeonatoDto>()); // Retorna lista vazia com alerta
            }

            return CustomResponse(campeonatosDto);
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

            if (UsuarioEstaAutenticado()) // Método do BaseController
            {
                userId = ObterUsuarioIdLogado(); // Método do BaseController
            }

            var response = await _campeonatoService.GetAvailableCampeonatos(userId);

            // O serviço já deve ter adicionado notificações se algo deu errado
            if (!response.Success) // Se o serviço indicou falha
            {
                // As notificações já estarão em response.Notifications (NotificationDto)
                return CustomResponse(response);
            }

            // <<-- CORRIGIDO: Acessando response.Data antes de chamar Any() -->>
            if (response.Data == null || !response.Data.Any())
            {
                NotificarAlerta("Nenhum campeonato ativo disponível no momento.", "NENHUM_CAMPEONATO_DISPONIVEL");
                return CustomResponse<IEnumerable<CampeonatoDto>>(new List<CampeonatoDto>()); // Retorna lista vazia com alerta
            }

            return CustomResponse(response); // Retorna a resposta de sucesso do serviço
        }

        /// <summary>
        /// Obtém a lista de rodadas em destaque.
        /// Corresponde a GET /api/Apostador/RodadasEmDestaque
        /// </summary>
        [HttpGet("RodadasEmDestaque")]
        [AllowAnonymous] // <--- AGORA ELE SERÁ REALMENTE ANÔNIMO
        public async Task<IActionResult> GetRodadasEmDestaque()
        {
            var rodadasApiResponse = await _rodadaService.ObterRodadasEmDestaque(); // Retorna ApiResponse<IEnumerable<RodadaDto>>

            // O serviço já deve ter adicionado notificações se algo deu errado
            if (!rodadasApiResponse.Success)
            {
                return CustomResponse(rodadasApiResponse);
            }

            var rodadasDto = rodadasApiResponse.Data?.ToList(); // Mapeia o Data da ApiResponse para List<RodadaDto>

            if (rodadasDto == null || !rodadasDto.Any())
            {
                NotificarAlerta("Nenhuma rodada em destaque encontrada.", "NENHUMA_RODADA_DESTAQUE");
                return CustomResponse<IEnumerable<RodadaDto>>(new List<RodadaDto>()); // Retorna lista vazia com alerta
            }

            return CustomResponse(rodadasApiResponse); // Retorna a resposta de sucesso do serviço
        }

        [HttpPost("AderirCampeonato")]
        [Authorize] // Este método CONTINUA protegido por [Authorize]
        public async Task<IActionResult> AderirCampeonato([FromBody] VincularApostadorCampeonatoDto request)
        {
            var userIdString = ObterUsuarioIdLogado(); // Método do BaseController

            if (string.IsNullOrEmpty(userIdString))
            {
                // CORRIGIDO: Usando NotificarErro do BaseController
                NotificarErro("Usuário não autenticado ou token inválido.", "USUARIO_NAO_AUTENTICADO");
                return CustomResponse<bool>(); // Usa CustomResponse do BaseController
            }

            var apostadorEntity = await _apostadorService.GetApostadorByUserIdAsync(userIdString);
            if (apostadorEntity == null)
            {
                // CORRIGIDO: Usando NotificarAlerta do BaseController
                NotificarAlerta("Dados do apostador não encontrados.", "APOSTADOR_NAO_ENCONTRADO");
                return CustomResponse<bool>(); // Usa CustomResponse do BaseController
            }

            if (!string.IsNullOrEmpty(request.ApostadorId) && request.ApostadorId != apostadorEntity.Id.ToString())
            {
                // CORRIGIDO: Usando NotificarErro do BaseController
                NotificarErro("ID do apostador na requisição inválido ou não corresponde ao usuário logado.", "ID_APOSTADOR_INVALIDO");
                return CustomResponse<bool>(); // Usa CustomResponse do BaseController
            }

            Guid campeonatoIdGuid;
            if (!Guid.TryParse(request.CampeonatoId, out campeonatoIdGuid))
            {
                // CORRIGIDO: Usando NotificarErro do BaseController
                NotificarErro("ID do Campeonato fornecido é inválido.", "ID_CAMPEONATO_INVALIDO");
                return CustomResponse<bool>(); // Usa CustomResponse do BaseController
            }

            var resultadoAdesao = await _campeonatoService.AderirCampeonatoAsync(apostadorEntity.Id, campeonatoIdGuid);

            return CustomResponse(resultadoAdesao); // Retorna a ApiResponse do serviço de forma consistente
        }
    }
}
