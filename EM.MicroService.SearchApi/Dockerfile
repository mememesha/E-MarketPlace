FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 6789
ENV ASPNETCORE_URLS=https://+:6789

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["EM.MicroService.SearchApi/EM.MicroService.SearchApi.csproj", "EM.MicroService.SearchApi/"]
RUN dotnet restore "EM.MicroService.SearchApi/EM.MicroService.SearchApi.csproj"
COPY . .
WORKDIR "/src/EM.MicroService.SearchApi"
RUN dotnet build "EM.MicroService.SearchApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EM.MicroService.SearchApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EM.MicroService.SearchApi.dll"]