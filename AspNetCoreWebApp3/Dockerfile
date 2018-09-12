FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY AspNetCoreWebApp3/AspNetCoreWebApp3.csproj AspNetCoreWebApp3/
RUN dotnet restore AspNetCoreWebApp3/AspNetCoreWebApp3.csproj
COPY . .
WORKDIR /src/AspNetCoreWebApp3
RUN dotnet build AspNetCoreWebApp3.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish AspNetCoreWebApp3.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "AspNetCoreWebApp3.dll"]
