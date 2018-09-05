FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["VsToolsForDocker/VsToolsForDocker.csproj", "VsToolsForDocker/"]
RUN dotnet restore "VsToolsForDocker/VsToolsForDocker.csproj"
COPY . .
WORKDIR "/src/VsToolsForDocker"
RUN dotnet build "VsToolsForDocker.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "VsToolsForDocker.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "VsToolsForDocker.dll"]