FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
EXPOSE 443

COPY EM.UI.Blazor.csproj .
RUN dotnet restore EM.UI.Blazor.csproj
COPY . .
RUN dotnet build EM.UI.Blazor.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish EM.UI.Blazor.csproj -c Release -o /app/publish

FROM nginx:alpine AS final
WORKDIR /etc/nginx/certs
COPY certs .
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish/wwwroot .
COPY nginx.conf /etc/nginx/nginx.conf