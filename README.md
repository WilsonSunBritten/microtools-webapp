# Microtools Webapp

A stateless web application for hosting and running microtools. Each microtool is a self-contained module that can be added to the application without any configuration beyond placing it in the microtools directory.

## Features

- **Automatic Discovery**: Microtools are automatically discovered and displayed on the home page
- **Dynamic UI Generation**: UI is generated based on the microtool's schema
- **Stateless Architecture**: Built on Azure technologies for a stateless architecture
- **Secure**: Authentication and authorization can be configured to control access

## Architecture

- **Backend**: .NET 8 Azure Functions
- **Frontend**: Blazor WebAssembly
- **Microtools**: Self-contained .NET assemblies with YAML configuration

### Architecture Diagrams

The following PlantUML diagrams illustrate the architecture of the Microtools Webapp:

- [Architecture Overview](docs/architecture.puml) - High-level overview of the system architecture
- [Component Diagram](docs/components.puml) - Detailed view of the system components and their interactions
- [Sequence Diagram](docs/sequence.puml) - Flow of execution when a user interacts with a microtool
- [Deployment Diagram](docs/deployment.puml) - Deployment architecture on Azure

To view these diagrams, you can use the PlantUML extension in VS Code or upload the files to the [PlantUML Web Server](http://www.plantuml.com/plantuml/uml/).

## Project Structure

- `src/MicrotoolsWebApp.Core`: Core functionality and shared code
- `src/MicrotoolsWebApp.Functions`: Azure Functions host for microtools
- `src/MicrotoolsWebApp.Web`: Blazor WebAssembly frontend
- `src/MicrotoolsWebApp.Build`: Build-time tools for microtool discovery
- `microtools/`: Directory containing all microtools

## Getting Started

### Prerequisites

- .NET 8 SDK
- Azure Functions Core Tools (optional for local development)

### Running Locally

You can run the application using the provided script:

```bash
chmod +x run-local.sh
./run-local.sh
```

This script will:
1. Start the backend API (running on http://localhost:5000)
2. Start the Blazor WebAssembly frontend (running on http://localhost:5085)

Alternatively, you can start the components manually:

1. Start the backend API:

```bash
cd src/MicrotoolsWebApp.Functions
dotnet run
```

2. Start the Blazor WebAssembly frontend:

```bash
cd src/MicrotoolsWebApp.Web
dotnet run
```

3. Open your browser and navigate to `http://localhost:5085` to access the frontend

## Creating a Microtool

1. Create a new directory in the `microtools` folder with your microtool name
2. Create a `metadata.yaml` file with information about your microtool:

```yaml
id: your-microtool-id
name: Your Microtool Name
description: Description of what your microtool does
version: 1.0.0
author: Your Name
tags:
  - tag1
  - tag2
```

3. Create a `ui-schema.yaml` file to define the UI for your microtool:

```yaml
components:
  - type: textinput
    id: inputField
    label: Input Field
    placeholder: Enter text here
    required: true
  
  - type: button
    id: submitButton
    label: Submit
  
  - type: display
    id: result
    label: Result
```

4. Create a .NET class library that implements the `IMicrotool` interface
5. Build the class library and copy the DLL to your microtool directory
6. Restart the application to discover your new microtool

## Deployment

The application can be deployed to Azure using:

- Azure Static Web Apps for the frontend
- Azure Functions for the backend
- Azure AD B2C for authentication and authorization

## License

This project is licensed under the MIT License - see the LICENSE file for details.
