# --- Estágio 1: Build (Compilação) ---
# Baseia-se na imagem SDK mais recente para compilação
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia o arquivo de solução
# (Assumindo que BolaoOnlineProject.sln está na mesma pasta que o Dockerfile)
COPY ["BolaoOnlineProject.sln", "./"]

# Copia os arquivos .csproj dos projetos necessários (melhora o cache do Docker)
COPY ["src/ApostasApp.Core.Web/ApostasApp.Core.Web.csproj", "src/ApostasApp.Core.Web/"]
COPY ["src/ApostasApp.Core.Application/ApostasApp.Core.Application.csproj", "src/ApostasApp.Core.Application/"]
# Adicione outros projetos core/infrastructure aqui...

# Restaura dependências
RUN dotnet restore "BolaoOnlineProject.sln"

# Copia o código-fonte restante
COPY . .

# Move para o diretório do projeto API
WORKDIR /src/src/ApostasApp.Core.Web

# Publica a aplicação
RUN dotnet publish "ApostasApp.Core.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false


# --- Estágio 2: Imagem de Produção Final (Runtime) ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copia os arquivos publicados do estágio 'build'
COPY --from=build /app/publish .

# ** CRÍTICO PARA ACA: Define a porta de escuta interna como 8080 **
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Comando de entrada
ENTRYPOINT ["dotnet", "ApostasApp.Core.Web.dll"]
