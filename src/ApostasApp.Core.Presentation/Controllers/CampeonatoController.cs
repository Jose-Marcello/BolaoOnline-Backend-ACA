// ApostasApp.Web/Controllers/CampeonatoController.cs

using ApostasApp.Core.Application.Services.Interfaces.Campeonatos;
using ApostasApp.Core.Application.Services.Interfaces; // Necessário para INotificador, que é injetado
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using ApostasApp.Core.Domain.Interfaces.Notificacoes;


namespace ApostasApp.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CampeonatoController : BaseController
    {
        private readonly ICampeonatoService _campeonatoService;

        // >>> ESTE CONSTRUTOR É CRÍTICO: ELE DEVE INJETAR O NOTIFICADOR E PASSÁ-LO PARA A BASE <<<
        public CampeonatoController(
            ICampeonatoService campeonatoService,
            INotificador notificador) : base(notificador) // <-- Chamar o construtor da classe base aqui
        {
            _campeonatoService = campeonatoService;
        }

        [HttpGet]
        public async Task<ActionResult> GetAvailableCampeonatos()
        {
            var campeonatos = await _campeonatoService.GetAvailableCampeonatosAsync();
            return CustomResponse(campeonatos);
        }

        [HttpPost("{campeonatoId}/aderir")]
        public async Task<ActionResult> AdherirCampeonato(
            Guid campeonatoId,
            [FromHeader(Name = "X-Apostador-Id")] Guid apostadorId)
        {
            if (apostadorId == Guid.Empty)
            {
                // >>> AGORA, USE O MÉTODO Notificar DIRETAMENTE, POIS ELE É HERDADO DE BaseController <<<
                Notificar("Erro", "ID do apostador é obrigatório."); // Você pode usar o overload Notificar("Mensagem") também.
                return CustomResponse();
            }

            var sucesso = await _campeonatoService.AdherirCampeonatoAsync(apostadorId, campeonatoId);

            if (!sucesso)
            {
                return CustomResponse();
            }

            return CustomResponse(new { message = $"Adesão ao campeonato '{campeonatoId}' realizada com sucesso!" });
        }
    }
}