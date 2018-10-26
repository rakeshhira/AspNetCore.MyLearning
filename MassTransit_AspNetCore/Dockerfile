FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["MassTransit_AspNetCore/MassTransit_AspNetCore.csproj", "MassTransit_AspNetCore/"]
RUN dotnet restore "MassTransit_AspNetCore/MassTransit_AspNetCore.csproj"
COPY . .
WORKDIR "/src/MassTransit_AspNetCore"
RUN dotnet build "MassTransit_AspNetCore.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "MassTransit_AspNetCore.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "MassTransit_AspNetCore.dll"]