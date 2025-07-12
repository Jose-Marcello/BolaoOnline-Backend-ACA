// Localização: ApostasApp.Core.Application.Services.Base/BaseService.cs

using ApostasApp.Core.Domain.Interfaces;
using ApostasApp.Core.Domain.Interfaces.Notificacoes;
using ApostasApp.Core.Domain.Models.Notificacoes;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApostasApp.Core.Application.Services.Base
{
    public abstract class BaseService
    {
        private readonly INotificador _notificador;
        private readonly IUnitOfWork _uow;

        protected BaseService(INotificador notificador, IUnitOfWork uow)
        {
            _notificador = notificador;
            _uow = uow;
        }

        // Métodos protegidos para notificação, acessíveis por classes filhas
        protected void Notificar(NotificationDto notification)
        {
            _notificador.Handle(notification);
        }

        protected void Notificar(string tipo, string mensagem)
        {
            _notificador.Handle(new NotificationDto { Tipo = tipo, Mensagem = mensagem });
        }

        protected IEnumerable<NotificationDto> ObterNotificacoesParaResposta()
        {
            return _notificador.ObterNotificacoes();
        }

        // Método protegido para commit da Unit of Work
        protected async Task<bool> CommitAsync()
        {
            if (_notificador.TemNotificacao()) return false;
            return await _uow.CommitAsync();
        }
    }
}
