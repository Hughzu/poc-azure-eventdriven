Command to deploy the arm template : 
az deployment group create --resource-group rg-events --template-file ServiceBusDeploy.json --parameters ServiceBusDeploy.parameters.json