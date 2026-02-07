// sqlDatabase.bicep
param name string
param location string
param administratorLogin string
param administratorLoginPassword string

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
  name: '${name}/${name}db'
  location: location
  properties: {
    collation: 'SQL_Latin1_General_CP1_CI_AS'
    maxSizeBytes: 2147483648
    sampleName: 'AdventureWorksLT'
  }
  dependsOn: [sqlServer]
}

output sqlServerName string = sqlServer.name
output sqlDbName string = sqlDb.name
