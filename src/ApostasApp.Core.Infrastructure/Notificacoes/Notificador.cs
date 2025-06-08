using ApostasApp.Core.Domain.Interfaces.Notificacoes;
using ApostasApp.Core.Domain.Models.Notificacoes;
using System.Collections.Generic;
using System.Linq;

namespace ApostasApp.Core.Infrastructure.Notificacoes
{
    public class Notificador : INotificador
    {
        private readonly List<Notificacao> _notifications;

        public Notificador()
        {
            _notifications = new List<Notificacao>();
        }

        public bool TemNotificacao()
        {
            return _notifications.Any();
        }

        public List<Notificacao> ObterNotificacoes()
        {
            return _notifications;
        }

        public void Handle(Notificacao notificacao)
        {
            _notifications.Add(notificacao);
        }

        public void Handle(string tipo, string mensagem)
        {
            _notifications.Add(new Notificacao(tipo, mensagem));
        }

        public void Handle(List<Notificacao> notificacoes)
        {
            _notifications.AddRange(notificacoes);
        }
    }
}
