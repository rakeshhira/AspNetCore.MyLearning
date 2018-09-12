FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["AspNetCoreWebApp1/AspNetCoreWebApp1.csproj", "AspNetCoreWebApp1/"]
RUN dotnet restore "AspNetCoreWebApp1/AspNetCoreWebApp1.csproj"
COPY . .
WORKDIR "/src/AspNetCoreWebApp1"
RUN dotnet build "AspNetCoreWebApp1.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "AspNetCoreWebApp1.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "AspNetCoreWebApp1.dll"]