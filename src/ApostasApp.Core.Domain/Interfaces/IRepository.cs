using System.Linq.Expressions;


namespace ApostasApp.Core.Domain.Interfaces
{
    public interface IRepository<TEntity> : IDisposable where TEntity : class
    {
        Task Adicionar(TEntity entity);
        Task<TEntity> ObterPorId(Guid id);
        Task<IEnumerable<TEntity>> ObterTodos();
        Task Atualizar(TEntity entity);
        Task Remover(TEntity entity);

        // Retorna IQueryable<TEntity> para permitir encadeamento de LINQ (Include, Where, OrderBy, etc.)
        IQueryable<TEntity> Buscar(Expression<Func<TEntity, bool>> predicate);

        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);
    }
}
