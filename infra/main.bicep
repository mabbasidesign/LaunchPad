// main.bicep
// Entry point for deploying all Azure resources for LaunchPad app

// Parameters
param location string // Location for App Service and other resources (e.g., Canada Central)
param sqlLocation string // Location for SQL Server (e.g., East US)
param appServiceName string = 'launchpad-appsvc'
param sqlServerName string = 'launchpad-sqlsrv'
param sqlDbName string = 'launchpaddb'
param sqlAdminUser string
@secure()
param sqlAdminPassword string
param appServiceSkuName string = 'F1'
param appServiceSkuTier string = 'Free'
param sqlSkuName string = 'Basic'
param sqlSkuTier string = 'Basic'
param sqlSkuCapacity int = 5
param sqlMaxSizeBytes int = 2147483648

// Modules (to be created separately)
module appService 'appService.bicep' = {
  name: appServiceName
  params: {
    name: appServiceName
    location: location
    skuName: appServiceSkuName
    skuTier: appServiceSkuTier
    sqlServerName: sqlServerName
    sqlDbName: sqlDbName
  }
}

module sqlDb 'sqlDatabase.bicep' = {
  name: sqlServerName
  params: {
    name: sqlServerName
    databaseName: sqlDbName
    location: sqlLocation // SQL in US region
    administratorLogin: sqlAdminUser
    administratorLoginPassword: sqlAdminPassword
    sqlSkuName: sqlSkuName
    sqlSkuTier: sqlSkuTier
    sqlSkuCapacity: sqlSkuCapacity
    maxSizeBytes: sqlMaxSizeBytes
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
