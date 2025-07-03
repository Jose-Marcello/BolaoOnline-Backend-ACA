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
    }
}

