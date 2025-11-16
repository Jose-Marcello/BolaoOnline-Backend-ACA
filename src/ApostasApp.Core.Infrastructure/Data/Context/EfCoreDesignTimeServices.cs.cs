using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.Extensions.DependencyInjection;
using Npgsql.EntityFrameworkCore.PostgreSQL.Design;
using Npgsql.EntityFrameworkCore.PostgreSQL.Scaffolding.Internal; // Importante para o Npgsql

/*
namespace ApostasApp.Core.Infrastructure
{
  // Esta classe é um ponto de entrada especial para o 'dotnet ef'
  // Ele força o EF Core a usar as configurações corretas de provedor
  // durante a execução de comandos como Add-Migration.
  public class EfCoreDesignTimeServices : IDesignTimeServices
  {
    public void ConfigureDesignTimeServices(IServiceCollection services)
    {
      // Adiciona o provedor Npgsql de design-time
      services.AddSingleton<IProviderCodeGenerator, NpgsqlCodeGenerator>();
    }
  }
}
*/
