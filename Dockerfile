# Estágio 1: Build do Backend ASP.NET Core
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS backend-build
# Define o WORKDIR como a RAIZ do container.
WORKDIR /

# Copia a Solution e os .csproj que o restore precisa
COPY ["ApostasApp.Core.sln", "./"]
COPY ["src/ApostasApp.Core.Web/ApostasApp.Core.Web.csproj", "src/ApostasApp.Core.Web/"]

# CORREÇÃO CRÍTICA: Copia a pasta 'src' inteira para que o 'dotnet restore' encontre todos os projetos dependentes
COPY src/ src/

# Agora, fazemos o restore referenciando a Solution na RAIZ
RUN dotnet restore ApostasApp.Core.sln

# Copia o restante do código para o Backend e publica
COPY . .
RUN dotnet publish "src/ApostasApp.Core.Web/ApostasApp.Core.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Estágio 2: Build do Frontend Angular (em um ambiente Node.js)
FROM node:18-alpine AS frontend-build
WORKDIR /

# Cria uma pasta separada para o build do Angular
WORKDIR /frontend-app

# Copia os arquivos de dependência do Angular (package.json)
COPY ["src/ApostasApp.Core.Web/BolaoOnlineAppV5/package.json", "src/ApostasApp.Core.Web/BolaoOnlineAppV5/package-lock.json", "./"]

# Instala as dependências
RUN npm install

# Faz o build do frontend
# Copia o restante do código para o build e executa
COPY ["src/ApostasApp.Core.Web/BolaoOnlineAppV5", "/frontend-app/"]
RUN npm run build -- --output-path=/app/dist/angular-app/browser

# Estágio 3: Imagem de Produção Final
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copia os arquivos publicados do backend para a imagem final
COPY --from=backend-build /app/publish ./

# Remove a pasta wwwroot padrão gerada pelo .NET
RUN rm -rf wwwroot

# Copia a pasta de build do Angular para a wwwroot final
COPY --from=frontend-build /frontend-app/app/dist/angular-app/browser ./wwwroot

# Expõe a porta e define o ponto de entrada
EXPOSE 80
ENTRYPOINT ["dotnet", "ApostasApp.Core.Web.dll"]
