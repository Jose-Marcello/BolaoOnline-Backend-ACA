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
# Copia todo o conteúdo do seu projeto para o contêiner
COPY . .
# Navega para a pasta do projeto de backend
WORKDIR "/app/src/ApostasApp.Core.Web"
RUN dotnet restore
RUN dotnet build -c Release -o /app/build

# Estágio de publicação
FROM backend-build AS publish
WORKDIR "/app/src/ApostasApp.Core.Web"
RUN dotnet publish -c Release -o /app/publish

# Estágio final (runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
# Copia os arquivos do backend publicados
COPY --from=publish /app/publish .
# Copia os arquivos do frontend compilados
COPY --from=frontend-build /app/dist/browser/ ./wwwroot/
ENTRYPOINT ["dotnet", "ApostasApp.Core.Web.dll"]