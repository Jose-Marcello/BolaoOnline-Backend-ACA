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
        protected void Notificar(string tipo, string mensagem, string nomeCampo = null)
        {
            _notificador.Handle(new Notificacao(null, tipo, mensagem, nomeCampo));
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
            // A lógica de TemNotificacao() no Notificador.cs foi ajustada para verificar apenas "Erro".
            if (_notificador.TemNotificacao())
            {
                return false;
            }

            // Realiza o commit da UnitOfWork.
            // Se o commit falhar, uma exceção será lançada e capturada pelo bloco catch no serviço chamador.
            return await _uow.CommitAsync();
        }

        // Método para limpar as notificações do notificador.
        protected void LimparNotificacoes()
        {
            _notificador.LimparNotificacoes();
        }
    }
}
