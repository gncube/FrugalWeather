## Blazor WASM Project Creation

The BlazorStarter project was created using the following command:

```bash
dotnet new blazorwasm -n FrugalWeather.Client -au IndividualB2C -o src/Client -p -e
```

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


## Configure Function App settings

After creating the Function App, ensure it has a valid storage connection string in the `AzureWebJobsStorage` app setting. The CLI normally sets this when you pass `--storage-account`, but verify it if you encounter trigger sync errors.

```bash
# Get the storage account connection string (replace names if different)
conn=$(az storage account show-connection-string --resource-group rg-frugalweather --name stfrugalweather --query connectionString -o tsv)

# Set the Function App app settings (ensures storage + worker runtime are set)
az functionapp config appsettings set --resource-group rg-frugalweather --name func-frugalweather --settings AzureWebJobsStorage="$conn" FUNCTIONS_WORKER_RUNTIME=dotnet-isolated

# Verify the app settings
az functionapp config appsettings list --resource-group rg-frugalweather --name func-frugalweather -o table
```

Notes:
- Do not use `UseDevelopmentStorage=true` for deployed apps; that value is only valid when running locally against the storage emulator.
- If you see "Error calling sync triggers (BadRequest)" after publishing, verify that `AzureWebJobsStorage` is present and points to an accessible storage account. Also check the Function App logs:

```bash
# Stream logs to help diagnose runtime errors
az webapp log tail --name func-frugalweather --resource-group rg-frugalweather
```

Add the Deployment to Azure

# From the src/API directory

func azure functionapp publish func-frugalweather

# The output will show your function URLs
