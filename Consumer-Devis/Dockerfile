#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Consumer-Devis/Devis-Consumer.csproj", "Consumer-Devis/"]
RUN dotnet restore "Consumer-Devis/Devis-Consumer.csproj"
COPY . .
WORKDIR "/src/Consumer-Devis"
RUN dotnet build "Devis-Consumer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Devis-Consumer.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Devis-Consumer.dll"]