using ApostasApp.Core.Domain.Interfaces.Apostadores;
using ApostasApp.Core.Domain.Models;
using ApostasApp.Core.Domain.Models.Apostadores;
using ApostasApp.Core.Domain.Models.Interfaces;
using ApostasApp.Core.Domain.Services.Apostadores;
using ApostasApp.Core.Domain.Validations;
using Microsoft.EntityFrameworkCore;

namespace ApostasApp.Core.InfraStructure.Services.Apostadores
{
    public class ApostadorService : BaseService, IApostadorService
    {
        private readonly IUnitOfWork _uow;

        public ApostadorService(IUnitOfWork uow, INotificador notificador) : base(notificador)
        {
            _uow = uow;
        }


        public async Task Adicionar(Apostador apostador)
        {

            try
            {
                if (!ExecutarValidacao(new ApostadorValidation(), apostador))
                {
                    return;
                }
               
                //var apostadorRepository = _uow.GetRepository<Apostador>() as ApostadorRepository;
                //await apostadorRepository.Adicionar(apostador);
                await _uow.GetRepository<Apostador>().Adicionar(apostador);

            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Erro ao adicionar APOSTADOR (DbUpdateException): {ex.Message}");
                // Registre a exceção completa para depuração
                Console.WriteLine(ex);
                // Verifique se há erros de validação de dados ou problemas de integridade referencial
                // Notifique o usuário sobre o problema
                _uow.Rollback();
            }
            catch (ObjectDisposedException ex)
            {
                Console.WriteLine($"Erro ao adicionar APOSTADOR (ObjectDisposedException): {ex.Message}");
                // Registre a exceção completa para depuração
                Console.WriteLine(ex);
                // Verifique se o DbContext ou a UnitOfWork foram descartados prematuramente
                // Notifique o usuário sobre o problema
                _uow.Rollback();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao adicionar APOSTADOR: {ex.Message}");
                // Registre a exceção completa para depuração
                Console.WriteLine(ex);
                // Notifique o usuário sobre o problema
                _uow.Rollback();


            }
        }



        public async Task Atualizar(Apostador apostador)
        {
            if (!ExecutarValidacao(new ApostadorValidation(), apostador)) return;
                       
            //var apostadorRepository = _uow.GetRepository<Apostador>() as ApostadorRepository;
            //await apostadorRepository.Atualizar(apostador);
            await _uow.GetRepository<Apostador>().Atualizar(apostador);

        }

        public async Task Remover(Guid id)
        {
            //var apostadorRepository = _uow.GetRepository<Apostador>() as ApostadorRepository;
            //await apostadorRepository.Remover(id);
            await _uow.GetRepository<Apostador>().Remover(id);

        }

        public void Dispose()
        {
            // _uow.Apostadores?.Dispose();
        }
    }
}