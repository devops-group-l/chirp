FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG configuration=Release

WORKDIR /src
COPY ["src/Chirp.WebService/Chirp.WebService.csproj", "src/Chirp.WebService/"]
# Install Node.js and npm
RUN curl -sL https://deb.nodesource.com/setup_16.x | bash -
RUN apt-get install -y nodejs

RUN dotnet restore "src/Chirp.WebService/Chirp.WebService.csproj"
COPY . .
WORKDIR "/src/src/Chirp.WebService"
RUN dotnet build "Chirp.WebService.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "Chirp.WebService.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Chirp.WebService.dll"]
