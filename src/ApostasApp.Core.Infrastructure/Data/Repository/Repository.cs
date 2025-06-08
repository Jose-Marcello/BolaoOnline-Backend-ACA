using ApostasApp.Core.Domain.Interfaces; // Para IRepository
using ApostasApp.Core.InfraStructure.Data.Context; // Para MeuDbContext
using Microsoft.EntityFrameworkCore; // Para DbSet, AsNoTracking, Where, FirstOrDefaultAsync, ToListAsync, CountAsync
using System;
using System.Collections.Generic;
using System.Linq; // Para IQueryable, Where
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ApostasApp.Core.Infrastructure.Data.Repository
{
    // Classe abstrata que implementa a interface IRepository<TEntity>
    public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly MeuDbContext Db;
        protected readonly DbSet<TEntity> DbSet;

        protected Repository(MeuDbContext context)
        {
            Db = context;
            DbSet = context.Set<TEntity>();
        }

        // Implementação do método Buscar que retorna IQueryable para consultas flexíveis
        public IQueryable<TEntity> Buscar(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.AsNoTracking().Where(predicate);
        }

        // Métodos CRUD básicos, marcados como virtual para permitir sobrescrita em repositórios específicos
        public virtual async Task Adicionar(TEntity entity)
        {
            DbSet.Add(entity);
        }

        public virtual async Task<TEntity> ObterPorId(Guid id)
        {
            // FindAsync é otimizado para buscar por chave primária
            return await DbSet.FindAsync(id);
        }

        public virtual async Task<IEnumerable<TEntity>> ObterTodos()
        {
            return await DbSet.AsNoTracking().ToListAsync();
        }

        public virtual async Task Atualizar(TEntity entity)
        {
            DbSet.Update(entity);
        }

        public virtual async Task Remover(TEntity entity)
        {
            DbSet.Remove(entity);
        }

        // Implementação do método CountAsync
        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await DbSet.CountAsync(predicate);
        }

        // Implementação do método Dispose da interface IDisposable
        public void Dispose()
        {
            Db?.Dispose();
            GC.SuppressFinalize(this); // Opcional: para GC não chamar finalizador se Dispose já foi chamado
        }
    }
}
