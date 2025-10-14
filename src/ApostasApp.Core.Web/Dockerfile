Estágio 1: Build do Backend ASP.NET Core
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

Copia e restaura o projeto .NET (incluindo o arquivo de solução)
COPY ["ApostasApp.Core.sln", "./"]
COPY ["src/ApostasApp.Core.Web/ApostasApp.Core.Web.csproj", "src/ApostasApp.Core.Web/"]
RUN dotnet restore

Copia o código-fonte e publica
COPY . .
RUN dotnet publish "src/ApostasApp.Core.Web/ApostasApp.Core.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

Estágio 2: Imagem de Produção Final (API)
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

Copia os arquivos publicados do backend para a imagem final
COPY --from=build /app/publish ./

Expõe a porta e define o ponto de entrada
EXPOSE 80
ENTRYPOINT ["dotnet", "ApostasApp.Core.Web.dll"]

