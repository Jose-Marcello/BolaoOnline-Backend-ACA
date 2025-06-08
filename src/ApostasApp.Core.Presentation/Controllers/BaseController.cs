using ApostasApp.Core.Domain.Interfaces.Notificacoes;
using ApostasApp.Core.Domain.Models.Notificacoes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; // Adicionado para Task (se necessário)

namespace ApostasApp.Web.Controllers
{
    /// <summary>
    /// Classe base abstrata para todos os controladores da aplicação.
    /// Fornece funcionalidades comuns como acesso ao Notificador e tratamento de notificações.
    /// </summary>
    public abstract class BaseController : ControllerBase // Alterado para ControllerBase para APIs puras
    {
        protected readonly INotificador Notificador;

        protected BaseController(INotificador notificador)
        {
            Notificador = notificador;
        }

        /// <summary>
        /// Adiciona uma notificação de erro com uma mensagem específica.
        /// </summary>
        /// <param name="mensagem">A mensagem de erro.</param>
        protected void NotificarErro(string mensagem)
        {
            Notificador.Handle(new Notificacao("Erro", mensagem));
        }

        /// <summary>
        /// Adiciona uma notificação com um tipo (ex: "Alerta", "Sucesso", "Erro") e uma mensagem.
        /// </summary>
        /// <param name="tipo">O tipo da notificação (ex: "Alerta").</param>
        /// <param name="mensagem">A mensagem da notificação.</param>
        protected void Notificar(string tipo, string mensagem)
        {
            Notificador.Handle(new Notificacao(tipo, mensagem));
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
        /// Obtém todas as notificações registradas.
        /// </summary>
        /// <returns>Uma coleção de objetos Notificacao.</returns>
        protected IEnumerable<Notificacao> ObterTodasNotificacoes()
        {
            return Notificador.ObterNotificacoes();
        }

        /// <summary>
        /// Retorna uma resposta padronizada para APIs, incluindo notificações e erros do ModelState.
        /// </summary>
        /// <param name="result">O objeto de resultado a ser retornado no corpo da resposta.</param>
        /// <returns>Um IActionResult com o status HTTP apropriado.</returns>
        protected ActionResult CustomResponse(object result = null)
        {
            if (TemNotificacao())
            {
                // Se houver notificações de erro, retorna BadRequest com as notificações.
                // Pode ser mais específico se Notificador.ObterNotificacoes() contiver tipos de notificação.
                return BadRequest(new ValidationProblemDetails(ModelState)
                {
                    // Você pode adicionar as notificações do Notificador aqui para o frontend
                    Detail = "Ocorreram erros na requisição.",
                    Extensions = { { "notifications", Notificador.ObterNotificacoes() } }
                });
            }

            if (result == null)
            {
                return NoContent(); // 204 No Content para operações sem retorno específico
            }

            return Ok(result); // 200 OK com o resultado
        }

        /// <summary>
        /// Retorna uma resposta padronizada para APIs quando há erros de validação do ModelState.
        /// </summary>
        /// <param name="modelState">O ModelState atual do controlador.</param>
        /// <returns>Um BadRequest com os erros do ModelState.</returns>
        protected ActionResult CustomResponse(ModelStateDictionary modelState)
        {
            // Adiciona todos os erros do ModelState ao Notificador
            var errors = modelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                Notificar("Erro", error.ErrorMessage);
            }
            return CustomResponse(); // Reutiliza o método principal para retornar o BadRequest com as notificações
        }

        // Este método é um filtro de ação do MVC e não deve ser declarado aqui se o Controller for ControllerBase.
        // Ele não é necessário para APIs puras que não renderizam Views.
        // public override void OnActionExecuting(ActionExecutingContext context)
        // {
        //     ViewBag.Notificacoes = Notificador.ObterNotificacoes();
        //     if (TemNotificacao())
        //     {
        //         foreach (var notificacao in Notificador.ObterNotificacoes())
        //         {
        //             if (!string.IsNullOrEmpty(notificacao.NomeCampo))
        //             {
        //                 ModelState.AddModelError(notificacao.NomeCampo, notificacao.Mensagem);
        //             }
        //             else
        //             {
        //                 ModelState.AddModelError(string.Empty, notificacao.Mensagem);
        //             }
        //         }
        //     }
        //     base.OnActionExecuting(context);
        // }
    }
}
