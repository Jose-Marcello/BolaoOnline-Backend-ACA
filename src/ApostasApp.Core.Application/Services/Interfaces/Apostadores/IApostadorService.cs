using ApostasApp.Core.Application.DTOs.Apostadores;
using ApostasApp.Core.Application.Models;
using ApostasApp.Core.Domain.Models.Apostadores; // Para a entidade Apostador
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApostasApp.Core.Application.Services.Interfaces.Apostadores
{
    public interface IApostadorService //: IBaseService // Certifique-se que herda de IBaseService
    {
        // Métodos existentes:
        Task Adicionar(Apostador apostador);
        Task Atualizar(Apostador apostador);
        Task Remover(Guid id);
        Task<Apostador> ObterPorId(Guid id);
        Task<IEnumerable<Apostador>> ObterTodos();

        // NOVO MÉTODO PARA OBTER APOSTADOR PELO ID DO USUÁRIO IDENTITY
        Task<Apostador> GetApostadorByUserIdAsync(string userId); // <<-- ADICIONE ESTA LINHA

        // <<-- NOVA ASSINATURA DE MÉTODO -->>
        /// <summary>
        /// Obtém o ID do ApostadorCampeonato para um dado usuário e campeonato.
        /// Retorna o ID se encontrado, ou null se não houver adesão.
        /// </summary>
        /// <param name="userId">ID do usuário (Auth).</param>
        /// <param name="campeonatoId">ID do campeonato.</param>
        /// <returns>ApiResponse contendo o ApostadorCampeonatoId (string) ou null.</returns>
        Task<ApiResponse<string>> ObterApostadorCampeonatoIdParaUsuarioECampeonato(string userId, Guid campeonatoId);

        Task<bool> AtualizarPerfilAsync(string userId, UpdatePerfilRequestDto request);
    }


}

