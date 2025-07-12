// Localização: ApostasApp.Core.Domain/Interfaces/Notificacoes/INotificador.cs

using ApostasApp.Core.Domain.Models.Notificacoes;
using System.Collections.Generic;

namespace ApostasApp.Core.Domain.Interfaces.Notificacoes
{
    public interface INotificador
    {
        bool TemNotificacao();
        List<NotificationDto> ObterNotificacoes();
        void Handle(NotificationDto notification);
    }
}


/*
using ApostasApp.Core.Domain.Models.Notificacoes; // Para Notificacao e NotificationDto
using System.Collections.Generic;

namespace ApostasApp.Core.Domain.Interfaces.Notificacoes
{
    public interface INotificador
    {
        bool TemNotificacao();
        List<Notificacao> ObterNotificacoes(); // Retorna a entidade Notificacao
        void Handle(Notificacao notificacao); // Lida com a entidade Notificacao
        void Handle(string tipo, string mensagem); // Lida com a entidade Notificacao
        void Handle(List<Notificacao> notificacoes); // Lida com a entidade Notificacao
        void LimparNotificacoes(); // <<-- ADICIONADO: Novo método para limpar as notificações
    }
}
*/