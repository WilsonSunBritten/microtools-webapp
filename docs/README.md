# Microtools Webapp Architecture Documentation

This directory contains PlantUML diagrams that illustrate the architecture of the Microtools Webapp. These diagrams provide a visual representation of the system's components, their interactions, and the overall architecture.

## Viewing the Diagrams

To view these diagrams, you can use one of the following methods:

1. **VS Code Extension**: Install the [PlantUML extension](https://marketplace.visualstudio.com/items?itemName=jebbs.plantuml) for VS Code, which allows you to preview the diagrams directly in the editor.

2. **PlantUML Web Server**: Upload the `.puml` files to the [PlantUML Web Server](http://www.plantuml.com/plantuml/uml/) to render them online.

3. **Local PlantUML Installation**: Install PlantUML locally and use the command-line interface to generate images from the `.puml` files.

## Diagram Descriptions

### Architecture Overview (`architecture.puml`)

This diagram provides a high-level overview of the system architecture, showing the main components and their relationships:

- **Azure Static Web App**: Hosts the Blazor WebAssembly frontend
- **Azure Functions**: Hosts the API endpoints, Microtool Registry, and Microtool Loader
- **Microtools Directory**: Contains the microtools, each with metadata, UI schema, and implementation
- **Azure AD B2C**: Provides authentication and authorization services

### Component Diagram (`components.puml`)

This diagram shows the internal components of the system and how they interact:

- **Frontend Components**: App, MicrotoolsService, MicrotoolCard, DynamicUiComponent, DynamicUiForm
- **Backend Components**: MicrotoolsApi, MicrotoolRegistry, MicrotoolLoader
- **Core Components**: IMicrotool interface, MicrotoolMetadata, UiSchema
- **Microtools**: TextTransformer and other microtools that implement the IMicrotool interface

### Sequence Diagram (`sequence.puml`)

This diagram illustrates the flow of execution when a user interacts with a microtool:

1. **Initialization**: The system discovers and loads microtools
2. **Home Page Navigation**: The user navigates to the home page and sees the available microtools
3. **Microtool Selection**: The user selects a microtool and sees its UI
4. **Microtool Execution**: The user enters parameters, executes the microtool, and sees the results

### Deployment Diagram (`deployment.puml`)

This diagram shows how the application is deployed to Azure:

- **Developer Workstation**: Where the code is developed and pushed to the Git repository
- **GitHub**: Hosts the repository and runs GitHub Actions for CI/CD
- **Azure**: Hosts the Static Web App, Functions, Storage, and AD B2C services

## Relationship to Code

The diagrams correspond to the following code components:

- **Frontend (Blazor WebAssembly)**:
  - `src/MicrotoolsWebApp.Web/`: Contains the Blazor WebAssembly application
  - `src/MicrotoolsWebApp.Web/Components/`: Contains the UI components
  - `src/MicrotoolsWebApp.Web/Services/`: Contains the MicrotoolsService

- **Backend (Azure Functions)**:
  - `src/MicrotoolsWebApp.Functions/`: Contains the Azure Functions application
  - `src/MicrotoolsWebApp.Functions/MicrotoolsApi.cs`: Contains the API endpoints

- **Core**:
  - `src/MicrotoolsWebApp.Core/`: Contains the core functionality
  - `src/MicrotoolsWebApp.Core/IMicrotool.cs`: Contains the IMicrotool interface
  - `src/MicrotoolsWebApp.Core/Models/`: Contains the metadata and UI schema models
  - `src/MicrotoolsWebApp.Core/MicrotoolRegistry.cs`: Contains the MicrotoolRegistry
  - `src/MicrotoolsWebApp.Core/MicrotoolLoader.cs`: Contains the MicrotoolLoader

- **Microtools**:
  - `microtools/`: Contains the microtools
  - `microtools/text-transformer/`: Contains the TextTransformer microtool
