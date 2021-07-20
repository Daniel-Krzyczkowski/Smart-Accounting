#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["DocumentAnalyzer/SmartAccounting.DocumentAnalyzer.API/SmartAccounting.DocumentAnalyzer.API.csproj", "DocumentAnalyzer/SmartAccounting.DocumentAnalyzer.API/"]
RUN dotnet restore "DocumentAnalyzer/SmartAccounting.DocumentAnalyzer.API/SmartAccounting.DocumentAnalyzer.API.csproj"
COPY . .
WORKDIR "/src/DocumentAnalyzer/SmartAccounting.DocumentAnalyzer.API"
RUN dotnet build "SmartAccounting.DocumentAnalyzer.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SmartAccounting.DocumentAnalyzer.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SmartAccounting.DocumentAnalyzer.API.dll"]