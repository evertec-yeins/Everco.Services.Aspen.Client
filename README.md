# Everco.Services.Aspen.Client
Versión alpha del cliente del servicio Aspen

```c#
var client = AutonomousApp.Initialize()
	.RoutingTo("https://localhost/api")
	.WithIdentity("MyApyKey", "MyApiSecret")
	.Authenticate()
	.GetClient();

client.Settings.GetDocTypes();
```
