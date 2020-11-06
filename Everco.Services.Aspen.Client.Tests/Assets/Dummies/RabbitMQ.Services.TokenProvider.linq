<Query Kind="Program">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <NuGetReference>RabbitMQ.Client</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
  <Namespace>RabbitMQ.Client</Namespace>
  <Namespace>RabbitMQ.Client.Events</Namespace>
  <Namespace>System</Namespace>
  <Namespace>System.Net</Namespace>
</Query>

void Main()
{
	string flagPath = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), $"{Util.CurrentQuery.Name}.flag");
	StringBuilder flagBluider = new StringBuilder();
	string debugMessage = $"[x] Starting RpcServer [{Util.CurrentQuery.Name}]...";
	flagBluider.AppendLine(debugMessage);
	Console.WriteLine(debugMessage);

	Util.ClearResults();
	try
	{
		ConnectionFactory factory = new ConnectionFactory()
		{
			HostName = "localhost",
			UserName = "test",
			Password = "test",
			VirtualHost = "TokenProvider"
		};

		debugMessage = $"[x] Using connection: 'host={factory.HostName};virtualhost={factory.VirtualHost};username={factory.UserName};password={factory.Password};exchange={this.Exchange}'";
		Console.WriteLine(debugMessage);
		flagBluider.AppendLine(debugMessage);
		using (IConnection connection = factory.CreateConnection("TokenProvider Dummy RPC Server"))
		{
			using (IModel channel = connection.CreateModel())
			{
				channel.ExchangeDeclare(exchange: this.Exchange, type: ExchangeType.Direct, durable: false, autoDelete: true, arguments: null);
				this.GenerateToken(channel);
				this.RedeemToken(channel);
				this.NullifyToken(channel);

				debugMessage = "[x] Awaiting requests...";
				Console.WriteLine(debugMessage);
				flagBluider.AppendLine(debugMessage);

				File.WriteAllText(flagPath, flagBluider.ToString());
				Console.WriteLine("[i] Press [enter] to exit.");
				Console.ReadLine();
			}
		}
	}
	finally
	{
		if (File.Exists(flagPath))
		{
			File.Delete(flagPath);
		}
	}
}

string Exchange => "temporal_tests_rpc";

List<string> Tokens = new List<string>()
{
	"143691", "848870", "442933", "408407", "377107",
	"233699", "365420", "731548", "722126", "759754"
};

void GenerateToken(IModel channel)
{
	string routingKey = "Processa.RabbitMQ.Services.Bifrost.Contracts.TokenRequest:Processa.RabbitMQ.Services.Bifrost".Dump("GenerateTokenRoutingKey");
	this.GetResponse(channel, routingKey, (GenerateTokenRequestInfo request, GenerateTokenResponseInfo response) =>
	{
		string userIdentity = $"{request.DocType}-{request.DocNumber}";
		Console.WriteLine($"[i] ({DateTime.Now.ToString("HH:mm:ss.fff")}) Generate Token Request Received for user: '{userIdentity}'");
		request.CorrelationalId = request.CorrelationalId ?? Guid.NewGuid().ToString();
		response.CorrelationalId = request.CorrelationalId;

		request.AccountType = request.AccountType ?? string.Empty;
		Regex accountTypePattern = new Regex("^(80|81|82|83|84)$", RegexOptions.Singleline);
		if (!accountTypePattern.IsMatch(request.AccountType))
		{
			response.ResponseCode = (int)HttpStatusCode.BadRequest;
			response.ResponseMessage = "El tipo de cuenta es inválido.";
			Console.WriteLine($"[i] ({DateTime.Now.ToString("HH:mm:ss.fff")}) Generate token request failed.");
			return;
		}

		request.DocType = request.DocType ?? string.Empty;
		Regex docTypePattern = new Regex("^(CC|NJ|TI|CE|PS)$", RegexOptions.Singleline | RegexOptions.IgnoreCase);
		if (!docTypePattern.IsMatch(request.DocType))
		{
			response.ResponseCode = (int)HttpStatusCode.BadRequest;
			response.ResponseMessage = "El tipo de documento es inválido.";
			Console.WriteLine($"[i] ({DateTime.Now.ToString("HH:mm:ss.fff")}) Generate token request failed.");
			return;
		}

		request.DocNumber = request.DocNumber ?? string.Empty;
		Regex docNumberPattern = new Regex(@"^\d{1,18}$", RegexOptions.Singleline);
		if (!docNumberPattern.IsMatch(request.DocNumber))
		{
			response.ResponseCode = (int)HttpStatusCode.BadRequest;
			response.ResponseMessage = "El número de documento es inválido.";
			Console.WriteLine($"[i] ({DateTime.Now.ToString("HH:mm:ss.fff")}) Generate token request failed.");
			return;
		}

		if (string.IsNullOrWhiteSpace(request.Channel))
		{
			response.ResponseCode = (int)HttpStatusCode.BadRequest;
			response.ResponseMessage = "El canal es inválido.";
			Console.WriteLine($"[i] ({DateTime.Now.ToString("HH:mm:ss.fff")}) Generate token request failed.");
			return;
		}

		if (request.Channel == "DEMOINTSERERR0000000")
		{
			response.ResponseCode = (int)HttpStatusCode.InternalServerError;
			response.ResponseMessage = "Una respuesta con código de error interno. Este es un comportamiento controlado para imitar el escenario donde no se puede generar el token por algún error interno.";
			Console.WriteLine($"[i] ({DateTime.Now.ToString("HH:mm:ss.fff")}) Generate token request failed.");
			return;
		}

		if (request.Channel == "DEMOINVRESCOD0000000")
		{
			response.ResponseCode = 99999;
			response.ResponseMessage = "Una respuesta con código HTTP desconocido. Este es un comportamiento controlado para imitar el escenario donde se establece un código de respuesta que no coincide con un HttpStatusCode.";
			Console.WriteLine($"[i] ({DateTime.Now.ToString("HH:mm:ss.fff")}) Generate token request failed.");
			return;
		}

		if (request.Channel == "DEMOCUSTOMCOD0000000")
		{
			response.ResponseCode = (int)HttpStatusCode.Conflict;
			response.ResponseMessage = "Una respuesta con código HTTP válido. Este es un comportamiento controlado para imitar el escenario donde se establece un código de respuesta que coincide con un HttpStatusCode.";
			Console.WriteLine($"[i] ({DateTime.Now.ToString("HH:mm:ss.fff")}) Generate token request failed.");
			return;
		}

		int randomIndex = new Random().Next(0, this.Tokens.Count - 1);
		response.Token = this.Tokens[randomIndex];
		Console.WriteLine($"[i] ({DateTime.Now.ToString("HH:mm:ss.fff")}) Token Generated: '{response.Token}' for user: '{userIdentity}'");
	});
}

void RedeemToken(IModel channel)
{
	string routingKey = "Processa.RabbitMQ.Services.Bifrost.Contracts.TokenValidateRequest:Processa.RabbitMQ.Services.Bifrost".Dump("RedeemTokenRoutingKey");
	this.GetResponse(channel, routingKey, (RedeemTokenRequestInfo request, RedeemTokenResponseInfo response) =>
	{
		string token = string.IsNullOrWhiteSpace(request.Token) ? "NonSet" : request.Token;
		string userIdentity = $"{request.DocType}-{request.DocNumber}";
		Console.WriteLine($"[i] ({DateTime.Now.ToString("HH:mm:ss.fff")}) Token Redeem Request Received. Token: '{token}' issued to user: '{userIdentity}'");
		request.CorrelationalId = request.CorrelationalId ?? Guid.NewGuid().ToString();
		response.CorrelationalId = request.CorrelationalId;

		request.DocType = request.DocType ?? string.Empty;
		Regex docTypePattern = new Regex("^(CC|NJ|TI|CE|PS)$", RegexOptions.Singleline | RegexOptions.IgnoreCase);
		if (!docTypePattern.IsMatch(request.DocType))
		{
			response.ResponseCode = (int)HttpStatusCode.BadRequest;
			response.ResponseMessage = "El tipo de documento es inválido.";
			Console.WriteLine($"[i] ({DateTime.Now.ToString("HH:mm:ss.fff")}) Redeem token request failed.");
			return;
		}
		
		request.DocNumber = request.DocNumber ?? string.Empty;
		Regex docNumberPattern = new Regex(@"^\d{1,18}$", RegexOptions.Singleline);
		if (!docNumberPattern.IsMatch(request.DocNumber))
		{
			response.ResponseCode = (int)HttpStatusCode.BadRequest;
			response.ResponseMessage = "El número de documento es inválido.";
			Console.WriteLine($"[i] ({DateTime.Now.ToString("HH:mm:ss.fff")}) Redeem token request failed.");
			return;
		}

		if (request.Token == "000000")
		{
			response.ResponseCode = (int)HttpStatusCode.NotFound;
			response.ResponseMessage = "No se encontró token con los valores proporcionados.";
			Console.WriteLine($"[i] ({DateTime.Now.ToString("HH:mm:ss.fff")}) Redeem token request failed.");
			return;
		}

		if (request.Token == "000001")
		{
			response.ResponseCode = (int)HttpStatusCode.InternalServerError;
			response.ResponseMessage = "Una respuesta con código de error interno. Este es un comportamiento controlado para imitar el escenario donde no se puede generar el token por algún error interno.";
			Console.WriteLine($"[i] ({DateTime.Now.ToString("HH:mm:ss.fff")}) Redeem token request failed.");
			return;
		}

		if (request.Token == "000002")
		{
			response.ResponseCode = 99999;
			response.ResponseMessage = "Una respuesta con código HTTP desconocido. Este es un comportamiento controlado para imitar el escenario donde se establece un código de respuesta que no coincide con un HttpStatusCode.";
			Console.WriteLine($"[i] ({DateTime.Now.ToString("HH:mm:ss.fff")}) Redeem token request failed.");
			return;
		}

		if (request.Token == "000003")
		{
			response.ResponseCode = (int)HttpStatusCode.Conflict;
			response.ResponseMessage = "Una respuesta con código HTTP válido. Este es un comportamiento controlado para imitar el escenario donde se establece un código de respuesta que coincide con un HttpStatusCode.";
			Console.WriteLine($"[i] ({DateTime.Now.ToString("HH:mm:ss.fff")}) Redeem token request failed.");
			return;
		}

		bool redeemed = this.Tokens.FirstOrDefault(t => t == request.Token) != null;
		if (!redeemed)
		{
			response.ResponseCode = (int)HttpStatusCode.Unauthorized;
			response.ResponseMessage = "Token inválido.";
			Console.WriteLine($"[i] ({DateTime.Now.ToString("HH:mm:ss.fff")}) Token: '{token}' issued to user: '{userIdentity}' not found.");
			return;
		}

		Console.WriteLine($"[i] ({DateTime.Now.ToString("HH:mm:ss.fff")}) Token: '{token}' issued to user: '{userIdentity}' redeemed.");
	});
}

void NullifyToken(IModel channel)
{
	string routingKey = "Processa.RabbitMQ.Services.Demo.TokenProvider.Contracts.NullifyToken".Dump("NullifyTokenRoutingKey");
	this.GetResponse(channel, routingKey, (NullifyTokenRequestInfo request, NullifyTokenResponseInfo response) =>
	{
		string token = string.IsNullOrWhiteSpace(request.Token) ? "NonSet" : request.Token;
		string userIdentity = $"{request.DocType}-{request.DocNumber}";
		Console.WriteLine($"[i] ({DateTime.Now.ToString("HH:mm:ss.fff")}) Nullify Token Request Received. Token: '{token}' from user: '{userIdentity}'");
		request.CorrelationalId = request.CorrelationalId ?? Guid.NewGuid().ToString();
		response.CorrelationalId = request.CorrelationalId;
		Console.WriteLine($"[i] ({DateTime.Now.ToString("HH:mm:ss.fff")}) Token: '{token}' from user: '{userIdentity}' nullified.");
	});
}

void GetResponse<TRequest, TResponse>(IModel channel, string routingKey, Action<TRequest, TResponse> onResponse)
	where TRequest : IRequest, new()
	where TResponse : IResponse, new()
{
	channel.QueueDeclare(queue: routingKey, durable: false, exclusive: false, autoDelete: true, arguments: null);
	channel.QueueBind(queue: routingKey, exchange: Exchange, routingKey: routingKey, arguments: null);
	EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
	channel.BasicConsume(queue: routingKey, autoAck: false, consumer: consumer);
	consumer.Received += (model, ea) =>
	{
		var body = ea.Body.ToArray();
		var props = ea.BasicProperties;
		var replyProps = channel.CreateBasicProperties();
		replyProps.CorrelationId = props.CorrelationId;
		TResponse response = new TResponse();

		try
		{
			string message = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(body))?.ToString();
			TRequest request = JsonConvert.DeserializeObject<TRequest>(message);
			response.CorrelationalId = string.IsNullOrWhiteSpace(request.CorrelationalId) ? (new Guid()).ToString() : request.CorrelationalId;
			onResponse(request, response);
		}
		catch (Exception ex)
		{
			response.ResponseCode = (int)HttpStatusCode.InternalServerError;
			response.ResponseMessage = ex.Message;
		}
		finally
		{
			var responseBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(response));
			channel.BasicPublish(exchange: string.Empty, routingKey: props.ReplyTo, basicProperties: replyProps, body: responseBytes);
			channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
		}
	};
}

interface IRequest
{
	string CorrelationalId { get; set; }
}

interface IResponse
{
	string CorrelationalId { get; set; }
	int ResponseCode { get; set; }
	string ResponseMessage { get; set; }
}

class GenerateTokenRequestInfo : IRequest
{
	[JsonProperty("FromAcctType")]
	public string AccountType { get; set; }
	public string Channel { get; set; }
	[JsonProperty("DocumentNum")]
	public string DocNumber { get; set; }
	[JsonProperty("DocumentType")]
	public string DocType { get; set; }
	public string CorrelationalId { get; set; }
}

class GenerateTokenResponseInfo : IResponse
{
	public GenerateTokenResponseInfo()
	{
		const HttpStatusCode statusCode = HttpStatusCode.OK;
		this.ResponseCode = (int)statusCode;
		this.ResponseMessage = statusCode.ToString();
	}

	public string CorrelationalId { get; set; }
	public int ResponseCode { get; set; }
	public string ResponseMessage { get; set; }
	public string Token { get; set; }
}

class RedeemTokenRequestInfo : IRequest
{
	public string CorrelationalId { get; set; }
	[JsonProperty("Metadata")]
	public string Token { get; set; }
	public string DocType { get; set; }
	public string DocNumber { get; set; }
}

class RedeemTokenResponseInfo : IResponse
{
	public RedeemTokenResponseInfo()
	{
		const HttpStatusCode statusCode = HttpStatusCode.OK;
		this.ResponseCode = (int)statusCode;
		this.ResponseMessage = statusCode.ToString();
	}

	public string CorrelationalId { get; set; }
	public int ResponseCode { get; set; }
	public string ResponseMessage { get; set; }
}

interface INullifyTokenRequest : IRequest
{
	int AppId { get; set; }
	string DocNumber { get; set; }
	string DocType { get; set; }
	string Metadata { get; set; }
	string Token { get; set; }
}

class NullifyTokenRequestInfo : INullifyTokenRequest
{
	public int AppId { get; set; }
	public string DocNumber { get; set; }
	public string DocType { get; set; }
	public string Metadata { get; set; }
	public string Token { get; set; }
	public string CorrelationalId { get; set; }
	public string ChannelKey { get; set; }
}

class NullifyTokenResponseInfo : IResponse
{
	public NullifyTokenResponseInfo()
	{
		const HttpStatusCode statusCode = HttpStatusCode.OK;
		this.ResponseCode = (int)statusCode;
		this.ResponseMessage = statusCode.ToString();
	}

	public string CorrelationalId { get; set; }
	public int ResponseCode { get; set; }
	public string ResponseMessage { get; set; }
}