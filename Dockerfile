#------------------------------------------------------------------
# Estágio 1: Build da Aplicação ASP.NET Core (Estabilizado e Prévio)
#------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS final

# Define o WORKDIR como a RAIZ do container
WORKDIR /app

# 1. Copia o arquivo de solução e os arquivos de projeto .NET
# Caminhos corrigidos para o ASP.NET Core
COPY ["ApostasApp.Core.sln", "./"]
COPY ["src/ApostasApp.Core.Web/ApostasApp.Core.Web.csproj", "src/ApostasApp.Core.Web/"]
COPY ["src/ApostasApp.Core.Application/ApostasApp.Core.Application.csproj", "src/ApostasApp.Core.Application/"]
COPY ["src/ApostasApp.Core.Domain/ApostasApp.Core.Domain.csproj", "src/ApostasApp.Core.Domain/"]
COPY ["src/ApostasApp.Core.Infrastructure/ApostasApp.Core.Infrastructure.csproj", "src/ApostasApp.Core.Infrastructure/"]

# Garante que o restore funcione corretamente referenciando a Solution
RUN dotnet restore ApostasApp.Core.sln

# Copia todo o restante do código-fonte
COPY . .

# 2. Publica o Backend
RUN dotnet publish "src/ApostasApp.Core.Web/ApostasApp.Core.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false


#------------------------------------------------------------------
# Estágio de Produção Final (Apenas Binários - Contém a Correção C#)
#------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
# Define o WORKDIR para a pasta onde a aplicação será executada
WORKDIR /app

# Copia os binários publicados do backend
COPY --from=final /app/publish ./

# COPIA FINAL DE ARQUIVOS ESTÁTICOS (do seu projeto local)
# Caminho corrigido: Deve incluir a pasta ApostasApp.Core.Web
COPY ["src/ApostasApp.Core.Web/wwwroot", "./wwwroot"]

# Define a porta de escuta padrão do contêiner como 80, que é a porta que o App Service espera para HTTP
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

# Comando de inicialização
ENTRYPOINT ["dotnet", "ApostasApp.Core.Web.dll"]

