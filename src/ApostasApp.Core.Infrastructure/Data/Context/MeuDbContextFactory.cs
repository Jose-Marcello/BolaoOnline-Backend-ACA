using ApostasApp.Core.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<MeuDbContext>
{
  public MeuDbContext CreateDbContext(string[] args)
  {
    var builder = new DbContextOptionsBuilder<MeuDbContext>();

    // --- AJUSTE PARA O AZURE ---
    // Substituímos o localhost pela string que você está usando no servidor real
    var connectionString = "Host=bolaoonline-pg-serv-jmag.postgres.database.azure.com;Database=bolaoonline_db;Username=bolaoadmin;Password=BdPostgresAlem@01;Port=5432;SSL Mode=Require;Trust Server Certificate=true;";

    // Garante que o provedor Npgsql use a string do Azure
    builder.UseNpgsql(connectionString);

    return new MeuDbContext(builder.Options);
  }
}
