using ApostasApp.Core.Application.Services.Base;
using ApostasApp.Core.Application.Services.Interfaces; // Para BaseService
using ApostasApp.Core.Application.Services.Interfaces.Apostadores; // Para IApostadorService
using ApostasApp.Core.Application.Validations; // Para ApostadorValidation
using ApostasApp.Core.Domain.Interfaces; // Para IUnitOfWork
using ApostasApp.Core.Domain.Interfaces.Apostadores; // Para IApostadorRepository
using ApostasApp.Core.Domain.Interfaces.Notificacoes; // Para INotificador
using ApostasApp.Core.Domain.Models.Apostadores; // Para Apostador
using Microsoft.EntityFrameworkCore; // Para DbUpdateException
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApostasApp.Core.Application.Services.Apostadores
{
    /// <summary>
    /// ApostadorService é responsável pela lógica de negócio relacionada a apostadores.
    /// Ele herda de BaseService, que gerencia o IUnitOfWork e o INotificador.
    /// </summary>
    public class ApostadorService : BaseService, IApostadorService
    {
        private readonly IApostadorRepository _apostadorRepository;

        public ApostadorService(IApostadorRepository apostadorRepository,
                                INotificador notificador,
                                IUnitOfWork uow) : base(notificador, uow)
        {
            _apostadorRepository = apostadorRepository;
        }

        /// <summary>
        /// Adiciona um novo apostador.
        /// </summary>
        /// <param name="apostador">A entidade Apostador a ser adicionada.</param>
        public async Task Adicionar(Apostador apostador)
        {
            try
            {
                // CHAMA ExecutarValidacao da BaseService
                if (!ExecutarValidacao(new ApostadorValidation(), apostador))
                {
                    return;
                }

                await _apostadorRepository.Adicionar(apostador);
                await Commit(); // CHAMA Commit da BaseService
                Notificar("Sucesso", "Apostador adicionado com sucesso!");
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Erro ao adicionar APOSTADOR (DbUpdateException): {ex.Message}");
                Console.WriteLine(ex);
                Notificar("Erro", "Ocorreu um erro de banco de dados ao adicionar o apostador. Tente novamente.");
            }
            catch (ObjectDisposedException ex)
            {
                Console.WriteLine($"Erro ao adicionar APOSTADOR (ObjectDisposedException): {ex.Message}");
                Console.WriteLine(ex);
                Notificar("Erro", "Ocorreu um erro interno ao adicionar o apostador. Tente novamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao adicionar APOSTADOR: {ex.Message}");
                Console.WriteLine(ex);
                Notificar("Erro", $"Ocorreu um erro inesperado ao adicionar o apostador: {ex.Message}");
            }
        }

        /// <summary>
        /// Atualiza um apostador existente.
        /// </summary>
        /// <param name="apostador">A entidade Apostador a ser atualizada.</param>
        public async Task Atualizar(Apostador apostador)
        {
            // CHAMA ExecutarValidacao da BaseService
            if (!ExecutarValidacao(new ApostadorValidation(), apostador)) return;

            await _apostadorRepository.Atualizar(apostador);
            await Commit(); // CHAMA Commit da BaseService
            Notificar("Sucesso", "Apostador atualizado com sucesso!");
        }

        /// <summary>
        /// Remove um apostador pelo seu ID.
        /// </summary>
        /// <param name="id">O ID do apostador a ser removido.</param>
        public async Task Remover(Guid id)
        {
            var apostador = await _apostadorRepository.ObterPorId(id);
            if (apostador == null)
            {
                Notificar("Alerta", "Apostador não encontrado para remoção.");
                return;
            }

            await _apostadorRepository.Remover(apostador);
            await Commit(); // CHAMA Commit da BaseService
            Notificar("Sucesso", "Apostador removido com sucesso!");
        }

        /// <summary>
        /// Obtém um apostador pelo seu ID.
        /// </summary>
        /// <param name="id">O ID do apostador.</param>
        /// <returns>O apostador encontrado, ou null se não existir.</returns>
        public async Task<Apostador> ObterPorId(Guid id)
        {
            return await _apostadorRepository.ObterPorId(id);
        }

        /// <summary>
        /// Obtém todos os apostadores.
        /// </summary>
        /// <returns>Uma coleção de apostadores.</returns>
        public async Task<IEnumerable<Apostador>> ObterTodos()
        {
            return await _apostadorRepository.ObterTodos();
        }
        
    }
}
