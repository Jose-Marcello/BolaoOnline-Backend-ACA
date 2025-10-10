#------------------------------------------------------------------
# Estágio 1: Build da Aplicação ASP.NET Core
#------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS final
WORKDIR /app

# ... o resto do código de build ...

#------------------------------------------------------------------
# Estágio de Produção Final
#------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=final /app/publish .
COPY ["./src/ApostasApp.Core.Web/wwwroot", "./wwwroot"]

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

ENTRYPOINT ["dotnet", "ApostasApp.Core.Web.dll"]