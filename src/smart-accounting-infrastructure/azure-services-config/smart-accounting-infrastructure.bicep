param location string = resourceGroup().location
param nameSuffix string ='dev'
param tenantId string = ''
param sqlAdministratorLogin string = ''
param sqlAdministratorPassword string = ''
param publisherName string = ''
param publisherEmail string = ''

resource storageAccount 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: 'stsmartaccounting${nameSuffix}'
  location: location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_ZRS'
  }
  properties: {
    accessTier: 'Hot'
    supportsHttpsTrafficOnly: true
  }
}

resource containerRegistry 'Microsoft.ContainerRegistry/registries@2019-05-01' = {
  name: 'acrsmartaccounting${nameSuffix}'
  location: location
  sku: {
    name: 'Basic'
  }
}


resource keyVault 'Microsoft.KeyVault/vaults@2019-09-01' = {
  name: 'kv-smart-accounting-${nameSuffix}'
  location: location
  properties:{
    sku: {
      name: 'standard'
      family: 'A'
    }
    tenantId: tenantId
    accessPolicies:[]
  }
}

resource formRecognizer 'Microsoft.CognitiveServices/accounts@2021-04-30' = {
  name: 'formrec-smart-accounting-${nameSuffix}'
  location: location
  sku: {
    name: 'S0'
    tier: 'Standard'
  }
  kind:'FormRecognizer'
  properties: {
    customSubDomainName: 'formrec-smart-accounting-${nameSuffix}'
    networkAcls: {
        defaultAction: 'Allow'
        virtualNetworkRules: []
        ipRules: []
    }
    publicNetworkAccess: 'Enabled'
    restore: false
  }
}

resource applicationInsights 'Microsoft.Insights/components@2015-05-01' = {
  name: 'appi-smart-accounting-${nameSuffix}'
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    Flow_Type: 'Bluefield'
    Request_Source: 'rest'
    IngestionMode: 'ApplicationInsights'
  }
}

resource cosmosDatabase 'Microsoft.DocumentDB/databaseAccounts@2021-04-15' = {
  name: 'cosmos-smart-accounting-${nameSuffix}'
  location: location
  kind: 'GlobalDocumentDB'
  properties:{
    publicNetworkAccess: 'Enabled'
    enableAutomaticFailover: false
    enableMultipleWriteLocations: false
    databaseAccountOfferType: 'Standard'
    consistencyPolicy: {
      defaultConsistencyLevel: 'Session'
      maxIntervalInSeconds: 5
      maxStalenessPrefix: 100
   }
   locations: [
      {
          locationName: 'West Europe'
          provisioningState: 'Succeeded'
          failoverPriority: 0
          isZoneRedundant: false
      }
    ]
  }
}

resource sqlDatabaseServer 'Microsoft.Sql/servers@2021-02-01-preview' ={
  name: 'sql-smart-accounting-${nameSuffix}'
  location: location
  properties:{
    administratorLogin: sqlAdministratorLogin
    administratorLoginPassword: sqlAdministratorPassword
  }
}

resource sqlDatabase 'Microsoft.Sql/servers/databases@2021-02-01-preview'= {
  name: 'sqldb-smart-accounting-${nameSuffix}'
  location: location
  sku: {
    name: 'Standard'
    tier: 'Standard'
  }
  parent: sqlDatabaseServer
}

resource serviceBus 'Microsoft.ServiceBus/namespaces@2021-01-01-preview' ={
  name: 'sb-smart-accounting-${nameSuffix}'
  location: location
  sku: {
    name: 'Standard'
    tier: 'Standard'
  }
}

resource serviceBusTopic 'Microsoft.ServiceBus/namespaces/topics@2018-01-01-preview'={
  name: 'smart-accounting-events'
  parent: serviceBus
}

resource signalrService 'Microsoft.SignalRService/signalR@2021-04-01-preview' ={
  name: 'signalr-smart-accounting-${nameSuffix}'
  location: location
  sku:{
    name: 'Free_F1'
    tier: 'Free'
    capacity: 1
  }
  kind: 'SignalR'
  properties:{
      features: [
        {
          flag: 'ServiceMode'
          value: 'Default'
        }
      ]
  }
}

resource apiManagementService 'Microsoft.ApiManagement/service@2020-12-01' = {
  name: 'apim-smart-accounting-${nameSuffix}'
  location: location
  sku: {
    name: 'Developer'
    capacity: 1
  }
  properties: {
    publisherEmail: publisherEmail
    publisherName: publisherName
  }
}

resource appServicePlan 'Microsoft.Web/serverfarms@2021-01-15' = {
  name: 'plan-smart-accounting-${nameSuffix}'
  location: resourceGroup().location
  sku: {
    name: 'B1'
    capacity: 1
  }
} 

resource azureWebApp 'Microsoft.Web/sites@2021-01-15'= {
name: 'app-smart-accounting-${nameSuffix}'
location: location
properties:{
    serverFarmId: appServicePlan.id
  }
identity: {
    type: 'SystemAssigned'
  }
}
