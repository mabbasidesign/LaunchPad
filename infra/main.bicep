// main.bicep
// Entry point for deploying all Azure resources for LaunchPad app

// Parameters
param location string = resourceGroup().location
param appServiceName string = 'launchpad-appsvc'
param sqlServerName string = 'launchpad-sqlsrv'
param sqlDbName string = 'launchpaddb'
param redisName string = 'launchpad-redis'
param storageName string = 'launchpadstorage'
param appInsightsName string = 'launchpad-ai'
param sqlAdminUser string
param sqlAdminPassword string

// Modules (to be created separately)
module appService 'appService.bicep' = if (true) {
  name: appServiceName
  params: {
  name: appServiceName
  location: location
}
}

module sqlDb 'sqlDatabase.bicep' = {
  name: sqlServerName
  params: {
    name: sqlServerName
    location: location
    administratorLogin: sqlAdminUser
    administratorLoginPassword: sqlAdminPassword
  }
}

// module redis 'redis.bicep' = if (true) {
//   name: redisName
//   params: {
//     location: location
//   }
// }

// module storage 'storage.bicep' = if (true) {
//   name: storageName
//   params: {
//     location: location
//   }
// }

// module appInsights 'appInsights.bicep' = if (true) {
//   name: appInsightsName
//   params: {
//     location: location
//   }
// }

// Outputs (example)
// output appServiceHostName string = appService.outputs.hostName
