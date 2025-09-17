<<<<<<< HEAD
# Estágio 1: Build do Frontend (Angular)
=======
# EstÃ¡gio 1: Build do Frontend (Angular)
>>>>>>> 73bcac91235ff145e8e801a43e2936a62247066e
FROM node:18-alpine AS frontend-builder
WORKDIR /app/frontend
# Copia somente o package.json para otimizar o cache do Docker
COPY BolaoOnlineAppV5/package*.json ./
RUN npm install
# Copia o restante do projeto Angular e faz o build
COPY BolaoOnlineAppV5/ .
RUN npm run build -- --output-path=/app/dist --base-href=/

<<<<<<< HEAD
# Estágio 2: Build do Backend (ASP.NET Core)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS backend-builder
WORKDIR /app/backend
# Copia a solução e todos os projetos de uma vez
COPY . .
# Restaura todas as dependências da solução inteira
RUN dotnet restore "src/ApostaApp.Core.sln"
# Publica a aplicação Web
RUN dotnet publish "src/ApostasApp.Core.Web/ApostasApp.Core.Web.csproj" -c Release -o /app/publish

# Estágio 3: Imagem final de produção
=======
# EstÃ¡gio 2: Build do Backend (ASP.NET Core)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS backend-builder
WORKDIR /app/backend
# Copia a soluÃ§Ã£o e todos os projetos de uma vez
COPY . .
# Restaura todas as dependÃªncias da soluÃ§Ã£o inteira
RUN dotnet restore "src/ApostaApp.Core.sln"
# Publica a aplicaÃ§Ã£o Web
RUN dotnet publish "src/ApostasApp.Core.Web/ApostasApp.Core.Web.csproj" -c Release -o /app/publish

# EstÃ¡gio 3: Imagem final de produÃ§Ã£o
>>>>>>> 73bcac91235ff145e8e801a43e2936a62247066e
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
# Copia os arquivos publicados do backend
COPY --from=backend-builder /app/backend/src/ApostasApp.Core.Web/publish .
# Copia os arquivos compilados do frontend para a wwwroot
COPY --from=frontend-builder /app/frontend/dist/ ./wwwroot/
<<<<<<< HEAD
# Ponto de entrada da aplicação
=======
# Ponto de entrada da aplicaÃ§Ã£o
>>>>>>> 73bcac91235ff145e8e801a43e2936a62247066e
ENTRYPOINT ["dotnet", "ApostasApp.Core.Web.dll"]