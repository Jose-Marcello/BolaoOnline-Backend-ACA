# Estágio 1: Build do Backend ASP.NET Core
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS backend-build
WORKDIR /src

# Copia e restaura o projeto .NET
COPY ["ApostasApp.Core.sln", "./"]
COPY ["src/ApostasApp.Core.Web/ApostasApp.Core.Web.csproj", "src/ApostasApp.Core.Web/"]
RUN dotnet restore

# Copia o código-fonte e publica
COPY . .
RUN dotnet publish "src/ApostasApp.Core.Web/ApostasApp.Core.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Estágio 2: Build do Frontend Angular (em um ambiente Node.js)
FROM node:18-alpine AS frontend-build
WORKDIR /src

# Copia o código do frontend e instala as dependências
COPY ["src/BolaoOnlineAppV5/package.json", "src/BolaoOnlineAppV5/package-lock.json", "src/BolaoOnlineAppV5/"]
WORKDIR /src/src/BolaoOnlineAppV5
RUN npm install

# Faz o build do frontend
COPY . .
RUN npm run build -- --output-path=/app/dist/angular-app/browser

# Estágio 3: Imagem de Produção Final
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copia os arquivos publicados do backend para a imagem final
COPY --from=backend-build /app/publish ./

# Remove a pasta wwwroot padrão gerada pelo .NET
RUN rm -rf wwwroot

# Copia a pasta de build do Angular para a wwwroot final
COPY --from=frontend-build /src/src/BolaoOnlineAppV5/dist/angular-app/browser ./wwwroot

# Expõe a porta e define o ponto de entrada
EXPOSE 80
ENTRYPOINT ["dotnet", "ApostasApp.Core.Web.dll"]

# Correcao de Sincronizacao
