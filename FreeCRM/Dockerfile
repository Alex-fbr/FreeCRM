FROM  mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["TelegramBot/TelegramBot.csproj", "TelegramBot/"]
COPY ["Common/Common.csproj", "Common/"]
RUN dotnet restore "TelegramBot/TelegramBot.csproj"
COPY . .
WORKDIR "/src/TelegramBot"
RUN dotnet build "TelegramBot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TelegramBot.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TelegramBot.dll"]
