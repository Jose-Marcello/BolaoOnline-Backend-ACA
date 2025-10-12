# Estágio de build do Angular
FROM node:18 AS frontend-build
# Define o diretório de trabalho para onde os arquivos de build serão copiados
WORKDIR /app

# O caminho para o package.json é agora relativo à pasta do projeto de backend
COPY src/ApostasApp.Core.Web/BolaoOnlineAppV5/package.json ./
RUN npm install
 
# Copia o restante dos arquivos do projeto Angular para o WORKDIR
COPY src/ApostasApp.Core.Web/BolaoOnlineAppV5/ ./
RUN npm run build -- --output-path=dist/browser --base-href=/

# Estágio de build do .NET (sem alterações)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS backend-build
WORKDIR /app
COPY ["src/ApostasApp.Core.Web/ApostasApp.Core.Web.csproj", "src/ApostasApp.Core.Web/"]
COPY ["src/ApostasApp.Core.Application/ApostasApp.Core.Application.csproj", "src/ApostasApp.Core.Application/"]
COPY ["src/ApostasApp.Core.Domain/ApostasApp.Core.Domain.csproj", "src/ApostasApp.Core.Domain/"]
COPY ["src/ApostasApp.Core.Infrastructure/ApostasApp.Core.Infrastructure.csproj", "src/ApostasApp.Core.Infrastructure/"]
RUN dotnet restore "src/ApostasApp.Core.Web/ApostasApp.Core.Web.csproj"
COPY . .
WORKDIR "/app/src/ApostasApp.Core.Web"
RUN dotnet build "ApostasApp.Core.Web.csproj" -c Release -o /app/build

# Estágio de publicação
FROM backend-build AS publish
RUN dotnet publish "ApostasApp.Core.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Estágio final (runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
# Copia os arquivos do frontend para a pasta wwwroot (dist/browser é o padrão do Angular)
COPY --from=frontend-build /app/dist/browser/ ./wwwroot/
ENTRYPOINT ["dotnet", "ApostasApp.Core.Web.dll"]