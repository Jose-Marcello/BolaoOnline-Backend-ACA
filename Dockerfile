# Estágio de build do Frontend Angular
FROM node:18-alpine AS frontend-builder
WORKDIR /app
COPY ./BolaoOnlineAppV5/package.json ./BolaoOnlineAppV5/package-lock.json ./
RUN npm install
COPY ./BolaoOnlineAppV5/ .
RUN npm run build -- --output-path ./dist --base-href /

# Estágio de build do Backend .NET
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS backend-builder
WORKDIR /src
COPY ./src/ApostasApp.Core.Application/ ./ApostasApp.Core.Application/
COPY ./src/ApostasApp.Core.Domain/ ./ApostasApp.Core.Domain/
COPY ./src/ApostasApp.Core.Infrastructure/ ./ApostasApp.Core.Infrastructure/
COPY ./src/ApostasApp.Core.Web/ ./ApostasApp.Core.Web/

# Restaura as dependências
RUN dotnet restore "ApostasApp.Core.Web/ApostasApp.Core.Web.csproj"
# Publica a aplicação
RUN dotnet publish "ApostasApp.Core.Web/ApostasApp.Core.Web.csproj" -c Release -o /app/publish

# Estágio final: criando a imagem de produção
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
# Copia os arquivos do backend
COPY --from=backend-builder /app/publish .
# Copia os arquivos estáticos do frontend para a wwwroot
COPY --from=frontend-builder /app/dist/bolao-obnline-app-v5/ ./wwwroot/
ENTRYPOINT ["dotnet", "ApostasApp.Core.Web.dll"]