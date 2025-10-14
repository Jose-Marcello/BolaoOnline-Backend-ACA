# Este Dockerfile é para um aplicativo .NET Core (Backend)
# Ele cria uma imagem do seu backend, ignorando o frontend.

# ======================================================
# Estágio 1: Build do Backend (.NET Core)
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
# Estágio 2: Estágio Final (Runtime)
# ======================================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Adicionamos o comando EXPOSE 8080: ESSENCIAL para avisar ao Docker e Azure qual é a porta interna.
EXPOSE 8080

# Copia a aplicação .NET publicada do estágio backend-build
COPY --from=backend-build /app/publish .

ENTRYPOINT ["dotnet", "ApostasApp.Core.Web.dll"]