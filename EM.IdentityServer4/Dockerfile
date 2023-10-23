FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
# FROM bitnami/aspnet-core:3.1.26-debian-11-r8 AS base
#RUN curl -ks  http://pki.mts.ru/class2root.crt -o /usr/local/share/ca-certificates/class2root.crt && /usr/sbin/update-ca-certificates
WORKDIR /app
EXPOSE 10001
ENV ASPNETCORE_URLS=https://+:10001
ENV ASPNETCORE_Kestrel__Certificates__Default__Password="21"
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/https/is4.pfx

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
# FROM bitnami/dotnet-sdk:3.1.420-debian-11-r8 AS build
#RUN curl -ks  http://pki.mts.ru/class2root.crt -o /usr/local/share/ca-certificates/class2root.crt && /usr/sbin/update-ca-certificates
WORKDIR /src
COPY "EM.IdentityServer4.csproj" .
RUN dotnet restore "EM.IdentityServer4.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "EM.IdentityServer4.csproj" -c Release -o /app/build --no-restore

FROM build AS publish
RUN dotnet publish "EM.IdentityServer4.csproj" -c Release -o /app/publish

FROM base AS final
RUN apt update -y
RUN apt install libengine-gost-openssl1.1 -y
COPY certs/openssl.cnf /etc/ssl/
WORKDIR /https
COPY certs .
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EM.IdentityServer4.dll"]
