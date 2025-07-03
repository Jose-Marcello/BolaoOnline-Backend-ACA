// ApostasApp.Web/Controllers/BaseController.cs

using ApostasApp.Core.Application.DTOs.Usuarios; // Manter se necessário para outros métodos
using ApostasApp.Core.Domain.Interfaces; // Para IUnitOfWork
using ApostasApp.Core.Domain.Interfaces.Notificacoes; // Para INotificador
using ApostasApp.Core.Domain.Models.Notificacoes; // Para o modelo de domínio Notificacao
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System; // Adicionar para usar Guid
using System.Collections.Generic;
using System.Linq; // Para usar o .Select
using System.Security.Claims; // Adicionar para usar Claims
using Microsoft.AspNetCore.Http; // Para StatusCodes
using ApostasApp.Core.Application.Models; // Para NotificationDto

namespace ApostasApp.Web.Controllers
{
    /// <summary>
    /// Classe base abstrata para todos os controladores da aplicação.
    /// Fornece funcionalidades comuns como acesso ao Notificador e tratamento de notificações.
    /// </summary>
    // <<-- REMOVIDO: [ApiController] deve estar nos controladores derivados -->>
    public abstract class BaseController : ControllerBase
    {
        protected readonly INotificador Notificador;
        protected readonly IUnitOfWork Uow; // Propriedade para IUnitOfWork

        // Construtor que aceita INotificador e IUnitOfWork
        public BaseController(INotificador notificador, IUnitOfWork uow)
        {
            Notificador = notificador;
            Uow = uow; // Atribuição do IUnitOfWork
        }

        /// <summary>
        /// Obtém o ID do usuário logado a partir dos claims do token JWT.
        /// Retorna Guid.Empty se o ID não for encontrado ou não for um Guid válido.
        /// </summary>
        protected string? ObterUsuarioLogadoId()
        {
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            return userIdClaim?.Value;
        }

        /// <summary>
        /// Verifica se o usuário está autenticado.
        /// </summary>
        protected bool UsuarioEstaAutenticado()
        {
            return HttpContext.User.Identity?.IsAuthenticated == true && !string.IsNullOrEmpty(ObterUsuarioLogadoId());
        }

        // <<-- CORRIGIDO: Métodos Notificar para incluir 'codigo' -->>

        /// <summary>
        /// Adiciona uma notificação de erro com uma mensagem específica.
        /// </summary>
        /// <param name="mensagem">A mensagem de erro.</param>
        /// <param name="codigo">Opcional: Código de erro.</param>
        protected void NotificarErro(string mensagem, string codigo = null)
        {
            Notificador.Handle(new Notificacao(codigo, "Erro", mensagem));
        }

        /// <summary>
        /// Adiciona uma notificação com um tipo (ex: "Alerta", "Sucesso", "Erro") e uma mensagem.
        /// </summary>
        /// <param name="tipo">O tipo da notificação (ex: "Alerta").</param>
        /// <param name="mensagem">A mensagem da notificação.</param>
        /// <param name="codigo">Opcional: Código da notificação.</param>
        protected void Notificar(string tipo, string mensagem, string codigo = null)
        {
            Notificador.Handle(new Notificacao(codigo, tipo, mensagem));
        }

        /// <summary>
        /// Adiciona uma notificação com tipo, mensagem e nome de campo opcional.
        /// </summary>
        /// <param name="tipo">O tipo da notificação (ex: "Erro").</param>
        /// <param name="mensagem">A mensagem da notificação.</param>
        /// <param name="nomeCampo">Opcional: O nome do campo a que a notificação se refere.</param>
        /// <param name="codigo">Opcional: Código da notificação.</param>
        protected void Notificar(string tipo, string mensagem, string nomeCampo, string codigo = null)
        {
            Notificador.Handle(new Notificacao(codigo, tipo, mensagem, nomeCampo));
        }

        /// <summary>
        /// Adiciona uma notificação usando um objeto Notificacao existente.
        /// </summary>
        protected void Notificar(Notificacao notificacao)
        {
            Notificador.Handle(notificacao);
        }

        /// <summary>
        /// Verifica se há notificações registradas.
        /// </summary>
        /// <returns>True se houver notificações, false caso contrário.</returns>
        protected bool TemNotificacao()
        {
            return Notificador.TemNotificacao();
        }

        /// <summary>
        /// Obtém todas as notificações registradas e as mapeia para DTOs.
        /// </summary>
        /// <returns>Uma coleção de objetos NotificationDto.</returns>
        // <<-- CORRIGIDO: Mapeando a propriedade 'Codigo' -->>
        protected IEnumerable<NotificationDto> ObterNotificacoesParaResposta()
        {
            return Notificador.ObterNotificacoes().Select(n => new NotificationDto
            {
                Codigo = n.Codigo, // Mapeia o código
                Tipo = n.Tipo,
                Mensagem = n.Mensagem,
                NomeCampo = n.NomeCampo
            });
        }

        /// <summary>
        /// Retorna um ProblemDetails para erros de validação de modelo.
        /// Este método é para ser usado quando ModelState.IsValid é false.
        /// </summary>
        /// <param name="modelState">O ModelState atual do controlador.</param>
        /// <returns>Um BadRequestObjectResult com os detalhes do problema.</returns>
        protected IActionResult CustomValidationProblem(ModelStateDictionary modelState)
        {
            var problemDetails = new ValidationProblemDetails(modelState)
            {
                Title = "One or more validation errors occurred.",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Ocorreram erros na requisição.",
                Instance = HttpContext.Request.Path
            };

            // Adicionar notificações do _notificador às extensões do problemDetails
            if (Notificador.TemNotificacao())
            {
                var notificationsDto = ObterNotificacoesParaResposta().ToList();
                problemDetails.Extensions.Add("notifications", notificationsDto);
            }

            return new BadRequestObjectResult(problemDetails)
            {
                ContentTypes = { "application/problem+json", "application/json" }
            };
        }

        // <<-- REMOVIDO: Este método CustomResponse é redundante e causa confusão -->>
        // <<-- REMOVIDO: protected ActionResult CustomResponse(bool success, object data = null, string message = null) -->>
        // A lógica de retorno deve ser tratada diretamente no controlador com Ok(response) ou BadRequest(response)
        // usando a ApiResponse do serviço.

        /// <summary>
        /// Retorna uma resposta padronizada para APIs, incluindo notificações.
        /// Este método é para ser usado quando a operação do serviço já retornou um ApiResponse.
        /// </summary>
        /// <param name="response">O objeto ApiResponse a ser retornado.</param>
        /// <returns>Um IActionResult com o status HTTP apropriado (200 OK ou 400 BadRequest).</returns>
        protected IActionResult CustomApiResponse<T>(ApiResponse<T> response)
        {
            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                // Se a ApiResponse em si sinaliza falha, retorna BadRequest.
                // As notificações já estão dentro do objeto response.
                return BadRequest(response);
            }
        }
    }
}
