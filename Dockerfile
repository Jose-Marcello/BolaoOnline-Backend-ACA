# Estágio de build do Angular
FROM node:18 AS frontend-build
WORKDIR /app
COPY ./src/ApostasApp.Core.Presentation/package.json ./
RUN npm install
COPY ./src/ApostasApp.Core.Presentation/ ./
RUN npm run build

# Estágio de build do .NET
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS backend-build
WORKDIR /app
COPY ["src/ApostasApp.Core.Web/ApostasApp.Core.Web.csproj", "src/ApostasApp.Core.Web/"]
COPY ["src/ApostasApp.Core.Application/ApostasApp.Core.Application.csproj", "src/ApostasApp.Core.Application/"]
COPY ["src/ApostasApp.Core.Domain/ApostasApp.Core.Domain.csproj", "src/ApostasApp.Core.Domain/"]
COPY ["src/ApostasApp.Core.Infrastructure/ApostasApp.Core.Infrastructure.csproj", "src/ApostasApp.Core.Infrastructure/"]
RUN dotnet restore "src/ApostasApp.Core.Web/ApostasApp.Core.Web.csproj"
COPY . .
WORKDIR "/app/src/ApostasApp.Core.Web"
RUN dotnet build "ApostasApp.Core.Web.csproj" -c Release -o /app/build

# Estágio de publicação
FROM backend-build AS publish
RUN dotnet publish "ApostasApp.Core.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Estágio final (runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
# Copia os arquivos do frontend para a pasta wwwroot
COPY --from=frontend-build /app/dist/apostas-app/browser/ ./wwwroot/
ENTRYPOINT ["dotnet", "ApostasApp.Core.Web.dll"]