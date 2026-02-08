// main.bicep
// Entry point for deploying all Azure resources for LaunchPad app

// Parameters
param location string = 'canadawest'
param appServiceName string = 'launchpad-appsvc'
param sqlServerName string = 'launchpad-sqlsrv'
param sqlDbName string = 'launchpaddb'
param redisName string = 'launchpad-redis'
param storageName string = 'launchpadstorage'
param appInsightsName string = 'launchpad-ai'
param sqlAdminUser string
@secure()
param sqlAdminPassword string
param appServiceSku string = 'B1'

// Modules (to be created separately)
module appService 'appService.bicep' = {
  name: appServiceName
  params: {
    name: appServiceName
    location: location
    skuName: appServiceSku
    sqlServerName: sqlServerName
    sqlDbName: sqlDbName
  }
}

module sqlDb 'sqlDatabase.bicep' = {
  name: sqlServerName
  params: {
    name: sqlServerName
    databaseName: sqlDbName
    location: location
    administratorLogin: sqlAdminUser
    administratorLoginPassword: sqlAdminPassword
  }
}

// module redis 'redis.bicep' = {
//   name: redisName
//   params: {
//     location: location
//   }
// }

// module storage 'storage.bicep' = {
//   name: storageName
//   params: {
//     location: location
//   }
// }

// module appInsights 'appInsights.bicep' = {
//   name: appInsightsName
//   params: {
//     location: location
//   }
// }


output appUrl string = 'https://${appService.outputs.webAppHostName}'
output sqlServerName string = sqlDb.outputs.sqlServerName
output sqlDbName string = sqlDb.outputs.sqlDbName
