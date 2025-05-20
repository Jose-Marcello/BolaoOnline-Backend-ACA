using ApostasApp.Core.Domain.Models.Interfaces;

namespace ApostasApp.Core.Domain.Models.Notificacoes{
    public class Notificador : INotificador
    {
        private List<Notificacao> _notificacoes;

        public Notificador()
        {
            _notificacoes = new List<Notificacao>();
        }
        public void Handle(Notificacao notificacao)
        {
            _notificacoes.Add(notificacao);
        }
        public List<Notificacao> ObterNotificacoes()
        {
            return _notificacoes;
        }
        public bool TemNotificacao()
        {
            return _notificacoes.Any();
        }
    }
}