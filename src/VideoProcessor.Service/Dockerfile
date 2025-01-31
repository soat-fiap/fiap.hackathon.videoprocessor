FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 80
EXPOSE 443

USER root
RUN apt-get update && apt-get install -y ffmpeg
RUN ffmpeg -version

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ./*.sln /
COPY src ./src

# Restoring just the api project will cause a chain reaction that will resolve the nuget dependencies from all class libraries.
RUN dotnet restore src/VideoProcessor.Service

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
ARG API_VERSION
RUN dotnet publish "src/VideoProcessor.Service/VideoProcessor.Service.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false


FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VideoProcessor.Service.dll"]

