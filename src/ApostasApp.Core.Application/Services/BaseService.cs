// Exemplo: ApostasApp.Core.Application.Services.Base\BaseService.cs
using ApostasApp.Core.Domain.Interfaces.Notificacoes;
using ApostasApp.Core.Domain.Interfaces;
using ApostasApp.Core.Domain.Models.Notificacoes; // Para Notificacao (entidade de domínio)
using ApostasApp.Core.Application.Services.Interfaces; // Para INotifiableService
using System.Collections.Generic;
using System.Linq; // Para .ToList() e .Select()
using System.Threading.Tasks; // Para Task

namespace ApostasApp.Core.Application.Services.Base
{
    // <<-- CORRIGIDO: BaseService agora implementa INotifiableService -->>
    public abstract class BaseService : INotifiableService
    {
        private readonly INotificador _notificador;
        private readonly IUnitOfWork _uow;

        protected BaseService(INotificador notificador, IUnitOfWork uow)
        {
            _notificador = notificador;
            _uow = uow;
        }

        protected void Notificar(string tipo, string mensagem, string codigo = null, string nomeCampo = null)
        {
            _notificador.Handle(new Notificacao(codigo, tipo, mensagem, nomeCampo));
        }

        protected void Notificar(Notificacao notificacao) // Sobrecarga para Notificacao de domínio
        {
            _notificador.Handle(notificacao);
        }

        // <<-- CORRIGIDO: Métodos da interface agora são públicos -->>
        public bool TemNotificacao()
        {
            return _notificador.TemNotificacao();
        }

        // <<-- CORRIGIDO: Métodos da interface agora são públicos -->>
        public IEnumerable<NotificationDto> ObterNotificacoesParaResposta()
        {
            return _notificador.ObterNotificacoes()
                               .Select(n => new NotificationDto(n.Codigo, n.Tipo, n.Mensagem, n.NomeCampo))
                               .ToList();
        }

        protected async Task<bool> CommitAsync()
        {
            if (_notificador.TemNotificacao()) return false;

            if (await _uow.CommitAsync()) return true;

            _notificador.Handle(new Notificacao("ERRO_PERSISTENCIA", "Erro", "Não foi possível persistir os dados."));
            return false;
        }
    }
}
