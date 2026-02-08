// appService.bicep

param name string
param location string
param skuName string = 'B1'
param sqlServerName string
param sqlDbName string

resource appServicePlan 'Microsoft.Web/serverfarms@2022-03-01' = {
  name: '${name}-plan'
  location: location
  sku: {
    name: skuName
    tier: (skuName == 'B1' ? 'Basic' : 'PremiumV2')
    size: skuName
  }
  kind: 'app'
}

resource webApp 'Microsoft.Web/sites@2022-03-01' = {
  name: name
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      netFrameworkVersion: 'v8.0'
      appSettings: [
        {
          name: 'ConnectionStrings__DefaultConnection'
          value: 'Server=${sqlServerName}.database.windows.net;Database=${sqlDbName};Authentication=Active Directory Managed Identity;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
        }
      ]
    }
  }
}

output webAppHostName string = webApp.properties.defaultHostName
