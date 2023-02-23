﻿FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/DatasetCollector/DatasetCollector.csproj", "src/DatasetCollector/"]
RUN dotnet restore "src/DatasetCollector/DatasetCollector.csproj"
COPY . .
WORKDIR "/src/src/DatasetCollector"
RUN dotnet build "DatasetCollector.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DatasetCollector.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DatasetCollector.dll"]
