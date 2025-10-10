#------------------------------------------------------------------
# Estágio 1: Build da Aplicação ASP.NET Core (Simplificado e Estabilizado)
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

# Garante que o restore funcione corretamente referenciando a Solution
RUN dotnet restore ApostasApp.Core.sln

# Copia todo o restante do código-fonte (incluindo a pasta wwwroot, que agora tem o index.html)
COPY . .

# 2. Publica o Backend
RUN dotnet publish "src/ApostasApp.Core.Web/ApostasApp.Core.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false


#------------------------------------------------------------------
# Estágio de Produção Final (Apenas Binários do .NET)
#------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
# Define o WORKDIR para a pasta onde a aplicação será executada
WORKDIR /app

# Copia os binários publicados do backend
COPY --from=final /app/publish ./

# A pasta wwwroot que tem o index.html e os assets do Angular precisa ser copiada manualmente
# O contêiner agora vai iniciar e tentar servir o index.html
RUN mkdir -p wwwroot/assets # Garante que a pasta assets exista
COPY ["src/ApostasApp.Core.Web/BolaoOnlineAppV5/dist/angular-app/browser/index.html", "./wwwroot/"]
COPY ["src/ApostasApp.Core.Web/BolaoOnlineAppV5/dist/angular-app/browser/styles-LN5CX2XI.css", "./wwwroot/"]
COPY ["src/ApostasApp.Core.Web/BolaoOnlineAppV5/dist/angular-app/browser/main-V35KFR4P.js", "./wwwroot/"]
COPY ["src/ApostasApp.Core.Web/BolaoOnlineAppV5/dist/angular-app/browser/polyfills-FFHMD2TL.js", "./wwwroot/"]


# Define a porta de escuta padrão do contêiner como 80, que é a porta que o App Service espera para HTTP
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

# Comando de inicialização
ENTRYPOINT ["dotnet", "ApostasApp.Core.Web.dll"]
