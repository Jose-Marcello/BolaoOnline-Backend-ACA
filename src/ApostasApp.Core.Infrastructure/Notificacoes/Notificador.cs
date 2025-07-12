// Localização: ApostasApp.Core.Infrastructure/Notificacoes/Notificador.cs

using ApostasApp.Core.Domain.Interfaces.Notificacoes;
using ApostasApp.Core.Domain.Models.Notificacoes;
using System.Collections.Generic;
using System.Linq;

namespace ApostasApp.Core.Infrastructure.Notificacoes
{
    public class Notificador : INotificador
    {
        private List<NotificationDto> _notifications;

        public Notificador()
        {
            _notifications = new List<NotificationDto>();
        }

        public bool TemNotificacao()
        {
            return _notifications.Any();
        }

        public List<NotificationDto> ObterNotificacoes()
        {
            return _notifications;
        }

        public void Handle(NotificationDto notification)
        {
            _notifications.Add(notification);
        }
    }
}


/*
using ApostasApp.Core.Domain.Interfaces.Notificacoes;
using ApostasApp.Core.Domain.Models.Notificacoes;
using System.Collections.Generic;
using System.Linq;

namespace ApostasApp.Core.Application.Services.Notificacoes
{
    public class Notificador : INotificador
    {
        private List<Notificacao> _notificacoes;

        public Notificador()
        {
            _notificacoes = new List<Notificacao>();
        }

        public bool TemNotificacao()
        {
            return _notificacoes.Any();
        }

        public List<Notificacao> ObterNotificacoes()
        {
            return _notificacoes;
        }

        public void Handle(Notificacao notificacao)
        {
            _notificacoes.Add(notificacao);
        }

        public void Handle(string tipo, string mensagem)
        {
            // Assumindo que você tem um construtor Notificacao(string codigo, string tipo, string mensagem, string nomeCampo)
            // Ou que você deseja criar uma Notificacao sem código/nomeCampo aqui.
            // Vou usar um construtor que aceita tipo e mensagem, ou você pode ajustar Notificacao.
            _notificacoes.Add(new Notificacao(null, tipo, mensagem, null)); // Ajuste o construtor conforme sua Notificacao
        }

        public void Handle(List<Notificacao> notificacoes)
        {
            _notificacoes.AddRange(notificacoes);
        }

        // <<-- IMPLEMENTAÇÃO DO NOVO MÉTODO -->>
        public void LimparNotificacoes()
        {
            _notificacoes.Clear();
        }
    }
}
*/