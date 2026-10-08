# Stage 1: Build & Publish
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj first to leverage Docker layer caching for restored dependencies
COPY ["TaskManager.API.csproj", "./"]
RUN dotnet restore "TaskManager.API.csproj"

# Copy the rest of the backend source files and build
COPY . .
RUN dotnet build "TaskManager.API.csproj" -c Release -o /app/build

RUN dotnet publish "TaskManager.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime Environment
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Expose HTTP port
EXPOSE 5000

# Set ASP.NET Core listening port
ENV ASPNETCORE_URLS=http://+:5000

# Copy compiled binaries from the build stage
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "TaskManager.API.dll"]