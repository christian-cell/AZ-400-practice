# First of all sync pulumi state with azure
```
pulumi refresh
```


# Set your subscriptionId
```
az login

az account set --subscription "YOUR_SUBSCRIPTION_NAME_OR_ID"
```

# or you can set the subscriptionId in pulumi
```
pulumi config set azure-native:subscriptionId YOUR_SUBSCRIPTION_ID
```

# check stack list
```
pulumi stack ls
```

# create stacks for project "users-container":
```
pulumi stack init users-container-dev
pulumi stack init users-container-pre
pulumi stack init users-container-pro
```

# select a stack
```
pulumi stack select users-container-dev
pulumi stack select users-container-pre
pulumi stack select users-container-pro
```

# set up vars for users-container-dev
```
pulumi config set projectName users --stack users-container-dev
pulumi config set env dev --stack users-container-dev
pulumi config set location northeurope --stack users-container-dev
pulumi config set registryName chrUsers --stack users-container-dev
```

# set up vars for users-container-pre
```
pulumi config set projectName users --stack users-container-pre
pulumi config set env pre --stack users-container-pre
pulumi config set location northeurope --stack users-container-pre
pulumi config set registryName chrUsers --stack users-container-pre
```

# set up vars for users-container-pro
```
pulumi config set projectName users --stack users-container-pro
pulumi config set env pro --stack users-container-pro
pulumi config set location northeurope --stack users-container-pro
pulumi config set registryName chrUsers --stack users-container-pro
```

# check selected stack
```
pulumi stack
```

# apply this
```
pulumi up --yes
```