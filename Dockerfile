# Estágio de build do Angular
FROM node:18 AS frontend-build
WORKDIR /app
COPY src/ApostasApp.Core.Web/BolaoOnlineAppV5/package.json ./
RUN npm install
COPY src/ApostasApp.Core.Web/BolaoOnlineAppV5/ ./
RUN npm run build -- --output-path=dist/browser --base-href=/

# Estágio de build do .NET
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS backend-build
WORKDIR /app
# Copia apenas a pasta src, que contém todos os projetos
COPY src ./src
# Restaura as dependências do projeto de web
WORKDIR "/app/src/ApostasApp.Core.Web"
RUN dotnet restore
# Compila e publica a aplicação
RUN dotnet publish -c Release -o /app/publish

# Estágio final (runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
# Copia os arquivos do backend publicados
COPY --from=backend-build /app/publish .
# Copia os arquivos do frontend compilados
COPY --from=frontend-build /app/dist/browser/ ./wwwroot/
ENTRYPOINT ["dotnet", "ApostasApp.Core.Web.dll"]