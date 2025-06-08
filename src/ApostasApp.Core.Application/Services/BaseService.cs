using ApostasApp.Core.Domain.Interfaces;
using ApostasApp.Core.Domain.Interfaces.Notificacoes;
using ApostasApp.Core.Domain.Models.Notificacoes;
using FluentValidation;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApostasApp.Core.Application.Services.Base
{
    public abstract class BaseService
    {
        private readonly INotificador _notificador;
        protected readonly IUnitOfWork _uow;

        protected BaseService(INotificador notificador, IUnitOfWork uow)
        {
            _notificador = notificador;
            _uow = uow;
        }

        protected void Notificar(string tipo, string mensagem, string nomeCampo = null)
        {
            _notificador.Handle(tipo, mensagem);
        }

        protected void Notificar(Notificacao notificacao)
        {
            _notificador.Handle(notificacao);
        }

        protected void Notificar(List<Notificacao> notificacoes)
        {
            _notificador.Handle(notificacoes);
        }

        protected bool TemNotificacao()
        {
            return _notificador.TemNotificacao();
        }

        protected async Task<bool> Commit()
        {
            if (TemNotificacao())
            {
                return false;
            }

            if (await _uow.CommitAsync())
            {
                return true;
            }

            Notificar("Erro", "Houve um erro ao salvar os dados no banco de dados. Tente novamente mais tarde.");
            return false;
        }

        protected bool ExecutarValidacao<TValidator, TEntity>(TValidator validator, TEntity entity)
            where TValidator : AbstractValidator<TEntity>
            where TEntity : class
        {
            var validationResult = validator.Validate(entity);
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    Notificar("Erro", error.ErrorMessage, error.PropertyName);
                }
                return false;
            }
            return true;
        }
    }
}
