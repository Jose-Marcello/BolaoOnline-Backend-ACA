// Localização: ApostasApp.Core.Application.Services/BaseService.cs

using ApostasApp.Core.Domain.Interfaces.Notificacoes;
using ApostasApp.Core.Domain.Models.Notificacoes; // Usar a classe Notificacao do Domain
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApostasApp.Core.Domain.Interfaces; // Para IUnitOfWork

namespace ApostasApp.Core.Application.Services
{
    // BaseService agora é uma classe abstrata que fornece funcionalidades comuns para outros serviços.
    public abstract class BaseService
    {
        // O notificador é injetado para registrar mensagens de erro ou sucesso.
        protected readonly INotificador _notificador;
        // A UnitOfWork é injetada para gerenciar transações de banco de dados.
        protected readonly IUnitOfWork _uow; // Adicionado IUnitOfWork aqui

        // Construtor para injetar as dependências.
        protected BaseService(INotificador notificador, IUnitOfWork uow) // Adicionado IUnitOfWork ao construtor
        {
            _notificador = notificador;
            _uow = uow; // Inicializa a UnitOfWork
        }

        // Método para registrar uma notificação.
        // Ele cria uma instância de Notificacao (do domínio) e a passa para o notificador.
        // Este método é genérico e será chamado pelos métodos específicos abaixo.
        protected void Notificar(string tipo, string mensagem, string nomeCampo = null)
        {
            _notificador.Handle(new Notificacao(null, tipo, mensagem, nomeCampo));
        }

        // NOVOS MÉTODOS: Métodos específicos para notificar erros, sucessos e alertas
        protected void NotificarErro(string mensagem, string codigo = null, string nomeCampo = null)
        {
            Notificar("Erro", mensagem, nomeCampo);
        }

        protected void NotificarSucesso(string mensagem, string codigo = null, string nomeCampo = null)
        {
            Notificar("Sucesso", mensagem, nomeCampo);
        }

        protected void NotificarAlerta(string mensagem, string codigo = null, string nomeCampo = null)
        {
            Notificar("Alerta", mensagem, nomeCampo);
        }


        // Método para obter as notificações registradas no notificador.
        // Ele mapeia as notificações do domínio (Notificacao) para DTOs de notificação (NotificationDto)
        // que serão retornados na resposta da API.
        protected List<NotificationDto> ObterNotificacoesParaResposta()
        {
            // Obtém as notificações do notificador (que são do tipo Notificacao)
            // e as projeta para NotificationDto (o DTO usado na API).
            // Isso garante que a API retorne o formato esperado.
            return _notificador.ObterNotificacoes().Select(n => new NotificationDto
            {
                Codigo = n.Codigo,
                Tipo = n.Tipo,
                Mensagem = n.Mensagem,
                NomeCampo = n.NomeCampo
            }).ToList();
        }

        // Método para realizar o commit da transação.
        // Ele verifica se há notificações de erro antes de tentar commitar.
        // Se houver erros, o commit é abortado.
        protected async Task<bool> CommitAsync()
        {
            // Se o notificador tem notificações do tipo "Erro", não permite o commit.
            if (_notificador.ObterNotificacoes().Any(n => n.Tipo == "Erro")) // Verifica se há notificações de erro
            {
                return false;
            }

            // Realiza o commit da UnitOfWork.
            return await _uow.CommitAsync();
        }

        // Método para limpar as notificações do notificador.
        protected void LimparNotificacoes()
        {
            _notificador.LimparNotificacoes();
        }
    }
}
