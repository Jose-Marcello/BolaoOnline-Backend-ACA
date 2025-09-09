using System.Threading.Tasks;

namespace ApostasApp.Core.Domain.Interfaces
{
    /// <summary>
    /// Define o contrato para a Unidade de Trabalho (Unit of Work),
    /// responsável por encapsular as operações de banco de dados em uma única transação.
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Salva todas as alterações pendentes na unidade de trabalho de forma assíncrona.
        /// </summary>
        /// <returns>True se as alterações foram salvas com sucesso, false caso contrário.</returns>
        Task<bool> CommitAsync(); // Este método é crucial e deve estar aqui

        /// <summary>
        /// Descarta a unidade de trabalho e libera os recursos.
        /// </summary>
        void Dispose();
    }
}
