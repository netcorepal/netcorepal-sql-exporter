#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["NuGet.config", "."]
COPY ["src/NetCorePal.SqlExporter/NetCorePal.SqlExporter.csproj", "src/NetCorePal.SqlExporter/"]
RUN dotnet restore "src/NetCorePal.SqlExporter/NetCorePal.SqlExporter.csproj"
COPY . .
WORKDIR "/src/src/NetCorePal.SqlExporter"
RUN dotnet build "NetCorePal.SqlExporter.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NetCorePal.SqlExporter.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NetCorePal.SqlExporter.dll"]