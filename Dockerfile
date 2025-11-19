# --- Estágio 1: Build (Compilação) ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS backend-build
WORKDIR /app
# Copia o código-fonte inteiro (A Action executa o COPY . . a partir da raiz do repositório)
COPY . .
# Navega para o diretório do projeto Web
WORKDIR "/app/src/ApostasApp.Core.Web"
# Restaura as dependências
# CORREÇÃO: Ignora warnings no restore
RUN dotnet restore /p:TreatWarningsAsErrors=false
# Publica a aplicação de forma otimizada
# CORREÇÃO: Ignora warnings no publish
RUN dotnet publish -c Release -o /app/publish --no-self-contained /p:TreatWarningsAsErrors=false

# --- Estágio 2: Imagem de Produção Final (Runtime) ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# ** CRÍTICO PARA ACA: Define a porta de escuta interna como 8080 **
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Copia a aplicação .NET publicada do estágio backend-build
COPY --from=backend-build /app/publish .

ENTRYPOINT ["dotnet", "ApostasApp.Core.Web.dll"]
