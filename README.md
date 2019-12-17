# Everco.Services.Aspen.Client
Versión beta del cliente del servicio Aspen

```c#
var client = AutonomousApp.Initialize()
	.RoutingTo("https://localhost/api")
	.WithIdentity("MyApyKey", "MyApiSecret")
	.Authenticate()
	.GetClient();

var docTypes = client.Settings.GetDocTypes();
```


## [Guía de uso](https://aspenclient.readthedocs.io)
