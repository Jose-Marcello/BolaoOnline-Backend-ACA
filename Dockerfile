# Estágio de build do Angular
FROM node:18 AS frontend-build
WORKDIR /app
COPY . .
WORKDIR /app/src/ApostasApp.Core.Web/BolaoOnlineAppV5
RUN npm install
RUN npm run build -- --output-path=./dist/browser --base-href=/

# Estágio de build do .NET
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS backend-build
WORKDIR /app
COPY . .
WORKDIR "/app/src/ApostasApp.Core.Web"
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

# Estágio final (runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=backend-build /app/publish .
COPY --from=frontend-build /app/src/ApostasApp.Core.Web/BolaoOnlineAppV5/dist/browser/ ./wwwroot/
ENTRYPOINT ["dotnet", "ApostasApp.Core.Web.dll"]