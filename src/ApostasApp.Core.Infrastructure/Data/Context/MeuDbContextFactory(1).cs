using ApostasApp.Core.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

// ESTA CLASSE É USADA PELO COMANDO 'dotnet ef' PARA SABER COMO CONSTRUIR O DbContext
// EM TEMPO DE PROJETO (design time), POIS ELE NÃO TEM ACESSO ÀS VARIÁVEIS DE AMBIENTE DO AZURE.
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<MeuDbContext>
{
  public MeuDbContext CreateDbContext(string[] args)
  {
    var builder = new DbContextOptionsBuilder<MeuDbContext>();

    // --- CONFIGURAÇÃO DE CONEXÃO DE DESIGN-TIME ---
    // Aqui, usamos uma string de conexão LOCAL/MOCK. 
    // A string real do Azure será usada no deploy.
    // O valor exato da string aqui NÃO IMPORTA, 
    // mas o PROVEDOR (UseSqlServer) deve ser o mesmo.
    var connectionString = "Server=(localdb)\\mssqllocaldb;Database=DesignDB;Trusted_Connection=True;MultipleActiveResultSets=true";

    builder.UseSqlServer(connectionString);

    return new MeuDbContext(builder.Options);
  }
}
