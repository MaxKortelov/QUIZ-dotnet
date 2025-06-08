#!/bin/sh

# Apply database migrations
echo "Applying database migrations..."
dotnet ef database update --project ./quiz-api.csproj

# Start application on port 3001
echo "Starting application on port 3001..."
dotnet run --project ./quiz-api.csproj --urls "http://0.0.0.0:3001"