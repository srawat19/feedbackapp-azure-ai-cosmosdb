using './feedback-main.bicep'

param location  = 'australiaeast'

param cosmosDbAccountName  = 'cosmos-acc-feedback'

param cosmosDbName  = 'cosmos-db-feedback'

param cosmosContainerName  ='feedbacks'

param cosmosContainerName2 = 'admin-events' 

param partitionKeyPath  ='/eventId'

param appServicePlanName   = 'asp-feedback'

param webAppName  = 'feedbackapp'

param vaultName='vault-app'

param vaultSecretName = 'textAnalyticsKey'

param textAnalyticsServiceName = 'textAnalytics-service-feedback'

param partitionKeyPathAdmin  ='/email'

