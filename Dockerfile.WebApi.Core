FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 7890
ENV ASPNETCORE_URLS=https://+:7890
ENV ASPNETCORE_Kestrel__Certificates__Default__Password="password"
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/https/webapi.pfx

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["EM.WebApi.Core/EM.WebApi.Core.csproj", "EM.WebApi.Core/"]
RUN dotnet restore "EM.WebApi.Core/EM.WebApi.Core.csproj"
COPY . .
WORKDIR "/src/EM.WebApi.Core"
RUN dotnet build "EM.WebApi.Core.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EM.WebApi.Core.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /https
COPY EM.WebApi.Core/certs .
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EM.WebApi.Core.dll"]
