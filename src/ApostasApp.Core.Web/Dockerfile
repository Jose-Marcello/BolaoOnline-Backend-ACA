#------------------------------------------------------------------
# Estágio 1: Build da Aplicação ASP.NET Core (BUILD) - APENAS COMPILAÇÃO
#------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia e restaura o projeto .NET
COPY ["ApostasApp.Core.sln", "./"]
COPY ["src/ApostasApp.Core.Web/ApostasApp.Core.Web.csproj", "src/ApostasApp.Core.Web/"]
COPY ["src/ApostasApp.Core.Application/ApostasApp.Core.Application.csproj", "src/ApostasApp.Core.Application/"]
COPY ["src/ApostasApp.Core.Domain/ApostasApp.Core.Domain.csproj", "src/ApostasApp.Core.Domain/"]
COPY ["src/ApostasApp.Core.Infrastructure/ApostasApp.Core.Infrastructure.csproj", "src/ApostasApp.Core.Infrastructure/"]

# Garante o restore
RUN dotnet restore ApostasApp.Core.sln

# Copia o código-fonte e publica
COPY . .

# Move para o diretório do projeto API
WORKDIR /src/src/ApostasApp.Core.Web

# Publica a aplicação
RUN dotnet publish "ApostasApp.Core.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false



#------------------------------------------------------------------
# Estágio 2: Imagem de Produção Final (RUNTIME) - MIGRATIONS + HEROKU
#------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# [ADICIONAL CRÍTICO] Instala bibliotecas nativas e dotnet-ef para o MIGRATION SCRIPT
# Instala libs nativas para PostgreSQL
RUN apt-get update && apt-get install -y libpq-dev \
    && rm -rf /var/lib/apt/lists/*
    
# INSTALA A FERRAMENTA EF CORE (AGORA NO RUNTIME, ONDE É NECESSÁRIO PARA O SCRIPT)
RUN dotnet tool install --global dotnet-ef --version 8.0.4

# Copia os arquivos publicados do backend para a imagem final
COPY --from=build /app/publish ./

# Copia o script de startup e dá permissão
COPY entrypoint.sh .
RUN chmod +x entrypoint.sh

# [AJUSTE HEROKU] Define a porta 8080 (Apenas uma vez)
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Mudar o ponto de entrada para o script customizado
ENTRYPOINT ["./entrypoint.sh"]
