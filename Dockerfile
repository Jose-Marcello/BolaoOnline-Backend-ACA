#------------------------------------------------------------------
# Estágio 1: Build da Aplicação ASP.NET Core (Estabilizado e Prévio)
#------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS final

# Define o WORKDIR como a RAIZ do container
WORKDIR /app

# 1. Copia o arquivo de solução e os arquivos de projeto .NET
# Os caminhos estão corretos porque o contexto de build é a raiz do repositório
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
# Estágio de Produção Final (Contém a Correção da wwwroot)
#------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
# Define o WORKDIR para a pasta onde a aplicação será executada
WORKDIR /app

# Copia os binários publicados do backend
COPY --from=final /app/publish ./

# CORREÇÃO CRUCIAL:
# A pasta wwwroot precisa estar no mesmo nível que o arquivo .dll publicado.
# O comando a seguir copia a pasta wwwroot (que contém o index.html e os assets)
# da sua estrutura de código fonte para o diretório de trabalho do contêiner.
# O caminho de origem é relativo à raiz do repositório (onde o Dockerfile está).
COPY ["./src/ApostasApp.Core.Web/wwwroot", "./wwwroot"]

# Define a porta de escuta padrão do contêiner como 80, que é a porta que o App Service espera para HTTP
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

# Comando de inicialização
ENTRYPOINT ["dotnet", "ApostasApp.Core.Web.dll"]