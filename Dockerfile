#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
#ENV DBSTRING "Data Source=192.168.178.130,1433;Initial Catalog=Proverb;Persist Security Info=True;User ID=SA;Password=yourStrong(!)Password;TrustServerCertificate=True"
#ENV KAFKABROKER "localhost:29092"
#ENV KAFKATOPIC "payment"
#ENV WEBAPIPORT 8080
EXPOSE $WEBAPIPORT

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["tg-payment-webapi/Presentation.csproj", "tg-payment-webapi/"]
COPY ["Core/Core.csproj", "Core/"]
COPY ["DataAccess/DataAccess.csproj", "DataAccess/"]
RUN dotnet restore "./tg-payment-webapi/./Presentation.csproj"
COPY . .
WORKDIR "/src/tg-payment-webapi"
RUN dotnet build "./Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Presentation.dll"]