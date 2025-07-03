// Localização sugerida: ApostasApp.Core.Application.Services.Interfaces/INotifiableService.cs
// Esta interface define os métodos de notificação que seus serviços devem expor.

using ApostasApp.Core.Domain.Models.Notificacoes; // Ajuste o namespace para NotificationDto


namespace ApostasApp.Core.Application.Services.Interfaces
{
    public interface INotifiableService
    {
        // Métodos de notificação que espelham o INotificador e o BaseController
        // Observe que ObterNotificacoes() do INotificador retorna List<Notificacao>
        // mas ObterNotificacoesParaResposta() do BaseController retorna IEnumerable<NotificationDto>.
        // Vamos manter o ObterNotificacoesParaResposta() aqui para consistência com o frontend.
        bool TemNotificacao();
        IEnumerable<NotificationDto> ObterNotificacoesParaResposta();
    }
}
