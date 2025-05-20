using ApostasApp.Core.Domain.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ApostasApp.Core.Presentation.Controllers
{
    public abstract class BaseController : Controller
    {
        private readonly INotificador _notificador;

        protected Guid UserId { get; set; }
        protected string UserName { get; set; }

        //protected BaseController(INotificador notificador,
        //                         IAppIdentityUser user)
        protected BaseController(INotificador notificador)
        {
            _notificador = notificador;

            //if (user.IsAuthenticated())
            //{
            //    UserId = user.GetUserId();
            //    UserName = user.GetUsername();
            //}
        }

        protected bool OperacaoValida()
        {
            return !_notificador.TemNotificacao();
        }
    }
}