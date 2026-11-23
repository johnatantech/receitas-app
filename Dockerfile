FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restaura primeiro (aproveita o cache de camadas)
COPY src/Receitas.Api/Receitas.Api.csproj src/Receitas.Api/
RUN dotnet restore src/Receitas.Api/Receitas.Api.csproj

COPY src/Receitas.Api/ src/Receitas.Api/
RUN dotnet publish src/Receitas.Api/Receitas.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .

ENV ASPNETCORE_ENVIRONMENT=Production

# O Render informa a porta pela variável PORT
CMD ["sh", "-c", "ASPNETCORE_URLS=http://0.0.0.0:${PORT:-8080} exec dotnet Receitas.Api.dll"]