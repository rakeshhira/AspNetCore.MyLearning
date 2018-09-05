# AspNetCore.MyLearning
I started this repository to document my learning experience with using Visual Studio 2017 to 
- Building AspNetCore based Microservices Using Docker 
- Using selenium & xUnit for Behavior Driven Development 
- Using Postgres, Redis for data storage
- Using Rabbitmq for asynchronous message queuing

# Visual Studio Tools for Docker with ASP.NET Core

I followed the instructions in following article to create a visual studio project to learn docker support

- [Visual Studio Tools for Docker with ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/docker/visual-studio-tools-for-docker?view=aspnetcore-2.1)

Go ahead follow this article and then you compare your learning with mine.  I created the project as following
- Visual Studio 2017
- **File** -> **New** -> **Project** -> **Visual C#** -> **Web** > **ASP.NET Core Web Application**
- Set name of the project to **VsToolsForDocker**
- Select **API** template
- Select **Enable Docker Support**
- Unselect **HTTPS**

## What did I learn?

### `docker images` command
This article exposed me to [docker images](https://docs.docker.com/engine/reference/commandline/images/) command.  If you followed the article you would have ran following command to list all docker images on your system.  

> docker images

If your environment is anything like mine, the command would have come back with lot more images that you expected.  This was because I had been playing with microservices & docker for quite a few days and didn't clean anything up.  I really didn't care about any of these images, so I ran [docker image prune](https://docs.docker.com/engine/reference/commandline/image_prune/) command to clean up my system

> docker image prune -a 

This step was in prepration to understand what visual studio `docker` command was doing my behalf.

### `docker build .` doesn't work
The article introduces to the [Dockerfile](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/docker/visual-studio-tools-for-docker?view=aspnetcore-2.1#dockerfile-overview) that Visual Studio added to the project.  Following was added to my project:
```
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
ENTRYPOINT ["dotnet", "VsToolsForDocker.dll"
```

When I tried `docker build .` from command line from the folder where dockerfile was present, it didn't work.  It failed with message **COPY failed: stat /var/lib/docker/tmp/docker-builder022060742/VsToolsForDocker/VsToolsForDocker.csproj: no such file or directory**.  I didn't understand why? 

> docker build .

```
**********************************************************************
** Visual Studio 2017 Developer Command Prompt v15.8.2
** Copyright (c) 2017 Microsoft Corporation
**********************************************************************

D:\GitRepos\AspNetCore.MyLearning\VsToolsForDocker>docker build .
Sending build context to Docker daemon  1.183MB
Step 1/16 : FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
 ---> 251f1045c425
Step 2/16 : WORKDIR /app
 ---> Using cache
 ---> 4c71f8fabe82
Step 3/16 : EXPOSE 80
 ---> Using cache
 ---> e857956103d2
Step 4/16 : FROM microsoft/dotnet:2.1-sdk AS build
2.1-sdk: Pulling from microsoft/dotnet
55cbf04beb70: Pull complete
1607093a898c: Pull complete
9a8ea045c926: Pull complete
d4eee24d4dac: Pull complete
301e8412b0a6: Pull complete
b93cc8ca51b6: Pull complete
1d30e5bc88a9: Pull complete
Digest: sha256:14876c4207dc91300a9de5e3579782de5c99619d4906f603cbd360c29a6d695c
Status: Downloaded newer image for microsoft/dotnet:2.1-sdk
 ---> bde01d9ed6eb
Step 5/16 : WORKDIR /src
 ---> Running in d9172044bbe3
Removing intermediate container d9172044bbe3
 ---> 1f6397df96e3
Step 6/16 : COPY ["VsToolsForDocker/VsToolsForDocker.csproj", "VsToolsForDocker/"]
COPY failed: stat /var/lib/docker/tmp/docker-builder022060742/VsToolsForDocker/VsToolsForDocker.csproj: no such file or directory

D:\GitRepos\AspNetCore.MyLearning\VsToolsForDocker>
```

This file ofcourse worked from Visual Studio, so I took a look at Visual Studio Build output. I immediately noticed that Visual Studio used a different command that what I used **docker build -f "D:\GitRepos\AspNetCore.MyLearning\VsToolsForDocker\Dockerfile" -t vstoolsfordocker:dev --target base "D:\GitRepos\AspNetCore.MyLearning"**

```
1>------ Build started: Project: VsToolsForDocker, Configuration: Debug Any CPU ------
1>VsToolsForDocker -> D:\GitRepos\AspNetCore.MyLearning\VsToolsForDocker\bin\Debug\netcoreapp2.1\VsToolsForDocker.dll
1>C:\WINDOWS\System32\WindowsPowerShell\v1.0\powershell.exe -NonInteractive -NoProfile -WindowStyle Hidden -ExecutionPolicy RemoteSigned -File "C:\Users\xyz\AppData\Local\Temp\GetVsDbg.ps1" -Version vs2017u5 -RuntimeID debian.8-x64 -InstallPath "C:\Users\xyz\vsdbg\vs2017u5"
1>Info: Using vsdbg version '15.7.20425.2'
1>Info: Using Runtime ID 'linux-x64'
1>Info: Latest version of VsDbg is present. Skipping downloads
1>docker build -f "D:\GitRepos\AspNetCore.MyLearning\VsToolsForDocker\Dockerfile" -t vstoolsfordocker:dev --target base "D:\GitRepos\AspNetCore.MyLearning"
1>Sending build context to Docker daemon  19.46kB
1>
1>Step 1/3 : FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
1> ---> 251f1045c425
1>Step 2/3 : WORKDIR /app
1> ---> Using cache
1> ---> 4c71f8fabe82
1>Step 3/3 : EXPOSE 80
1> ---> Using cache
1> ---> e857956103d2
1>Successfully built e857956103d2
1>Successfully tagged vstoolsfordocker:dev
1>SECURITY WARNING: You are building a Docker image from Windows against a non-Windows Docker host. All files and directories added to build context will have '-rwxr-xr-x' permissions. It is recommended to double check and reset permissions for sensitive files and directories.
1>docker run -dt -v "C:\Users\xyz\vsdbg\vs2017u5:/remote_debugger:rw" -v "D:\GitRepos\AspNetCore.MyLearning\VsToolsForDocker:/app" -v "C:\Users\xyz\.nuget\packages\:/root/.nuget/fallbackpackages2" -v "C:\Program Files\dotnet\sdk\NuGetFallbackFolder:/root/.nuget/fallbackpackages" -e "DOTNET_USE_POLLING_FILE_WATCHER=1" -e "ASPNETCORE_ENVIRONMENT=Development" -e "NUGET_PACKAGES=/root/.nuget/fallbackpackages2" -e "NUGET_FALLBACK_PACKAGES=/root/.nuget/fallbackpackages;/root/.nuget/fallbackpackages2" -P --entrypoint tail vstoolsfordocker:dev -f /dev/null
1>27971ff96be440f407602f4edc7c20f24ffc66ca086320adfab56cb5e8ede9ee
========== Build: 1 succeeded, 0 failed, 0 up-to-date, 0 skipped ==========

```

The dockerfile Visual Studio had created is a multi-stage build and the debug command had targetted "base" build.  And all that build did was to build from **microsoft/dotnet:2.1-aspnetcore-runtime** and then create a **/app** folder in that image and set it as the working directory.  **So where were the binaries?**

Well Visual Studio use volume mounting to provide debugging experience.  I noticed following command in the build output.
> 1>docker run -dt -v "C:\Users\xyz\vsdbg\vs2017u5:/remote_debugger:rw" -v "D:\GitRepos\AspNetCore.MyLearning\VsToolsForDocker:/app" -v "C:\Users\xyz\.nuget\packages\:/root/.nuget/fallbackpackages2" -v "C:\Program Files\dotnet\sdk\NuGetFallbackFolder:/root/.nuget/fallbackpackages" -e "DOTNET_USE_POLLING_FILE_WATCHER=1" -e "ASPNETCORE_ENVIRONMENT=Development" -e "NUGET_PACKAGES=/root/.nuget/fallbackpackages2" -e "NUGET_FALLBACK_PACKAGES=/root/.nuget/fallbackpackages;/root/.nuget/fallbackpackages2" -P --entrypoint tail vstoolsfordocker:dev -f /dev/null

## Need default .gitignore file to push work to git
When I tried to stage my changes, I got error.  I had to follow the instruction below to create a default .gitignore file.
https://stackoverflow.com/questions/47460039/vs-2017-git-local-commit-db-lock-error-on-every-commit/47472811