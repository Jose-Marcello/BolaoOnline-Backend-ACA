#------------------------------------------------------------------
# Estágio 1: Build da Aplicação ASP.NET Core (BUILD)
#------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia e restaura o projeto .NET (incluindo o arquivo de solução)
COPY ["ApostasApp.Core.sln", "./"]
COPY ["src/ApostasApp.Core.Web/ApostasApp.Core.Web.csproj", "src/ApostasApp.Core.Web/"]
COPY ["src/ApostasApp.Core.Application/ApostasApp.Core.Application.csproj", "src/ApostasApp.Core.Application/"]
COPY ["src/ApostasApp.Core.Domain/ApostasApp.Core.Domain.csproj", "src/ApostasApp.Core.Domain/"]
COPY ["src/ApostasApp.Core.Infrastructure/ApostasApp.Core.Infrastructure.csproj", "src/ApostasApp.Core.Infrastructure/"]

# ADIÇÃO CRÍTICA: Instala a ferramenta EF Core (necessária para rodar as migrations)
RUN dotnet tool install --global dotnet-ef --version 8.0.4

# Garante que o restore funcione corretamente referenciando a Solution
RUN dotnet restore ApostasApp.Core.sln

# Copia o código-fonte e publica
COPY . .
# Certifique-se de que o publish use o caminho correto para o projeto Web
RUN dotnet publish "src/ApostasApp.Core.Web/ApostasApp.Core.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false


#------------------------------------------------------------------
# Estágio 2: Imagem de Produção Final (API) - CORRIGIDO PARA HEROKU/POSTGRESQL
#------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# [ADICIONAL CRÍTICO] Instala as bibliotecas de cliente PostgreSQL (libpq)
# Necessário para o driver Npgsql se conectar ao banco de dados no Linux
RUN apt-get update && apt-get install -y libpq-dev \ 
    && rm -rf /var/lib/apt/lists/*

# Copia os arquivos publicados do backend para a imagem final
COPY --from=build /app/publish ./

# [AJUSTE HEROKU] Define a porta que o Heroku espera (8080)
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Ponto de entrada
ENTRYPOINT ["dotnet", "ApostasApp.Core.Web.dll"]
