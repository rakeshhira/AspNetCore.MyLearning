FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["SwaggerSupport/SwaggerSupport.csproj", "SwaggerSupport/"]
RUN dotnet restore "SwaggerSupport/SwaggerSupport.csproj"
COPY . .
WORKDIR "/src/SwaggerSupport"
RUN dotnet build "SwaggerSupport.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "SwaggerSupport.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "SwaggerSupport.dll"]