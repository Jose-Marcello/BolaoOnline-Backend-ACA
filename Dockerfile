#------------------------------------------------------------------
# Estágio 1: Build da Aplicação ASP.NET Core
#------------------------------------------------------------------
# Usa a imagem SDK para o estágio de build, garantindo todas as ferramentas
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS final

# Define o diretório de trabalho dentro do contêiner
WORKDIR /app

# Copia o arquivo de solução e os arquivos de projeto .NET para otimizar o cache de build
COPY ["ApostasApp.Core.sln", "./"]
COPY ["src/ApostasApp.Core.Web/ApostasApp.Core.Web.csproj", "src/ApostasApp.Core.Web/"]
COPY ["src/ApostasApp.Core.Application/ApostasApp.Core.Application.csproj", "src/ApostasApp.Core.Application/"]
COPY ["src/ApostasApp.Core.Domain/ApostasApp.Core.Domain.csproj", "src/ApostasApp.Core.Domain/"]
COPY ["src/ApostasApp.Core.Infrastructure/ApostasApp.Core.Infrastructure.csproj", "src/ApostasApp.Core.Infrastructure/"]

# Restaura as dependências
RUN dotnet restore ApostasApp.Core.sln

# Copia o restante do código-fonte para o contêiner
COPY . .

# Publica a aplicação, gerando os binários para o deploy
RUN dotnet publish "src/ApostasApp.Core.Web/ApostasApp.Core.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

#------------------------------------------------------------------
# Estágio de Produção Final
#------------------------------------------------------------------
# Use a imagem de runtime oficial da Microsoft.
# Ela é mais leve e otimizada para ambientes de produção.
FROM mcr.microsoft.com/dotnet/runtime:8.0

# Define o diretório de trabalho para a execução da aplicação
WORKDIR /app

# Copia apenas os binários publicados para o diretório de trabalho do estágio final
COPY --from=final /app/publish .

# Copia a pasta wwwroot (com arquivos estáticos) para o local correto
COPY ["./src/ApostasApp.Core.Web/wwwroot", "./wwwroot"]

# Define a porta de escuta do contêiner para o App Service
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

# Comando de inicialização da aplicação
ENTRYPOINT ["dotnet", "ApostasApp.Core.Web.dll"]