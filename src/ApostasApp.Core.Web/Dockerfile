# Use a imagem do SDK do .NET para o estágio de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia os arquivos de configuração de dependências do backend
COPY ["src/ApostasApp.Core.Web/ApostasApp.Core.Web.csproj", "src/ApostasApp.Core.Web/"]
COPY ["src/ApostasApp.Core.Application/ApostasApp.Core.Application.csproj", "src/ApostasApp.Core.Application/"]
COPY ["src/ApostasApp.Core.Domain/ApostasApp.Core.Domain.csproj", "src/ApostasApp.Core.Domain/"]
COPY ["src/ApostasApp.Core.Infrastructure/ApostasApp.Core.Infrastructure.csproj", "src/ApostasApp.Core.Infrastructure/"]

# Copia os arquivos de configuração de dependências do frontend
COPY ["src/BolaoOnlineAppV5/package.json", "src/BolaoOnlineAppV5/package-lock.json", "src/BolaoOnlineAppV5/"]

# Copia o restante do código do projeto
COPY . .

# Restaura as dependências NuGet do backend
WORKDIR "/app/src/ApostasApp.Core.Web"
RUN dotnet restore "ApostasApp.Core.Web.csproj"

# Instala e faz o build do frontend
WORKDIR "/app/src/BolaoOnlineAppV5"
RUN npm install
RUN npm run build -- --output-path=dist/

# Publica a aplicação ASP.NET Core
WORKDIR "/app/src/ApostasApp.Core.Web"
RUN dotnet publish "ApostasApp.Core.Web.csproj" -c Release -o /app/publish

# Copia os arquivos estáticos do frontend para a pasta wwwroot do backend
# O build do Angular é feito em /app/src/BolaoOnlineAppV5/dist/
# O publish do .NET é feito em /app/publish/wwwroot
RUN cp -R "/app/src/BolaoOnlineAppV5/dist/bolao-online-app-v5/browser/." "/app/publish/wwwroot/"

# Estágio final do contêiner para a execução
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Define o ponto de entrada da aplicação
ENTRYPOINT ["dotnet", "ApostasApp.Core.Web.dll"]
