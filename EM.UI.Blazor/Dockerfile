FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["EM.UI.Blazor/EM.UI.Blazor.csproj", "EM.UI.Blazor/"]
COPY ["EM.Dto/EM.Dto.csproj", "EM.Dto/"]
RUN dotnet restore "EM.UI.Blazor/EM.UI.Blazor.csproj"
COPY . .
WORKDIR "/src/EM.UI.Blazor"
RUN dotnet build "EM.UI.Blazor.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EM.UI.Blazor.csproj" -c Release -o /app/publish

FROM nginx:alpine AS final
WORKDIR /etc/nginx/certs
COPY EM.UI.Blazor/certs .
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish/wwwroot .
COPY EM.UI.Blazor/nginx.conf /etc/nginx/nginx.conf
EXPOSE 80
EXPOSE 443