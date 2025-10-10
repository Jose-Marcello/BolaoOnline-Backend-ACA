#------------------------------------------------------------------
# Estágio 1: Build do Backend e Setup do Ambiente
#------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS backend-build
# Define o WORKDIR como a RAIZ do container para fácil referência
WORKDIR /

# 1. Copia o arquivo de solução e os arquivos de projeto .NET
# Isso permite que o NuGet otimize o download de dependências (Layer Caching)
COPY ["ApostasApp.Core.sln", "./"]
COPY ["src/ApostasApp.Core.Web/ApostasApp.Core.Web.csproj", "src/ApostasApp.Core.Web/"]
COPY ["src/ApostasApp.Core.Application/ApostasApp.Core.Application.csproj", "src/ApostasApp.Core.Application/"]
COPY ["src/ApostasApp.Core.Domain/ApostasApp.Core.Domain.csproj", "src/ApostasApp.Core.Domain/"]
COPY ["src/ApostasApp.Core.Infrastructure/ApostasApp.Core.Infrastructure.csproj", "src/ApostasApp.Core.Infrastructure/"]

# Garante que o restore funcione corretamente referenciando a Solution
RUN dotnet restore ApostasApp.Core.sln

# Copia todo o restante do código-fonte
COPY . .

# 2. Publica o Backend
# O ASP.NET Core está configurado para servir o Frontend a partir da wwwroot
RUN dotnet publish "src/ApostasApp.Core.Web/ApostasApp.Core.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false


#------------------------------------------------------------------
# Estágio 2: Build do Frontend Angular
#------------------------------------------------------------------
FROM node:18-alpine AS frontend-build
# Define WORKDIR para a pasta de arquivos estáticos do Angular
WORKDIR /frontend-app

# 1. Copia arquivos de dependência do Angular e instala
# Caminhos relativos à RAIZ do repositório (Contexto: .)
COPY ["src/ApostasApp.Core.Web/BolaoOnlineAppV5/package.json", "src/ApostasApp.Core.Web/BolaoOnlineAppV5/package-lock.json", "./"]

# Instala as dependências e limpa o cache (para estabilizar o build)
RUN npm install
RUN npm cache clean --force

# 2. Copia o código Angular e executa o build
# O Angular está configurado para criar a saída na pasta /app/dist/...
COPY ["src/ApostasApp.Core.Web/BolaoOnlineAppV5", "./"]
RUN npm run build -- --output-path=/app/dist/angular-app/browser


#------------------------------------------------------------------
# Estágio 3: Imagem de Produção Final
#------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
# Define o WORKDIR para a pasta onde a aplicação será executada
WORKDIR /app

# Copia os binários publicados do backend
COPY --from=backend-build /app/publish ./

# Copia o frontend Angular para a pasta wwwroot, onde o ASP.NET Core espera os arquivos estáticos
COPY --from=frontend-build /frontend-app/app/dist/angular-app/browser ./wwwroot

# Define a porta de escuta padrão do contêiner como 80, que é a porta que o App Service espera para HTTP
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

# Comando de inicialização que executa o binário principal
# A correção do roteamento de fallback está no seu Program.cs, que foi compilado neste binário.
ENTRYPOINT ["dotnet", "ApostasApp.Core.Web.dll"]

    
