# Este Dockerfile é para um aplicativo .NET Core com frontend Angular (Build Multi-Stage)
# Garante o tamanho mínimo da imagem final e a correta integração do SPA (Angular) no backend (.NET).

# ======================================================
# Estágio 1: Build do Frontend (Angular)
# ======================================================
FROM node:18 AS frontend-build
WORKDIR /app
# Copia o código-fonte inteiro para o estágio do frontend
COPY . .
# Navega para o diretório do projeto Angular
WORKDIR /app/src/ApostasApp.Core.Web/BolaoOnlineAppV5
# Instala as dependências do Angular
RUN npm install
# Executa o build do Angular.
RUN npm run build -- --output-path=./dist/browser --base-href=/

# ======================================================
# Estágio 2: Build do Backend (.NET Core)
# ======================================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS backend-build
WORKDIR /app
# Copia o código-fonte inteiro
COPY . .
# Navega para o diretório do projeto Web
WORKDIR "/app/src/ApostasApp.Core.Web"
# Restaura as dependências
RUN dotnet restore
# Publica a aplicação de forma otimizada
RUN dotnet publish -c Release -o /app/publish --no-self-contained

# ======================================================
# Estágio 3: Estágio Final (Runtime)
# ======================================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Adicionamos o comando EXPOSE 8080: ESSENCIAL para avisar ao Docker e Azure qual é a porta interna.
EXPOSE 8080

# Copia a aplicação .NET publicada do estágio backend-build
COPY --from=backend-build /app/publish .

# Copia os arquivos estáticos do Angular (dist/browser/) para a pasta wwwroot,
# onde o ASP.NET Core espera os arquivos estáticos do SPA.
COPY --from=frontend-build /app/src/ApostasApp.Core.Web/BolaoOnlineAppV5/dist/browser/ ./wwwroot/

# O comando de inicialização
ENTRYPOINT ["dotnet", "ApostasApp.Core.Web.dll"]
