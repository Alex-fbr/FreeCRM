#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["TelegramBot.Worker1/TelegramBot.Worker1.csproj", "TelegramBot.Worker1/"]
RUN dotnet restore "TelegramBot.Worker1/TelegramBot.Worker1.csproj"
COPY . .
WORKDIR "/src/TelegramBot.Worker1"
RUN dotnet build "TelegramBot.Worker1.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TelegramBot.Worker1.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TelegramBot.Worker1.dll"]