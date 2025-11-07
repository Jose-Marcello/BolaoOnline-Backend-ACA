# --- Estágio 1: Build (Compilação) ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS backend-build
WORKDIR /src

# Copia os arquivos .csproj e a solução primeiro para otimizar o cache
COPY ["ApostasApp.Core.sln", "."]
# Copia o projeto Web (e seus projetos dependentes)
COPY ["src/ApostasApp.Core.Web/ApostasApp.Core.Web.csproj", "src/ApostasApp.Core.Web/"]
COPY ["src/ApostasApp.Core.Application/ApostasApp.Core.Application.csproj", "src/ApostasApp.Core.Application/"]
COPY ["src/ApostasApp.Core.Domain/ApostasApp.Core.Domain.csproj", "src/ApostasApp.Core.Domain/"]
COPY ["src/ApostasApp.Core.Infrastructure/ApostasApp.Core.Infrastructure.csproj", "src/ApostasApp.Core.Infrastructure/"]

# Garante que a estrutura de pastas exista para o restore
RUN mkdir -p src/ApostasApp.Core.Web \
           src/ApostasApp.Core.Application \
           src/ApostasApp.Core.Domain \
           src/ApostasApp.Core.Infrastructure

# Restaura todas as dependências antes de copiar o código-fonte
RUN dotnet restore ApostasApp.Core.sln

# Copia todo o código-fonte restante (para resolver as referências)
COPY . .

# Publica a aplicação de forma otimizada
WORKDIR /src/src/ApostasApp.Core.Web
RUN dotnet publish -c Release -o /app/publish --no-self-contained

# --- Estágio 2: Imagem de Produção Final (Runtime) ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copia os arquivos publicados do estágio 'build'
COPY --from=backend-build /app/publish .

# ** CRÍTICO PARA ACA: Define a porta de escuta interna como 8080 **
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Comando de entrada
ENTRYPOINT ["dotnet", "ApostasApp.Core.Web.dll"]
