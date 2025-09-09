// ApostasApp.Core.Application.Services.Interfaces/INotifiableService.cs
using System.Collections.Generic;
using ApostasApp.Core.Domain.Models.Notificacoes; // Para NotificationDto

namespace ApostasApp.Core.Application.Services.Interfaces
{
    public interface INotifiableService
    {
        bool TemNotificacao();
        IEnumerable<NotificationDto> ObterNotificacoesParaResposta();
    }
}
