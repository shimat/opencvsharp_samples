# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.
#
# Build context is the repository root, since the Data assets under Samples/Data/ are
# pulled in via a Content link in Samples.WebApi.csproj. Build and run from the repo root:
#   docker build -f Samples.WebApi/Dockerfile -t samples-webapi .
#   docker run --rm -p 8080:8080 samples-webapi

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

# OpenCvSharp5.official.runtime.linux-x64.headless keeps the full module set (videoio,
# dnn, ml, contrib, stitching, barcode, ...) that this app needs (VideoCapture) but is
# built with highgui disabled, so GTK3/X11 are gone. The libdrm2 workaround for
# https://github.com/shimat/opencvsharp/issues/2071 is no longer needed as of
# 5.0.0.20260721: the native library no longer references libdrm.so.2 at all
# (confirmed via `readelf -d`/`strings` and `ldd` against mcr.microsoft.com/dotnet/aspnet:10.0),
# so no apt-get install is required on top of this base image.
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
