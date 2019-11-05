# Everco.Services.Aspen.Client
Versión alpha del cliente del servicio Aspen

```c#
IAutonomousApp autonomousClient = AutonomousApp.Initialize()
	.RoutingTo("https://localhost/api")
	.WithIdentity("MyApyKey", "MyApiSecret")
	.Authenticate(useCache: false)
	.GetClient();

autonomousClient.Settings.GetDocTypes().Dump("Autonomous Document Types");

IDelegatedApp delegatedClient = DelegatedApp.Initialize()
	.RoutingTo("https://localhost/api")
	.WithIdentity("MyApiKey", "MyApiSecret")
	.Authenticate("UserDocType", "UserDocNumber", "UserPassword", useCache: false)
	.GetClient();
		
delegatedClient.Settings.GetDocTypes().Dump("Delegated Document Types");
```
