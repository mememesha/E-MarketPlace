FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 10001
ENV ASPNETCORE_URLS=http://+:10001
ENV ASPNETCORE_ENVIRONMENT=Development

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY "EM.IdentityServer4.csproj" .
RUN dotnet restore "EM.IdentityServer4.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "EM.IdentityServer4.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EM.IdentityServer4.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EM.IdentityServer4.dll"]
