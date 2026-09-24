param location string
param tags object
param appServiceName string
param appServicePlanName string
param appInsightsConnectionString string
param sqlServerName string
param sqlDatabaseName string
@secure()
param jwtKey string
@secure()
param gmailAppPassword string
@secure()
param googleClientSecret string
@secure()
param payFastMerchantKey string
@secure()
param payFastPassPhrase string
@secure()
param openRouterApiKey string
@secure()
param stripeSecretKey string

resource plan 'Microsoft.Web/serverfarms@2023-12-01' = {
  name: appServicePlanName
  location: location
  tags: tags
  sku: {
    name: 'S1'
    tier: 'Standard'
    size: 'S1'
    family: 'S'
    capacity: 1
  }
  properties: {
    reserved: true
  }
}

resource appService 'Microsoft.Web/sites@2023-12-01' = {
  name: appServiceName
  location: location
  tags: tags
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: plan.id
    httpsOnly: true
    siteConfig: {
      minTlsVersion: '1.2'
      ftpsState: 'Disabled'
      scmType: 'None'
      alwaysOn: true
      linuxFxVersion: 'DOTNETCORE|9.0'
      appSettings: [
        { name: 'SCM_DO_BUILD_DURING_DEPLOYMENT', value: 'true' }
        { name: 'ENABLE_ORYX_BUILD', value: 'true' }
        { name: 'ORYX_DISABLE_COMPRESSION', value: 'true' }
        { name: 'APPLICATIONINSIGHTS_CONNECTION_STRING', value: appInsightsConnectionString }
        { name: 'ConnectionStrings__PortalDatabase', value: format('Server=tcp:{0}.{1},1433;Initial Catalog={2};Encrypt=True;TrustServerCertificate=False;Authentication=Active Directory Managed Identity;Connection Timeout=30;', sqlServerName, environment().suffixes.sqlServerHostname, sqlDatabaseName) }
        { name: 'Jwt__Key', value: jwtKey }
        { name: 'GmailSmtp__AppPassword', value: gmailAppPassword }
        { name: 'GoogleAuth__ClientSecret', value: googleClientSecret }
        { name: 'PayFast__MerchantKey', value: payFastMerchantKey }
        { name: 'PayFast__PassPhrase', value: payFastPassPhrase }
        { name: 'OpenRouter__ApiKey', value: openRouterApiKey }
        { name: 'Stripe__SecretKey', value: stripeSecretKey }
      ]
    }
  }
}

resource scmAuth 'Microsoft.Web/sites/basicPublishingCredentialsPolicies@2023-12-01' = {
  parent: appService
  name: 'scm'
  properties: {
    allow: true
  }
}

resource ftpAuth 'Microsoft.Web/sites/basicPublishingCredentialsPolicies@2023-12-01' = {
  parent: appService
  name: 'ftp'
  properties: {
    allow: false
  }
}

output appServiceName string = appService.name
output appServiceId string = appService.id
output principalId string = appService.identity.principalId
output defaultHostName string = appService.properties.defaultHostName
