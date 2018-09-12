FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY AspNetCoreWebApp2/AspNetCoreWebApp2.csproj AspNetCoreWebApp2/
RUN dotnet restore AspNetCoreWebApp2/AspNetCoreWebApp2.csproj
COPY . .
WORKDIR /src/AspNetCoreWebApp2
RUN dotnet build AspNetCoreWebApp2.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish AspNetCoreWebApp2.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "AspNetCoreWebApp2.dll"]
