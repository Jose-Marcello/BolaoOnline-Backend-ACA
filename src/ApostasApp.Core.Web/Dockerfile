# Estágio 1: Build do Frontend Angular
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copia e restaura o projeto .NET
COPY ["src/ApostasApp.Core.Web/ApostasApp.Core.Web.csproj", "src/ApostasApp.Core.Web/"]
COPY ["ApostasApp.Core.sln", "."]
RUN dotnet restore "ApostasApp.Core.sln"

# Copia e constrói o backend
COPY . ./
RUN dotnet publish "src/ApostasApp.Core.Web/ApostasApp.Core.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Estágio 2: Build do Frontend Angular (em um ambiente Node.js)
FROM node:18-alpine AS frontend-build
WORKDIR /app/frontend

# Copia o código do frontend e instala as dependências
COPY ["src/BolaoOnlineAppV5/package.json", "src/BolaoOnlineAppV5/package-lock.json", "./"]
WORKDIR /app/frontend/src/BolaoOnlineAppV5
RUN npm install

# Faz o build do frontend
COPY ["src/BolaoOnlineAppV5", "./"]
RUN npm run build -- --output-path=/app/dist/angular-app/browser

# Estágio 3: Imagem de Produção Final
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copia os arquivos publicados do backend para a imagem final
COPY --from=build-env /app/publish ./

# Remove a pasta wwwroot padrão gerada pelo .NET
RUN rm -rf wwwroot

# Copia a pasta de build do Angular para a wwwroot final
COPY --from=frontend-build /app/dist/angular-app/browser ./wwwroot

# Expõe a porta e define o ponto de entrada
EXPOSE 80
ENTRYPOINT ["dotnet", "ApostasApp.Core.Web.dll"]
