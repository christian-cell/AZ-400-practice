# First of all sync pulumi state with azure
```
pulumi refresh
```

# Set your subscriptionId
```
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

# create stacks for project "users":
```
pulumi stack init users-dev
pulumi stack init users-pre
pulumi stack init users-pro
```

# create stacks for project "products":
```
pulumi stack init products-dev
pulumi stack init products-pre
pulumi stack init products-pro
```

# select a stack
```
pulumi stack select users-dev
pulumi stack select users-pre
pulumi stack select users-pro
```

# set up vars for users-dev
```
pulumi config set projectName users --stack users-dev
pulumi config set env dev --stack users-dev
pulumi config set location westeurope --stack users-dev
```

# set up vars for users-pre
```
pulumi config set projectName users --stack users-pre
pulumi config set env pre --stack users-pre
pulumi config set location westeurope --stack users-pre
```

# set up vars for users-pro
```
pulumi config set projectName users --stack users-pro
pulumi config set env pro --stack users-pro
pulumi config set location westeurope --stack users-pro
```

# check selected stack
```
pulumi stack
```

# apply this
```
pulumi up --yes
```

to see where the resources are being created
```
pulumi up --verbose=5
```

4 while is running we can see logs or preview

C:\agent\_work\r1\a\_Users.API\drop\wwwroot\Users.API
```
pulumi logs
```

```
pulumi preview
```

5 we can cancel the process like this

```
pulumi cancel
```

6 Hard reset , delete all the resources in azure and pulumi state to sync from the beginning

```
pulumi destroy
```

7 Danger : you can clean state if you know what you do
```
pulumi stack export --file state.json
```

clean resource array  and import

```
pulumi stack import --file state.json
```

```
pulumi stack export
```

To change active directory where create the resource group and resources
```
 az login --service-principal -u <client-id> -p <service-principal-secret> --tenant <tenant-id>
```

Set pulumi to use subscription
```
pulumi config set azure-native:subscriptionId <your_subscription_id>
```

Set pulumi to use new directory
```
pulumi config set azure-native:tenantId <your_tenant_id>
```