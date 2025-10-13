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
# Define o WORKDIR diretamente para a pasta do projeto Angular
WORKDIR /src/src/BolaoOnlineAppV5

# Copia e instala as dependências
COPY ["package.json", "package-lock.json", "./"]
RUN npm install

# Faz o build do frontend. Os arquivos vão para /src/src/BolaoOnlineAppV5/dist/angular-app/browser
COPY . .
# A saída final do Angular será colocada em /app/dist/angular-app/browser
RUN npm run build -- --output-path=/app/dist/angular-app/browser

# Estágio 3: Imagem de Produção Final
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copia os arquivos publicados do backend para a imagem final
COPY --from=backend-build /app/publish ./

# Remove a pasta wwwroot padrão gerada pelo .NET
# ESSENCIAL para garantir que o contêiner não tenha arquivos antigos ou de templates
RUN rm -rf wwwroot

# Copia a pasta de build do Angular para a wwwroot final
# COPIA O CONTEÚDO (usando a sintaxe 'pasta/.' para evitar erros de caminho no Linux)
COPY --from=frontend-build /app/dist/angular-app/browser/. ./wwwroot

# Expõe a porta e define o ponto de entrada
EXPOSE 80
ENTRYPOINT ["dotnet", "ApostasApp.Core.Web.dll"]
