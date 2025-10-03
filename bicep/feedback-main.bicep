param cosmosDbAccountName string 
param location string
param cosmosDbName string 
param cosmosContainerName string 
param cosmosContainerName2 string 

param partitionKeyPath string 
param partitionKeyPathAdmin string
param appServicePlanName string
param webAppName string
param vaultName string
param vaultSecretName string
param textAnalyticsServiceName string

// use once and re-use:
var suffix = uniqueString(resourceGroup().id)



resource cosmosDbAccount 'Microsoft.DocumentDB/databaseAccounts@2025-04-15' = {
  name: '${cosmosDbAccountName}-${suffix}'
  location: location
  kind: 'GlobalDocumentDB'
  properties: {
    databaseAccountOfferType: 'Standard'
    enableFreeTier: true
    locations: [
      {
        locationName: location
        failoverPriority: 0
        isZoneRedundant: false
      }
    ]
    capabilities: [
      {
        name: 'EnableServerless'
      }
    ]

  }
}

resource cosmosDb 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2025-04-15' = {
  name: '${cosmosDbName}-${suffix}'
  parent: cosmosDbAccount
  properties: {
    resource: {
      id: '${cosmosDbName}-${suffix}'
    }
     
  }
}

resource cosmosDbContainer 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2025-04-15' = {
  name: '${cosmosContainerName}-${suffix}'
  parent : cosmosDb
  properties: {
    resource: {
      id:  '${cosmosContainerName}-${suffix}'
      partitionKey: {
        paths: [ partitionKeyPath]
        kind: 'Hash'

      }
    }
  }
}
 
resource cosmosDbAdminContainer 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2025-04-15' = {
  name: '${cosmosContainerName2}-${suffix}'
  parent : cosmosDb
  properties: {
    resource: {
      id:  '${cosmosContainerName2}-${suffix}'
      partitionKey: {
        paths: [ partitionKeyPathAdmin ]
        kind: 'Hash'

      }
    }
  }
}

resource appServicePlan 'Microsoft.Web/serverfarms@2024-04-01' = {
  name:'${appServicePlanName}-${suffix}'
  location: location
  sku: {
    name: 'F1'
    tier: 'Free'    
  }
  properties: {
    reserved: true
  }
  kind: 'linux'
}

resource webApp 'Microsoft.Web/sites@2024-04-01' = {
  name: '${webAppName}-${suffix}'
  location: location
  kind: 'app,linux'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      linuxFxVersion: 'DOTNETCORE|8.0'
      alwaysOn: false
     
    }
    
  }
} 

resource cosmosDbContributorRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(webApp.id, 'CosmosDBContributorRole')
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', 'b24988ac-6180-42a0-ab88-20f7382dd24c') // Cosmos DB Account Contributor
    principalType: 'ServicePrincipal'
    principalId: webApp.identity.principalId
  }
  scope: cosmosDbAccount
}

resource azureTextAnalytics 'Microsoft.CognitiveServices/accounts@2023-05-01' = {
  name: '${textAnalyticsServiceName}-${suffix}'
  location: location
  kind: 'TextAnalytics'
  sku: {
    name: 'F0' //Free Tier
  }
  properties: {
    customSubDomainName: '${textAnalyticsServiceName}-${suffix}'
    publicNetworkAccess: 'Enabled'
  }
}

resource azureKeyVault 'Microsoft.KeyVault/vaults@2022-07-01' = {
  name: '${vaultName}-${suffix}'
  location: location
  properties: {
    sku: {
      family: 'A'
      name: 'standard'
    }
    tenantId: subscription().tenantId
    accessPolicies: [
      {
        tenantId: subscription().tenantId
        objectId: webApp.identity.principalId
        permissions: {
          keys: ['get', 'list']
          secrets: ['get', 'list']
        }
      }
    ]
  }
} 

// //List the keys of the Text Analytics resource like az cognitiveservices account keys list
// resource textAnalyticsListKey 'Microsoft.CognitiveServices/accounts/listKeys@2024-10-01' = {
//   name: 'azureTextAnalytics
//   parent: azureTextAnalytics

// }

// //List the keys of the Text Analytics resource like az cognitiveservices account keys list
// resource textAnalyticsKey 'Microsoft.CognitiveServices/accounts/listKeys/action@2024-10-01' = {
//   name: 'listKeys'
//   parent: azureTextAnalytics

// }
// Create a secret in Key Vault to store the Text Analytics key
resource secret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: azureKeyVault
  name: '${vaultSecretName}-${suffix}'
  properties: {
    value : azureTextAnalytics.listKeys().key1
    //value: textAnalyticsKey.properties.key1
  }
}

resource keyVaultSecretUserRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(webApp.id, 'KeyVaultSecretUserRole')
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '4633458b-17de-408a-b874-0445c86b69e6') // Key Vault Secrets User Role Id
    principalType: 'ServicePrincipal'
    principalId: webApp.identity.principalId
  }
  scope: azureKeyVault
}




output webAppUrl string = webApp.properties.defaultHostName
output keyVaultName string = azureKeyVault.name
output textAnalyticsEndpoint string = azureTextAnalytics.properties.endpoint
