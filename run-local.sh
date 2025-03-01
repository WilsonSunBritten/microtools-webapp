#!/bin/bash

# Start the Azure Functions project in the background
echo "Starting Azure Functions..."
cd src/MicrotoolsWebApp.Functions
dotnet run &
FUNCTIONS_PID=$!

# Wait a moment for the Functions to start
sleep 5

# Start the Blazor WebAssembly project
echo "Starting Blazor WebAssembly..."
cd ../MicrotoolsWebApp.Web
dotnet run

# When the Blazor app is stopped, also stop the Functions
kill $FUNCTIONS_PID
