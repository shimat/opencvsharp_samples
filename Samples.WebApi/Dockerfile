# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.
#
# Build context is the repository root, since the Data assets under Samples/Data/ are
# pulled in via a Content link in Samples.WebApi.csproj. Build and run from the repo root:
#   docker build -f Samples.WebApi/Dockerfile -t samples-webapi .
#   docker run --rm -p 8080:8080 samples-webapi

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

# The full (non-slim) OpenCvSharp5.official.runtime.linux-x64 package links highgui against
# GTK3, so the native library fails to load without it even though this app never calls
# highgui. FFmpeg (needed by VideoCapture) is statically linked into that package already.
# libdrm2/libatomic1 are further transitive dependencies not pulled in automatically
# (confirmed via `ldd libOpenCvSharpExtern.so`, since --no-install-recommends omits them).
RUN apt-get update \
    && apt-get install -y --no-install-recommends libgtk-3-0 libdrm2 libatomic1 \
    && rm -rf /var/lib/apt/lists/*
USER app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Samples.WebApi/Samples.WebApi.csproj", "Samples.WebApi/"]
RUN dotnet restore "Samples.WebApi/Samples.WebApi.csproj"
COPY . .
WORKDIR "/src/Samples.WebApi"
RUN dotnet build "Samples.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Samples.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Samples.WebApi.dll"]
