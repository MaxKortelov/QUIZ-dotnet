FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet build -c Release -o out

# Install EF Core tools
RUN dotnet tool install --global dotnet-ef --version 9.0.*
ENV PATH="${PATH}:/root/.dotnet/tools"

# Set explicit port
ENV ASPNETCORE_URLS=http://+:3001

# Make entrypoint executable
COPY entrypoint.sh .
RUN chmod +x entrypoint.sh

# Set entrypoint
ENTRYPOINT ["./entrypoint.sh"]