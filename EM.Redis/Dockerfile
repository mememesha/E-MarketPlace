FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 7045
ENV ASPNETCORE_URLS=https://+:7045

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["EM.Redis/EM.Redis.csproj", "EM.Redis/"]
RUN dotnet restore "EM.Redis/EM.Redis.csproj"
COPY . .
WORKDIR "/src/EM.Redis"
RUN dotnet build "EM.Redis.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EM.Redis.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EM.Redis.dll"]