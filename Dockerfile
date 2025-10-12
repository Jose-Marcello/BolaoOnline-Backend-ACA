# Estágio de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ApostasApp.Core.Web/ApostasApp.Core.Web.csproj", "ApostasApp.Core.Web/"]
COPY ["ApostasApp.Core.Application/ApostasApp.Core.Application.csproj", "ApostasApp.Core.Application/"]
COPY ["ApostasApp.Core.Domain/ApostasApp.Core.Domain.csproj", "ApostasApp.Core.Domain/"]
COPY ["ApostasApp.Core.Infrastructure/ApostasApp.Core.Infrastructure.csproj", "ApostasApp.Core.Infrastructure/"]
RUN dotnet restore "ApostasApp.Core.Web/ApostasApp.Core.Web.csproj"
COPY . .
WORKDIR "/src/ApostasApp.Core.Web"
RUN dotnet build "ApostasApp.Core.Web.csproj" -c Release -o /app/build

# Estágio de publicação
FROM build AS publish
RUN dotnet publish "ApostasApp.Core.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Estágio final (runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ApostasApp.Core.Web.dll"]