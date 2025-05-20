using ApostasApp.Core.Domain.Models.Notificacoes;

namespace ApostasApp.Core.Domain.Models.Interfaces
{
    public interface INotificador    
    {
        bool TemNotificacao();
        List<Notificacao> ObterNotificacoes();
        void Handle(Notificacao notificacao);
    }
}