using ApostasApp.Core.Domain.Interfaces; // Para IUnitOfWork
using ApostasApp.Core.Infrastructure.Data.Context;


namespace ApostasApp.Core.Infrastructure.Data.Repository
{
    /// <summary>
    /// Implementação concreta da Unidade de Trabalho, utilizando Entity Framework Core.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MeuDbContext _context;

        public UnitOfWork(MeuDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Salva todas as alterações pendentes no contexto do Entity Framework Core de forma assíncrona.
        /// </summary>
        /// <returns>True se as alterações foram salvas com sucesso, false caso contrário.</returns>
        public async Task<bool> CommitAsync() // Certifique-se de que este método está exatamente assim
        {
            // O SaveChangesAsync retorna o número de estados de entidade gravados no banco de dados.
            // Se for maior que 0, significa que houve alterações salvas.
            return await _context.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// Libera o contexto do banco de dados.
        /// </summary>
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}



/*


using ApostasApp.Core.Domain.Models.Apostadores;
using ApostasApp.Core.Domain.Models.Apostas;
using ApostasApp.Core.Domain.Models.Base;
using ApostasApp.Core.Domain.Models.Campeonatos;
using ApostasApp.Core.Domain.Models.Jogos;
using ApostasApp.Core.Domain.Models.Rodadas;
using ApostasApp.Core.Domain.Models.Usuarios;
using ApostasApp.Core.Infrastructure.Data.Repository;
using ApostasApp.Core.Infrastructure.Data.Context;
using ApostasApp.Core.Infrastructure.Data.Repository.Apostadores;
using ApostasApp.Core.Infrastructure.Data.Repository.Campeonatos;
using ApostasApp.Infrastructure.Data.Repository;
using ApostasApp.Core.Domain.Models.RankingRodadas;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using ApostasApp.Core.Domain.Interfaces;

namespace ApostasApp.Core.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MeuDbContext _context;
        private readonly UserManager<Usuario> _userManager;
        private IDbContextTransaction _transaction;
        private readonly Dictionary<Type, object> _repositories;
        private readonly ILoggerFactory _loggerFactory; // Adicione ILoggerFactory

        public UnitOfWork(MeuDbContext context,
                          UserManager<Usuario> userManager,
                          ILoggerFactory loggerFactory) // Receba ILoggerFactory
        {
            _context = context;
            _userManager = userManager;
            _repositories = new Dictionary<Type, object>();
            _loggerFactory = loggerFactory;
            //_transaction = _context.Database.BeginTransaction();
        }

        // Implemente a propriedade DbContext
        //public MeuDbContext DbContext => _context;

        *//*public void BeginTransaction()
        {
            // Transação já iniciada no construtor
            Debug.WriteLine($"Transaction iniciada (já estava ativa): {_transaction?.GetHashCode() ?? 0}");
        }*//*

        public int GetTransactionHashCode()
        {
            return _transaction?.GetHashCode() ?? 0;
        }

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : Entity, new()
        {
            if (_repositories.ContainsKey(typeof(TEntity)))
            {
                return (IRepository<TEntity>)_repositories[typeof(TEntity)];
            }

            object repositoryInstance = null;

            if (typeof(TEntity) == typeof(Apostador))
            {
                var logger = _loggerFactory.CreateLogger<ApostadorRepository>();
                repositoryInstance = new ApostadorRepository(_context, logger);
            }
            else if (typeof(TEntity) == typeof(Campeonato))
            {
                var logger = _loggerFactory.CreateLogger<CampeonatoRepository>();
                repositoryInstance = new CampeonatoRepository(_context, logger);
            }
            
            else if (typeof(TEntity) == typeof(Palpite))
            {
                var logger = _loggerFactory.CreateLogger<PalpiteRepository>();
                repositoryInstance = new PalpiteRepository(_context, logger);
            }
            else if (typeof(TEntity) == typeof(ApostaRodada))
            {
                var logger = _loggerFactory.CreateLogger<ApostaRodadaRepository>();
                repositoryInstance = new ApostaRodadaRepository(_context, logger);
            }
            else if (typeof(TEntity) == typeof(ApostadorCampeonato))
            {
                var logger = _loggerFactory.CreateLogger<ApostadorCampeonatoRepository>();
                repositoryInstance = new ApostadorCampeonatoRepository(_context, logger);
            }
            else if (typeof(TEntity) == typeof(Jogo))
            {
                var logger = _loggerFactory.CreateLogger<JogoRepository>();
                repositoryInstance = new JogoRepository(_context, logger);
            }
            else if (typeof(TEntity) == typeof(RankingRodada))
            {
                var logger = _loggerFactory.CreateLogger<RankingRodadaRepository>();
                repositoryInstance = new RankingRodadaRepository(_context, logger);
            }
            else if (typeof(TEntity) == typeof(Rodada))
            {
                var logger = _loggerFactory.CreateLogger<RodadaRepository>();
                repositoryInstance = new RodadaRepository(_context, logger);
            }
            *//*else if (typeof(TEntity) == typeof(EquipeCampeonato))
            {
                var logger = _loggerFactory.CreateLogger<EquipeCampeonatoRepository>();
                repositoryInstance = new EquipeCampeonatoRepository(_context, logger);
            }
            else if (typeof(TEntity) == typeof(Equipe))
            {
                var logger = _loggerFactory.CreateLogger<EquipeRepository>();
                repositoryInstance = new EquipeRepository(_context, logger);
            }
            
            else if (typeof(TEntity) == typeof(Estadio))
            {
                var logger = _loggerFactory.CreateLogger<EstadioRepository>();
                repositoryInstance = new EstadioRepository(_context, logger);
            }
            
            else if (typeof(TEntity) == typeof(Uf))
            {
                var logger = _loggerFactory.CreateLogger<UfRepository>();
                repositoryInstance = new UfRepository(_context, logger);
            }*//*
            else
            {
                return null;
            }

            _repositories[typeof(TEntity)] = repositoryInstance;
            return (IRepository<TEntity>)repositoryInstance;
        }


        public async Task<int> SaveChanges()
        {
            var strategy = _context.Database.CreateExecutionStrategy(); // Obtém a estratégia de repetição

            return await strategy.ExecuteAsync(async () => // Executa SaveChanges dentro da estratégia
            {
                using (var transaction = _context.Database.BeginTransaction()) // Inicia a transação AQUI
                {
                    try
                    {
                        int result = await _context.SaveChangesAsync();
                        transaction.Commit();
                        return result; // Retorna o resultado de SaveChangesAsync
                    }
                    catch (DbUpdateException ex)
                    {
                        _transaction?.Rollback(); // Rollback da transação
                        throw new Exception("Erro ao salvar alterações no banco de dados.", ex);
                    }
                    catch (ObjectDisposedException ex)
                    {
                        _transaction?.Rollback();
                        throw new Exception("Erro ao salvar alterações devido a um objeto descartado.", ex);
                    }
                    catch (Exception ex)
                    {
                        _transaction?.Rollback();
                        throw new Exception("Erro ao salvar alterações.", ex);
                    }
                    finally
                    {
                        transaction?.Dispose(); // Garante que a transação seja descartada
                    }
                }
            });
        }


        *//*
        public async Task<int> SaveChanges()
        {
            return await _context.SaveChangesAsync();
        }
        */

        /*
        public void Commit()
        {
            try
            {
                _context.SaveChanges();
                _transaction.Commit();
                Debug.WriteLine($"Transaction COMMIT: {_transaction.GetHashCode()}");
            }
            catch (DbUpdateException ex)
            {
                Debug.WriteLine($"Erro ao salvar alterações (DbUpdateException): {ex.Message}");
                Debug.WriteLine(ex);
                _transaction.Rollback();
                throw new Exception("Erro ao salvar alterações no banco de dados.", ex);
            }
            catch (ObjectDisposedException ex)
            {
                Debug.WriteLine($"Erro ao salvar alterações (ObjectDisposedException): {ex.Message}");
                Debug.WriteLine(ex);
                _transaction.Rollback();
                throw new Exception("Erro ao salvar alterações devido a um objeto descartado.", ex);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao salvar alterações: {ex.Message}");
                Debug.WriteLine(ex);
                _transaction.Rollback();
                throw new Exception("Erro ao salvar alterações.", ex);
            }
        }
        *//*


        public void Rollback()
        {
            _transaction.Rollback();
        }

        public void DetachEntity<TEntity>(TEntity entity) where TEntity : class
        {
            _context.Entry(entity).State = EntityState.Detached;
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}*/