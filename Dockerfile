# Estágio 1: Build do Frontend (Angular)
FROM node:18-alpine AS frontend-builder
WORKDIR /app/frontend
# Copia somente o package.json para otimizar o cache do Docker
COPY BolaoOnlineAppV5/package*.json ./
RUN npm install
# Copia o restante do projeto Angular e faz o build
COPY BolaoOnlineAppV5/ .
RUN npm run build -- --output-path=/app/dist --base-href=/

# Estágio 2: Build do Backend (ASP.NET Core)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS backend-builder
WORKDIR /app

# Copia a solução e todos os projetos de uma vez
COPY . .
# Comando de diagnóstico: lista recursivamente todos os arquivos na pasta /app
RUN ls -R /app

# Restaura todas as dependências da solução inteira
RUN dotnet restore "ApostasApp.Core.sln"
# Publica a aplicação Web
RUN dotnet publish "src/ApostasApp.Core.Web/ApostasApp.Core.Web.csproj" -c Release -o /app/publish

# Estágio 3: Imagem final de produção
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
# Copia os arquivos publicados do backend
COPY --from=backend-builder /app/publish .
# Copia os arquivos compilados do frontend para a wwwroot
COPY --from=frontend-builder /app/frontend/dist/ ./wwwroot/
# Ponto de entrada da aplicação
ENTRYPOINT ["dotnet", "ApostasApp.Core.Web.dll"]