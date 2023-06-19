#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0-preview AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0-preview AS build
WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["OpcUa.Web/OpcUa.Web.csproj", "OpcUa.Web/"]
COPY ["OpcUa.Applications/OpcUa.Applications.csproj", "OpcUa.Applications/"]
COPY ["OpcUA.Domain/OpcUa.Domain.csproj", "OpcUA.Domain/"]
COPY ["OpcUa.Client.Applications/OpcUa.Client.Applications.csproj", "OpcUa.Client.Applications/"]
COPY ["OpcUa.Persistance/OpcUa.Persistance.csproj", "OpcUa.Persistance/"]
RUN dotnet restore "OpcUa.Web/OpcUa.Web.csproj"
COPY . .
WORKDIR "/src/OpcUa.Web"
RUN dotnet build "OpcUa.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OpcUa.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OpcUa.Web.dll"]