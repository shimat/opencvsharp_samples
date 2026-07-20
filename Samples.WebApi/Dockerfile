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
# built with highgui disabled, so GTK3/X11/libatomic are gone. libdrm2 is still needed
# for now: the headless build still links libdrm.so.2 unconditionally, a bug tracked
# upstream at https://github.com/shimat/opencvsharp/issues/2071 — drop this apt-get
# step once that lands in a release.
RUN apt-get update \
    && apt-get install -y --no-install-recommends libdrm2 \
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
