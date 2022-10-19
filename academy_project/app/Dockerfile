# To be discussed during class
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

COPY src/ ./.

WORKDIR /app/IA.DevOps.Movies.Web

RUN dotnet publish "IA.DevOps.Movies.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/publish .
ENTRYPOINT ["dotnet", "IA.DevOps.Movies.Web.dll"]
