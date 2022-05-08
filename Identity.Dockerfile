FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["DexWallet.Identity/DexWallet.Identity.csproj", "DexWallet.Identity/"]
COPY ["DexWallet.Common/DexWallet.Common.csproj", "DexWallet.Common/"]
RUN dotnet restore "DexWallet.Identity/DexWallet.Identity.csproj"
RUN dotnet restore "DexWallet.Common/DexWallet.Common.csproj"
COPY . .
WORKDIR "/src/DexWallet.Identity"
RUN dotnet build "DexWallet.Identity.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DexWallet.Identity.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DexWallet.Identity.dll"]