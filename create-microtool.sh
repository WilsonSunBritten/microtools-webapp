#!/bin/bash

# Check if a name was provided
if [ -z "$1" ]; then
  echo "Usage: ./create-microtool.sh <microtool-name>"
  exit 1
fi

# Convert the name to kebab-case for the directory and ID
MICROTOOL_NAME=$1
MICROTOOL_ID=$(echo $MICROTOOL_NAME | tr '[:upper:]' '[:lower:]' | tr ' ' '-')
MICROTOOL_DIR="microtools/$MICROTOOL_ID"

# Check if the directory already exists
if [ -d "$MICROTOOL_DIR" ]; then
  echo "Error: Microtool directory already exists: $MICROTOOL_DIR"
  exit 1
fi

# Create the microtool directory
mkdir -p "$MICROTOOL_DIR"

# Create metadata.yaml
cat > "$MICROTOOL_DIR/metadata.yaml" << EOL
id: $MICROTOOL_ID
name: $MICROTOOL_NAME
description: Description of the $MICROTOOL_NAME microtool
version: 1.0.0
author: Your Name
tags:
  - utility
EOL

# Create ui-schema.yaml with a basic UI
cat > "$MICROTOOL_DIR/ui-schema.yaml" << EOL
components:
  - type: textinput
    id: input
    label: Input
    placeholder: Enter input here
    required: true

  - type: button
    id: executeButton
    label: Execute

  - type: display
    id: result
    label: Result
EOL

# Create a new .NET class library project
dotnet new classlib -n $MICROTOOL_ID -o "$MICROTOOL_DIR/$MICROTOOL_ID"

# Add reference to the Core project
dotnet add "$MICROTOOL_DIR/$MICROTOOL_ID/$MICROTOOL_ID.csproj" reference src/MicrotoolsWebApp.Core/MicrotoolsWebApp.Core.csproj

# Create the microtool implementation
CLASS_NAME=$(echo $MICROTOOL_NAME | sed -r 's/(^| )([a-z])/\U\2/g' | tr -d ' ')
cat > "$MICROTOOL_DIR/$MICROTOOL_ID/$CLASS_NAME.cs" << EOL
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MicrotoolsWebApp.Core;

namespace $MICROTOOL_ID
{
    public class $CLASS_NAME : IMicrotool
    {
        public string Id => "$MICROTOOL_ID";

        public string Name => "$MICROTOOL_NAME";

        public string Description => "Description of the $MICROTOOL_NAME microtool";

        public Task<object> ExecuteAsync(IDictionary<string, object> parameters)
        {
            // Get the input
            if (!parameters.TryGetValue("input", out var inputObj) || inputObj == null)
            {
                throw new ArgumentException("Input is required");
            }

            string input = inputObj.ToString();

            // Process the input (replace this with your actual logic)
            string result = $"Processed: {input}";

            // Return the result
            return Task.FromResult<object>(new
            {
                ProcessedResult = result
            });
        }
    }
}
EOL

# Build the project
(cd "$MICROTOOL_DIR/$MICROTOOL_ID" && dotnet build -c Release)

# Copy the DLLs to the microtool directory
cp "$MICROTOOL_DIR/$MICROTOOL_ID/bin/Release/net8.0/$MICROTOOL_ID.dll" "$MICROTOOL_DIR/"
cp "src/MicrotoolsWebApp.Core/bin/Release/net8.0/MicrotoolsWebApp.Core.dll" "$MICROTOOL_DIR/"
cp "/home/codespace/.nuget/packages/yamldotnet/16.3.0/lib/net8.0/YamlDotNet.dll" "$MICROTOOL_DIR/"

echo "Microtool '$MICROTOOL_NAME' created successfully in $MICROTOOL_DIR"
echo "You can now customize the implementation in $MICROTOOL_DIR/$MICROTOOL_ID/$CLASS_NAME.cs"
