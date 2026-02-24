// sqlDatabase.bicep

param name string
param databaseName string
param location string
param administratorLogin string
@secure()
param administratorLoginPassword string
param sqlSkuName string = 'Basic'
param sqlSkuTier string = 'Basic'
param sqlSkuCapacity int = 5
param maxSizeBytes int = 2147483648

resource sqlServer 'Microsoft.Sql/servers@2022-02-01-preview' = {
  name: name
  location: location
  properties: {
    administratorLogin: administratorLogin
    administratorLoginPassword: administratorLoginPassword
    version: '12.0'
  }
}

resource sqlDb 'Microsoft.Sql/servers/databases@2022-02-01-preview' = {
  name: '${name}/${databaseName}'
  location: location
  sku: {
    name: sqlSkuName
    tier: sqlSkuTier
    capacity: sqlSkuCapacity
  }
  properties: {
    collation: 'SQL_Latin1_General_CP1_CI_AS'
    maxSizeBytes: maxSizeBytes
    createMode: 'Default'
  }
}

output sqlServerName string = sqlServer.name
output sqlDbName string = sqlDb.name
