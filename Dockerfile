#------------------------------------------------------------------
# Estágio 1: Build da Aplicação ASP.NET Core
#------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copia e restaura as dependências
COPY ["ApostasApp.Core.sln", "./"]
COPY ["src/ApostasApp.Core.Web/ApostasApp.Core.Web.csproj", "src/ApostasApp.Core.Web/"]
COPY ["src/ApostasApp.Core.Application/ApostasApp.Core.Application.csproj", "src/ApostasApp.Core.Application/"]
COPY ["src/ApostasApp.Core.Domain/ApostasApp.Core.Domain.csproj", "src/ApostasApp.Core.Domain/"]
COPY ["src/ApostasApp.Core.Infrastructure/ApostasApp.Core.Infrastructure.csproj", "src/ApostasApp.Core.Infrastructure/"]

RUN dotnet restore ApostasApp.Core.sln

# Copia todo o código e publica
COPY . .
RUN dotnet publish -c Release -o out "src/ApostasApp.Core.Web/ApostasApp.Core.Web.csproj"


#------------------------------------------------------------------
# Estágio 2: Ambiente de Produção Final
#------------------------------------------------------------------
# Use uma imagem otimizada para o Azure, que já vem com SSH
FROM mcr.microsoft.com/azure-app-service/dotnet:8-aspnetcore

WORKDIR /app
COPY --from=build-env /app/out ./

# Define a porta
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

# Comando de inicialização
ENTRYPOINT ["dotnet", "ApostasApp.Core.Web.dll"]