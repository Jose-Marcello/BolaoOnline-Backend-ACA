using ApostasApp.Core.Domain.Models.Notificacoes;
using System.Collections.Generic;

namespace ApostasApp.Core.Domain.Interfaces.Notificacoes
{
    public interface INotificador
    {
        bool TemNotificacao();
        List<Notificacao> ObterNotificacoes();
        void Handle(Notificacao notificacao);
        void Handle(string tipo, string mensagem);
        void Handle(List<Notificacao> notificacoes);
    }
}
