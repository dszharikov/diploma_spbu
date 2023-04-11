FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["DatasetCollector/DatasetCollector.csproj", "DatasetCollector/"]
RUN dotnet restore "DatasetCollector/DatasetCollector.csproj"
COPY . .
WORKDIR "/src/DatasetCollector"
RUN dotnet build "DatasetCollector.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DatasetCollector.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DatasetCollector.dll"]
