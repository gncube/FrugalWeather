The Azure Functions API was created using the command:

```bash
func init src/API --worker-runtime dotnet-isolated --target-framework net10.0 --name FrugalWeather.API
```

# Login to Azure

az login

# Set your subscription (if you have multiple)

az account set --subscription "YOUR_SUBSCRIPTION_NAME_OR_ID"

# Create a resource group

az group create --name rg-frugalweather --location uksouth

# Create a storage account (must be globally unique)

az storage account create \
  --name stfrugalweather \
  --resource-group rg-frugalweather \
  --location uksouth \
  --sku Standard_LRS

# Create a Function App

az functionapp create \
  --resource-group rg-frugalweather \
  --consumption-plan-location uksouth \
  --runtime dotnet-isolated \
  --runtime-version 10 \
  --functions-version 4 \
  --name func-frugalweather \
  --storage-account stfrugalweather \
  --os-type Linux
