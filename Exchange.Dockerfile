FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["DexWallet.Exchange/DexWallet.Exchange.csproj", "DexWallet.Exchange/"]
COPY ["DexWallet.Core/DexWallet.Core.csproj", "DexWallet.Core/"]
COPY ["DexWallet.Common/DexWallet.Common.csproj", "DexWallet.Common/"]
RUN dotnet restore "DexWallet.Exchange/DexWallet.Exchange.csproj"
RUN dotnet restore "DexWallet.Core/DexWallet.Core.csproj"
RUN dotnet restore "DexWallet.Common/DexWallet.Common.csproj"
COPY . .
WORKDIR "/src/DexWallet.Exchange"
RUN dotnet build "DexWallet.Exchange.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DexWallet.Exchange.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DexWallet.Exchange.dll"]