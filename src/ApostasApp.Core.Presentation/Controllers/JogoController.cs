using ApostasApp.Core.Application.Services.Interfaces.Jogos; // Para IJogoService
using ApostasApp.Core.Application.Services.Interfaces.Rodadas; // Para IRodadaService
using ApostasApp.Core.Domain.Interfaces.Notificacoes; // Para INotificador
using AutoMapper; // Apenas se houver necessidade de mapeamentos no Controller
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq; // Para Linq
using System.Threading.Tasks;

namespace ApostasApp.Web.Controllers
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
                              INotificador notificador) : base(notificador)
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
                Notificar("Alerta", "Jogo não encontrado."); // Notificação mais específica
                return NotFound(ObterTodasNotificacoes());
            }

            return Ok(jogoDetalheDto);
        }

        // ========================================================================================================
        // O método ManterJogos (HttpGet("jogos-da-rodada-em-construcao")) FOI REMOVIDO DESTE CONTROLADOR.
        // Sua funcionalidade é administrativa e pertence ao projeto BolaoAdm (MVC).
        // Não deve ser exposta nesta API para o usuário final do ApostasApp.
        // ========================================================================================================

        // Outros métodos específicos para o usuário final do ApostasApp podem ser adicionados aqui.
    }
}
