#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Notification/SmartAccounting.Notification.API/SmartAccounting.Notification.API.csproj", "Notification/SmartAccounting.Notification.API/"]
RUN dotnet restore "Notification/SmartAccounting.Notification.API/SmartAccounting.Notification.API.csproj"
COPY . .
WORKDIR "/src/Notification/SmartAccounting.Notification.API"
RUN dotnet build "SmartAccounting.Notification.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SmartAccounting.Notification.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SmartAccounting.Notification.API.dll"]