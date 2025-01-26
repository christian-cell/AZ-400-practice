# Create a new pulumi project
```
pulumi new azure-csharp -n MyProject
```
no questions
```
pulumi new azure-csharp -n MyProject --non-interactive --yes
```

# create stacks for project "users-functions":
```
pulumi stack init users-functions-dev
pulumi stack init users-functions-pre
pulumi stack init users-functions-pro
```

# select a stack
```
pulumi stack select users-functions-dev
pulumi stack select users-functions-pre
pulumi stack select users-functions-pro
```

# set up vars for users-functions-dev
```
pulumi config set projectName usersfdev --stack users-functions-dev
pulumi config set env dev --stack users-functions-dev
pulumi config set location northeurope --stack users-functions-dev
```

# set up vars for users-functions-pre
```
pulumi config set projectName usersfpre --stack users-functions-pre
pulumi config set env pre --stack users-functions-pre
pulumi config set location northeurope --stack users-functions-pre
```

# set up vars for users-functions-pro
```
pulumi config set projectName usersfpro --stack users-functions-pro
pulumi config set env pro --stack users-functions-pro
pulumi config set location northeurope --stack users-functions-pro
```

# create resources
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