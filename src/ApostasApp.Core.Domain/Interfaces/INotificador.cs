// Localização: ApostasApp.Core.Domain/Interfaces/Notificacoes/INotificador.cs

using ApostasApp.Core.Domain.Models.Notificacoes; // Usar Notificacao
using System.Collections.Generic;

namespace ApostasApp.Core.Domain.Interfaces.Notificacoes
{
    public interface INotificador
    {
        bool TemNotificacao();
        List<Notificacao> ObterNotificacoes(); // Mudar para Notificacao
        void Handle(Notificacao notificacao); // Mudar para Notificacao
        void LimparNotificacoes(); // Adicionar este método à interface, se já não estiver
    }
}
