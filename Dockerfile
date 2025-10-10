#------------------------------------------------------------------
# Estágio 1: Build da Aplicação ASP.NET Core
#------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS final
WORKDIR /app

COPY ["ApostasApp.Core.sln", "./"]
COPY ["src/ApostasApp.Core.Web/ApostasApp.Core.Web.csproj", "src/ApostasApp.Core.Web/"]
COPY ["src/ApostasApp.Core.Application/ApostasApp.Core.Application.csproj", "src/ApostasApp.Core.Application/"]
COPY ["src/ApostasApp.Core.Domain/ApostasApp.Core.Domain.csproj", "src/ApostasApp.Core.Domain/"]
COPY ["src/ApostasApp.Core.Infrastructure/ApostasApp.Core.Infrastructure.csproj", "src/ApostasApp.Core.Infrastructure/"]

RUN dotnet restore ApostasApp.Core.sln

COPY . .

RUN dotnet publish "src/ApostasApp.Core.Web/ApostasApp.Core.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false


#------------------------------------------------------------------
# Estágio de Produção Final
#------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
# Define o WORKDIR para a pasta onde a aplicação será executada
WORKDIR /app

# Copia os binários publicados para o diretório de trabalho do contêiner
COPY --from=final /app/publish .

# Copia a pasta wwwroot para a subpasta wwwroot do diretório de trabalho
COPY ["./src/ApostasApp.Core.Web/wwwroot", "./wwwroot"]

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

ENTRYPOINT ["dotnet", "ApostasApp.Core.Web.dll"]