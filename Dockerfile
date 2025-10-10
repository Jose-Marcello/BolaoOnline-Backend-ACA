#------------------------------------------------------------------
# Estágio 1: Build da Aplicação ASP.NET Core (sem alterações)
#------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS final

# Define o WORKDIR como a RAIZ do container
WORKDIR /app

# 1. Copia o arquivo de solução e os arquivos de projeto .NET
COPY ["ApostasApp.Core.sln", "./"]
COPY ["src/ApostasApp.Core.Web/ApostasApp.Core.Web.csproj", "src/ApostasApp.Core.Web/"]
COPY ["src/ApostasApp.Core.Application/ApostasApp.Core.Application.csproj", "src/ApostasApp.Core.Application/"]
COPY ["src/ApostasApp.Core.Domain/ApostasApp.Core.Domain.csproj", "src/ApostasApp.Core.Domain/"]
COPY ["src/ApostasApp.Core.Infrastructure/ApostasApp.Core.Infrastructure.csproj", "src/ApostasApp.Core.Infrastructure/"]

# Garante que o restore funcione corretamente
RUN dotnet restore ApostasApp.Core.sln

# Copia todo o restante do código-fonte
COPY . .

# 2. Publica o Backend
RUN dotnet publish "src/ApostasApp.Core.Web/ApostasApp.Core.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false


#------------------------------------------------------------------
# Estágio de Produção Final (A CORREÇÃO ESTÁ AQUI)
#------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# CORREÇÃO: Copia APENAS os binários publicados. O segundo ponto ('.') significa a pasta atual.
COPY --from=final /app/publish .

# CORREÇÃO: Agora, a wwwroot é copiada para o local correto.
COPY ["./src/ApostasApp.Core.Web/wwwroot", "./wwwroot"]

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

# Comando de inicialização
ENTRYPOINT ["dotnet", "ApostasApp.Core.Web.dll"]