targetScope = 'subscription'

param environmentName string
param location string
param sessionId string
param deployedBy string
param createdAt string
param appServiceName string
param appServicePlanName string
param sqlServerName string
param sqlDatabaseName string
param appInsightsName string
param logAnalyticsName string

@secure()
param jwtKey string = ''
@secure()
param gmailAppPassword string = ''
@secure()
param googleClientSecret string = ''
@secure()
param payFastMerchantKey string = ''
@secure()
param payFastPassPhrase string = ''
@secure()
param openRouterApiKey string = ''
@secure()
param stripeSecretKey string = ''

var tags = {
  'app-onboard-skill': 'true'
  'app-onboard-session-id': sessionId
  'created-at': createdAt
  environment: environmentName
  'deployed-by': deployedBy
}

resource rg 'Microsoft.Resources/resourceGroups@2023-07-01' = {
  name: 'rg-vitalityportal-dev-713f'
  location: location
  tags: tags
}

module monitoring './modules/monitoring.bicep' = {
  name: 'monitoring'
  scope: rg
  params: {
    location: location
    tags: tags
    appInsightsName: appInsightsName
    logAnalyticsName: logAnalyticsName
  }
}

module app './modules/app-service.bicep' = {
  name: 'app-service'
  scope: rg
  params: {
    location: location
    tags: tags
    appServiceName: appServiceName
    appServicePlanName: appServicePlanName
    appInsightsConnectionString: monitoring.outputs.appInsightsConnectionString
    sqlServerName: sqlServerName
    sqlDatabaseName: sqlDatabaseName
    jwtKey: jwtKey
    gmailAppPassword: gmailAppPassword
    googleClientSecret: googleClientSecret
    payFastMerchantKey: payFastMerchantKey
    payFastPassPhrase: payFastPassPhrase
    openRouterApiKey: openRouterApiKey
    stripeSecretKey: stripeSecretKey
  }
}

module sql './modules/sql-database.bicep' = {
  name: 'sql-database'
  scope: rg
  params: {
    location: location
    tags: tags
    sqlServerName: sqlServerName
    sqlDatabaseName: sqlDatabaseName
    sqlAdminPrincipalId: app.outputs.principalId
    sqlAdminPrincipalName: appServiceName
  }
}

output resourceGroupName string = rg.name
output appServiceName string = app.outputs.appServiceName
output appServiceHostName string = app.outputs.defaultHostName
output sqlServerFqdn string = sql.outputs.fullyQualifiedDomainName
