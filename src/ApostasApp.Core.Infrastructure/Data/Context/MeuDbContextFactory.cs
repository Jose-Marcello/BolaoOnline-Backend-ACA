using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration; // Necessário para IConfiguration
using System.IO; // Necessário para Path
using System; // Necessário para InvalidOperationException

// *** Mantenha o MESMO namespace do seu MeuDbContext ***
namespace ApostasApp.Core.Infrastructure.Data.Context
{
    // Esta classe é usada apenas pelas ferramentas do Entity Framework Core
    // para criar uma instância do DbContext em tempo de design (para migrações).
    public class MeuDbContextFactory : IDesignTimeDbContextFactory<MeuDbContext>
    {
        public MeuDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MeuDbContext>();

            // Este é um método simplificado e robusto para o IDesignTimeDbContextFactory.
            // Quando Add-Migration é executado com -StartupProject, as ferramentas do EF Core
            // **já carregam a configuração do projeto de startup**.
            // Portanto, a fábrica não precisa (e geralmente não deve) tentar ler o appsettings.json por si mesma.
            // Ela só precisa fornecer uma maneira de criar o DbContext.

            // A string de conexão abaixo é um placeholder para que o EF Core
            // consiga instanciar o DbContext em tempo de design para inspecionar o modelo.
            // ELA NÃO SERÁ USADA EM TEMPO DE EXECUÇÃO PELA SUA APLICAÇÃO.
            // Use uma string de conexão válida para seu SQL Server.
            //optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=ApostasAppDb;Trusted_Connection=True;MultipleActiveResultSets=true");
            optionsBuilder.UseSqlServer("Server=DESKTOP-1VSTFVA\\SQLEXPRESS;Database=BolaoLocalV2;User Id=sa;Password=Teste@123;MultipleActiveResultSets=true;TrustServerCertificate=True;MultipleActiveResultSets=true");
            return new MeuDbContext(optionsBuilder.Options);
        }
    }
}
