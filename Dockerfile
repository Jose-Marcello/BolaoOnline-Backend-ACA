# ARQUIVO: Dockerfile (DEVE FICAR NA RAIZ)

# --- Estágio 1: Build (Compilação) ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY . .
WORKDIR "/app/src/ApostasApp.Core.Web"
# CORREÇÃO: Ignora warnings no restore
RUN dotnet restore /p:TreatWarningsAsErrors=false
# CORREÇÃO: Ignora warnings no publish
RUN dotnet publish -c Release -o /app/publish --no-self-contained /p:TreatWarningsAsErrors=false

# --- Estágio 2: Imagem de Produção Final (Runtime) ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Define a porta de escuta interna como 8080 (Crucial para ACA)
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ApostasApp.Core.Web.dll"]
