#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Presentations/AuthMicroService/AccountMicroService.csproj", "Presentations/AuthMicroService/"]
COPY ["Entities/Entities.csproj", "Entities/"]
COPY ["Services/Services.csproj", "Services/"]
COPY ["Repositories/Repositories.csproj", "Repositories/"]
RUN dotnet restore "Presentations/AuthMicroService/AccountMicroService.csproj"
COPY . .
WORKDIR "/src/Presentations/AuthMicroService"
RUN dotnet build "AccountMicroService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AccountMicroService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AccountMicroService.dll"]