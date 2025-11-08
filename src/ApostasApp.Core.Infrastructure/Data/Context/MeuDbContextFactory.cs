using ApostasApp.Core.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace ApostasApp.Core.Infrastructure // <--- Confirme o Namespace Correto
{
  public class MeuDbContextFactory : IDesignTimeDbContextFactory<MeuDbContext>
  {
    public MeuDbContext CreateDbContext(string[] args)
    {
      var optionsBuilder = new DbContextOptionsBuilder<MeuDbContext>();

      // 1. Configuração do AppSettings para ler a string de conexão
      // Nota: Adicione 'src/ApostasApp.Core.Web/' ao SetBasePath se o appsettings estiver lá.
      var configuration = new ConfigurationBuilder()
          .SetBasePath(Directory.GetCurrentDirectory())
          .AddJsonFile("appsettings.json", optional: true) // Ler o principal (se existir)
          .AddJsonFile("appsettings.Development.json", optional: true)
          .Build();

      // Tenta obter a string de conexão (que deve ser formatada para PostgreSQL)
      var connectionString = configuration.GetConnectionString("DefaultConnection");

      // 2. Se a string de conexão não for encontrada no appsettings.json, use um placeholder para o EF inspecionar o modelo.
      if (string.IsNullOrEmpty(connectionString))
      {
        // ATENÇÃO: Esta é apenas uma string PLACEHOLDER. O EF Core vai carregar a string do projeto de startup em tempo real.
        // Mas, o builder precisa de uma string inicial. Usamos a estrutura básica do Postgres.
        connectionString = "Host=localhost;Database=postgres_temp;Username=postgres;Password=123";
      }

      // 3. << CORREÇÃO CRÍTICA: USAR NPGSQL (POSTGRESQL) >>
      //optionsBuilder.UseNpgsql(connectionString);

      return new MeuDbContext(optionsBuilder.Options);
    }
  }
}
