using ApostasApp.Core.Domain.Models.Base;
using ApostasApp.Core.Domain.Models.Interfaces;

namespace ApostasApp.Core.Domain.Models
{
    public interface IUnitOfWork : IDisposable
    {
        //IDbContextTransaction BeginTransaction();
        /// <summary>
        /// void BeginTransaction();
        /// </summary>
        //void Commit();
        void Rollback();
        // Adicione métodos para acessar os repositórios
        Task<int> SaveChanges();
        int GetTransactionHashCode();
        IRepository<TEntity> GetRepository<TEntity>() where TEntity : Entity, new();

        // Adicione esta propriedade para expor o DbContext
        //MeuDbContext DbContext { get; }
        void DetachEntity<TEntity>(TEntity entity) where TEntity : class;


    }
}