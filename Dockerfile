# Estágio de build do Frontend Angular
FROM node:18-alpine AS frontend-builder
WORKDIR /app/BolaoOnlineAppV5
COPY ./BolaoOnlineAppV5/package*.json ./
RUN npm install
COPY ./BolaoOnlineAppV5/ .
RUN npm run build -- --output-path=/app/dist --base-href=/

# Estágio de build do Backend .NET
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS backend-builder
WORKDIR /app
COPY . .
RUN dotnet restore "src/ApostasApp.Core.Web/ApostasApp.Core.Web.csproj"
RUN dotnet publish "src/ApostasApp.Core.Web/ApostasApp.Core.Web.csproj" -c Release -o /app/publish

# Estágio final: criando a imagem de produção
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
# Copia os arquivos do backend
COPY --from=backend-builder /app/publish .
# Copia os arquivos estáticos do frontend para a wwwroot
COPY --from=frontend-builder /app/dist/ ./wwwroot/
ENTRYPOINT ["dotnet", "ApostasApp.Core.Web.dll"]