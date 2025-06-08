FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /app

COPY . ./

RUN dotnet restore

RUN dotnet build

CMD ["dotnet", "ef", "database", "update","&&","dotnet", "run"]