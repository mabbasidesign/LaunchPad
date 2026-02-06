// appService.bicep
param name string
param location string

resource appServicePlan 'Microsoft.Web/serverfarms@2022-03-01' = {
  name: '${name}-plan'
  location: location
  sku: {
    name: 'P1v2'
    tier: 'PremiumV2'
    size: 'P1v2'
  }
  kind: 'app'
}

resource webApp 'Microsoft.Web/sites@2022-03-01' = {
  name: name
  location: location
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
  }
}

output webAppHostName string = webApp.properties.defaultHostName
