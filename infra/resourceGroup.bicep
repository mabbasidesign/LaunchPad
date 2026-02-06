targetScope = 'subscription'

param rgName string
param location string = 'canadaeast'

resource rg 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: rgName
  location: location
}
