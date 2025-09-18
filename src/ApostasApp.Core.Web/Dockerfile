# Use a imagem do SDK do .NET para o estágio de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia os arquivos de backend para restaurar as dependências
COPY ["BolaoOnlineProject/src/ApostasApp.Core.Web/ApostasApp.Core.Web.csproj", "BolaoOnlineProject/src/ApostasApp.Core.Web/"]
COPY ["BolaoOnlineProject/src/ApostasApp.Core.Application/ApostasApp.Core.Application.csproj", "BolaoOnlineProject/src/ApostasApp.Core.Application/"]
COPY ["BolaoOnlineProject/src/ApostasApp.Core.Domain/ApostasApp.Core.Domain.csproj", "BolaoOnlineProject/src/ApostasApp.Core.Domain/"]
COPY ["BolaoOnlineProject/src/ApostasApp.Core.Infrastructure/ApostasApp.Core.Infrastructure.csproj", "BolaoOnlineProject/src/ApostasApp.Core.Infrastructure/"]

# Copia os arquivos de frontend para instalar as dependências
COPY ["BolaoOnlineProject/BolaoOnlineAppV5/package.json", "BolaoOnlineProject/BolaoOnlineAppV5/package-lock.json", "BolaoOnlineProject/BolaoOnlineAppV5/"]

# Copia todo o código do projeto para o contêiner
COPY ["BolaoOnlineProject/", "BolaoOnlineProject/"]

# Restaura as dependências do backend
WORKDIR "/src/BolaoOnlineProject/src/ApostasApp.Core.Web"
RUN dotnet restore "ApostasApp.Core.Web.csproj"

# Instala e faz o build do frontend
WORKDIR "/src/BolaoOnlineProject/BolaoOnlineAppV5"
RUN npm install
RUN npm run build

# Publica a aplicação ASP.NET Core
WORKDIR "/src/BolaoOnlineProject/src/ApostasApp.Core.Web"
RUN dotnet publish "ApostasApp.Core.Web.csproj" -c Release -o /app/publish

# Copia os arquivos estáticos do frontend para a pasta wwwroot do backend
# O build do Angular é feito em /src/BolaoOnlineProject/BolaoOnlineAppV5/dist/bolao-online-app-v5/browser
# O publish do .NET é feito em /app/publish/wwwroot
RUN cp -R "/src/BolaoOnlineProject/BolaoAppV5/dist/bolao-online-app-v5/browser/." "/app/publish/wwwroot/"


# Estágio final do contêiner para a execução
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Define o ponto de entrada da aplicação
ENTRYPOINT ["dotnet", "ApostasApp.Core.Web.dll"]
