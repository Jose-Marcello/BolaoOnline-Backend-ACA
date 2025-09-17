# Usa uma imagem do Node.js como base para o build do frontend
FROM node:18-alpine AS frontend-builder
WORKDIR /usr/src/app
# Copia e instala as dependências do Angular
COPY BolaoOnlineAppV5/package*.json ./
RUN npm install
# Copia o código-fonte do Angular
COPY BolaoOnlineAppV5/ .
# Executa o build do Angular e salva a saída em um diretório /dist
RUN npm run build -- --output-path=/usr/src/app/dist --base-href=/

# Usa uma imagem do SDK do .NET para o build do backend
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS backend-builder
WORKDIR /src
# Copia todos os projetos .NET para o diretório /src
COPY src/ .
# Restaura as dependências
RUN dotnet restore "ApostasApp.Core.Web/ApostasApp.Core.Web.csproj"
# Publica a aplicação
RUN dotnet publish "ApostasApp.Core.Web/ApostasApp.Core.Web.csproj" -c Release -o /app/publish

# Usa a imagem final do ASP.NET para a aplicação em produção
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
# Copia a aplicação backend publicada no estágio anterior
COPY --from=backend-builder /app/publish .
# Copia a aplicação frontend compilada no primeiro estágio para a pasta wwwroot
COPY --from=frontend-builder /usr/src/app/dist/bolao-obnline-app-v5/ ./wwwroot/
# Define o comando que o container irá executar
ENTRYPOINT ["dotnet", "ApostasApp.Core.Web.dll"]