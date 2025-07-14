// Localização: ApostasApp.Core.Web/Controllers/JogoController.cs

using ApostasApp.Core.Application.Services.Interfaces.Jogos; // Para IJogoService
using ApostasApp.Core.Application.Services.Interfaces.Rodadas; // Para IRodadaService
using ApostasApp.Core.Domain.Interfaces.Notificacoes; // Para INotificador
using ApostasApp.Core.Application.DTOs.Jogos; // Para JogoDetalheDto
using ApostasApp.Core.Application.Models; // Para ApiResponse
using ApostasApp.Core.Web.Controllers; // Para BaseController
using AutoMapper; // Apenas se houver necessidade de mapeamentos no Controller
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq; // Para Linq
using System.Threading.Tasks;
using ApostasApp.Core.Domain.Interfaces; // Para IUnitOfWork (se ainda for necessário para DI, mas não para BaseController)

namespace ApostasApp.Core.Web.Controllers // Namespace CORRIGIDO para ApostasApp.Core.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JogoController : BaseController
    {
        private readonly IMapper _mapper; // Mantido para flexibilidade
        private readonly IJogoService _jogoService; // Injete o serviço de Jogos
        private readonly IRodadaService _rodadaService; // Injete o serviço de Rodadas

        public JogoController(IMapper mapper,
                              IJogoService jogoService,
                              IRodadaService rodadaService,
                              INotificador notificador
                              /* REMOVIDO: IUnitOfWork uow, pois BaseController não o recebe mais no construtor */)
            : base(notificador) // Passa apenas o notificador para a BaseController
        {
            _mapper = mapper;
            _jogoService = jogoService;
            _rodadaService = rodadaService;
        }

        /// <summary>
        /// Obtém os detalhes de um jogo específico.
        /// </summary>
        /// <param name="id">O ID do jogo.</param>
        /// <returns>Os detalhes do jogo em formato DTO (JSON).</returns>
        [HttpGet("dados-do-jogo/{id:guid}")]
        public async Task<IActionResult> DetalheJogo(Guid id)
        {
            var jogoDetalheDto = await _jogoService.ObterDetalhesJogo(id);

            if (jogoDetalheDto == null)
            {
                // CORRIGIDO: Usando NotificarAlerta do BaseController
                NotificarAlerta("Jogo não encontrado.");
                // CORRIGIDO: Usando CustomResponse para retornar a ApiResponse padronizada
                return CustomResponse<JogoDetalheDto>();
            }

            // CORRIGIDO: Usando CustomResponse para retornar o DTO encapsulado em ApiResponse de sucesso
            return CustomResponse(jogoDetalheDto);
        }

        // ========================================================================================================
        // O método ManterJogos (HttpGet("jogos-da-rodada-em-construcao")) FOI REMOVIDO DESTE CONTROLADOR.
        // Sua funcionalidade é administrativa e pertence ao projeto BolaoAdm (MVC).
        // Não deve ser exposta nesta API para o usuário final do ApostasApp.
        // ========================================================================================================

        // Outros métodos específicos para o usuário final do ApostasApp podem ser adicionados aqui.
    }
}
