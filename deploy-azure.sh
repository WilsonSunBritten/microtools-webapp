#!/bin/bash

# This script deploys the Microtools Webapp to Azure
# Prerequisites:
# - Azure CLI installed and logged in
# - .NET 8 SDK installed

# Exit immediately if a command exits with a non-zero status
set -e

# Configuration
RESOURCE_GROUP="microtools-rg"
LOCATION="westus"
STORAGE_ACCOUNT="microtoolsv2storage"
FUNCTION_APP_NAME="microtoolsv2api"
FUNCTION_STORAGE_ACCOUNT="microtoolsv2funcstorage"
APP_SERVICE_PLAN="microtoolsv2plan"

echo "Deploying Microtools Webapp to Azure..."

# Check if resource group exists
if az group show --name $RESOURCE_GROUP &>/dev/null; then
  echo "Resource group $RESOURCE_GROUP already exists."
else
  echo "Creating resource group..."
  az group create --name $RESOURCE_GROUP --location $LOCATION
fi

# Check if storage account exists
if az storage account show --name $STORAGE_ACCOUNT --resource-group $RESOURCE_GROUP &>/dev/null; then
  echo "Storage account $STORAGE_ACCOUNT already exists."
else
  echo "Creating storage account..."
  az storage account create \
    --name $STORAGE_ACCOUNT \
    --resource-group $RESOURCE_GROUP \
    --location $LOCATION \
    --sku Standard_LRS \
    --kind StorageV2
fi

# Get storage account key
echo "Getting storage account key..."
STORAGE_KEY=$(az storage account keys list \
  --account-name $STORAGE_ACCOUNT \
  --resource-group $RESOURCE_GROUP \
  --query "[0].value" \
  --output tsv)

# Create a container for microtools if it doesn't exist
echo "Creating microtools container..."
az storage container create \
  --name microtools \
  --account-name $STORAGE_ACCOUNT \
  --account-key "$STORAGE_KEY" \
  --public-access off

# Enable static website hosting on the storage account
echo "Enabling static website hosting..."
az storage blob service-properties update \
  --account-name $STORAGE_ACCOUNT \
  --account-key "$STORAGE_KEY" \
  --static-website \
  --index-document index.html \
  --404-document index.html

# Check if function storage account exists
if az storage account show --name $FUNCTION_STORAGE_ACCOUNT --resource-group $RESOURCE_GROUP &>/dev/null; then
  echo "Function storage account $FUNCTION_STORAGE_ACCOUNT already exists."
else
  echo "Creating function storage account..."
  az storage account create \
    --name $FUNCTION_STORAGE_ACCOUNT \
    --resource-group $RESOURCE_GROUP \
    --location $LOCATION \
    --sku Standard_LRS \
    --kind StorageV2
fi

# Create App Service Plan
echo "Creating App Service Plan..."
az appservice plan create \
  --name $APP_SERVICE_PLAN \
  --resource-group $RESOURCE_GROUP \
  --location $LOCATION \
  --is-linux \
  --sku B1

# Wait for the App Service Plan to be created
echo "Waiting for App Service Plan to be created..."
sleep 10

# Create Function App
echo "Creating function app..."
az functionapp create \
  --name $FUNCTION_APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --storage-account $FUNCTION_STORAGE_ACCOUNT \
  --plan $APP_SERVICE_PLAN \
  --runtime dotnet \
  --os-type Linux

# Wait for the Function App to be created
echo "Waiting for Function App to be created..."
sleep 10

# Configure .NET version for the function app
echo "Configuring .NET version for function app..."
az functionapp config appsettings set \
  --name $FUNCTION_APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --settings \
    "FUNCTIONS_EXTENSION_VERSION=~4" \
    "WEBSITE_RUN_FROM_PACKAGE=1" \
    "DOTNET_VERSION=8.0" \
    "AzureWebJobsStorage=DefaultEndpointsProtocol=https;AccountName=$FUNCTION_STORAGE_ACCOUNT;AccountKey=$(az storage account keys list --account-name $FUNCTION_STORAGE_ACCOUNT --resource-group $RESOURCE_GROUP --query "[0].value" --output tsv);EndpointSuffix=core.windows.net"

# Configure CORS for the function app
echo "Configuring CORS for function app..."
az functionapp cors add \
  --name $FUNCTION_APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --allowed-origins "*"

# Build the web application
echo "Building web application..."
dotnet publish src/MicrotoolsWebApp.Web -c Release

# Get the storage account web endpoint
STORAGE_WEB_ENDPOINT=$(az storage account show \
  --name $STORAGE_ACCOUNT \
  --resource-group $RESOURCE_GROUP \
  --query "primaryEndpoints.web" \
  --output tsv)

# Get the function app URL
FUNCTION_APP_URL=$(az functionapp show \
  --name $FUNCTION_APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --query "defaultHostName" \
  --output tsv)

# Update appsettings.json with the API URL
echo "Updating appsettings.json..."
cat > src/MicrotoolsWebApp.Web/bin/Release/net8.0/publish/wwwroot/appsettings.json << EOF
{
  "ApiUrl": "https://${FUNCTION_APP_URL}/api/"
}
EOF

# Upload the web application to the storage account
echo "Uploading web application..."
az storage blob upload-batch \
  --account-name $STORAGE_ACCOUNT \
  --account-key "$STORAGE_KEY" \
  --source src/MicrotoolsWebApp.Web/bin/Release/net8.0/publish/wwwroot \
  --destination '$web'

# Build the API application
echo "Building API application..."
dotnet publish src/MicrotoolsWebApp.Functions -c Release

# Deploy the API application to the function app
echo "Deploying API application to function app..."
cd src/MicrotoolsWebApp.Functions/bin/Release/net8.0/publish
zip -r ../../../../../function-app.zip .
cd ../../../../../
az functionapp deployment source config-zip \
  --name $FUNCTION_APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --src function-app.zip

echo "Deployment completed successfully!"
echo "Web URL: ${STORAGE_WEB_ENDPOINT}"
echo "API URL: https://${FUNCTION_APP_URL}/api/"
echo ""
echo "You can now access your application at: ${STORAGE_WEB_ENDPOINT}"
