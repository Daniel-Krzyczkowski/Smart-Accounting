#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["ProcessedDocument/SmartAccounting.ProcessedDocument.API/SmartAccounting.ProcessedDocument.API.csproj", "ProcessedDocument/SmartAccounting.ProcessedDocument.API/"]
RUN dotnet restore "ProcessedDocument/SmartAccounting.ProcessedDocument.API/SmartAccounting.ProcessedDocument.API.csproj"
COPY . .
WORKDIR "/src/ProcessedDocument/SmartAccounting.ProcessedDocument.API"
RUN dotnet build "SmartAccounting.ProcessedDocument.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SmartAccounting.ProcessedDocument.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SmartAccounting.ProcessedDocument.API.dll"]