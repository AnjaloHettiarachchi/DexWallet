FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["DexWallet.Core/DexWallet.Core.csproj", "DexWallet.Core/"]
RUN dotnet restore "DexWallet.Core/DexWallet.Core.csproj"
COPY . .
WORKDIR "/src/DexWallet.Core"
RUN dotnet build "DexWallet.Core.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DexWallet.Core.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DexWallet.Core.dll"]
