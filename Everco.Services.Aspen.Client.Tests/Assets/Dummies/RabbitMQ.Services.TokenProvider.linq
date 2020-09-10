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
	string routingKey = "Processa.RabbitMQ.Services.Demo.TokenProvider.Contracts.GenerateToken".Dump("GenerateTokenRoutingKey");
	this.GetResponse(channel, routingKey, (GenerateTokenRequestInfo request, GenerateTokenResponseInfo response) =>
	{
		Console.WriteLine($"[i] ({DateTime.Now.ToString("HH:mm:ss.fff")}) Generate Token Request Received for user [{request.DocType}-{request.DocNumber}]");
		request.CorrelationalId = request.CorrelationalId ?? Guid.NewGuid().ToString();
		response.CorrelationalId = request.CorrelationalId;
		int randomIndex = new Random().Next(0, this.Tokens.Count - 1);
		response.Token = this.Tokens[randomIndex];
		Console.WriteLine($"[i] ({DateTime.Now.ToString("HH:mm:ss.fff")}) Token Generated [{response.Token}] for user [{request.DocType}-{request.DocNumber}]");
	});
}

void RedeemToken(IModel channel)
{
	string routingKey = "Processa.RabbitMQ.Services.Demo.TokenProvider.Contracts.RedeemToken".Dump("RedeemTokenRoutingKey");
	this.GetResponse(channel, routingKey, (RedeemTokenRequestInfo request, RedeemTokenResponseInfo response) =>
	{
		Console.WriteLine($"[i] ({DateTime.Now.ToString("HH:mm:ss.fff")}) Token Redeem Request [{request.Token}] for user [{request.DocType}-{request.DocNumber}] Received...");
		request.CorrelationalId = request.CorrelationalId ?? Guid.NewGuid().ToString();
		response.CorrelationalId = request.CorrelationalId;
		
		bool redeemed = this.Tokens.FirstOrDefault(t => t == request.Token) != null;
		if (!redeemed)
		{
			response.ResponseCode = (int)HttpStatusCode.NotFound;
			response.ResponseMessage = "No se encontró un token con los valores proporcionados.";
			Console.WriteLine($"[i] ({DateTime.Now.ToString("HH:mm:ss.fff")}) Token: [{request.Token}] for user [{request.DocType}-{request.DocNumber}] Not Found.");
			return;
		}

		Console.WriteLine($"[i] ({DateTime.Now.ToString("HH:mm:ss.fff")}) Token: [{request.Token}] for user [{request.DocType}-{request.DocNumber}] Redeemed.");
	});
}

void NullifyToken(IModel channel)
{
	string routingKey = "Processa.RabbitMQ.Services.Demo.TokenProvider.Contracts.NullifyToken".Dump("NullifyTokenRoutingKey");
	this.GetResponse(channel, routingKey, (NullifyTokenRequestInfo request, NullifyTokenResponseInfo response) =>
	{
		Console.WriteLine($"[i] ({DateTime.Now.ToString("HH:mm:ss.fff")}) Nullify Token Request Received for user [{request.DocType}-{request.DocNumber}] and token: [{request.Token}]");
		request.CorrelationalId = request.CorrelationalId ?? Guid.NewGuid().ToString();
		response.CorrelationalId = request.CorrelationalId;
		Console.WriteLine($"[i] ({DateTime.Now.ToString("HH:mm:ss.fff")}) Token: [{request.Token}] from user [{request.DocType}-{request.DocNumber}] nullified.");
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
	string ChannelKey { get; set; }
}

interface IResponse
{
	string CorrelationalId { get; set; }
	int ResponseCode { get; set; }
	string ResponseMessage { get; set; }
}

interface IGenerateTokenRequest : IRequest
{
	string AccountType { get; set; }
	int? Amount { get; set; }
	string DocNumber { get; set; }
	string DocType { get; set; }
	string Metadata { get; set; }
	int RequestedFromAppId { get; set; }
	string RequestFrom { get; set; }
}

class GenerateTokenRequestInfo : IGenerateTokenRequest
{
	public string AccountType { get; set; }
	public int? Amount { get; set; }
	public string DocNumber { get; set; }
	public string DocType { get; set; }
	public string Metadata { get; set; }
	public string PinNumber { get; set; }
	public int RequestedFromAppId { get; set; }
	public string RequestFrom { get; set; }
	public string CorrelationalId { get; set; }
	public string ChannelKey { get; set; }
}

interface IGenerateTokenResponse : IResponse
{
	string Token { get; set; }
	DateTimeOffset ExpiresAt { get; set; }
	string ChannelKey { get; set; }
	string ChannelName { get; set; }
	int ExpirationMinutes { get; set; }
}

class GenerateTokenResponseInfo : IGenerateTokenResponse
{
	public GenerateTokenResponseInfo()
	{
		const HttpStatusCode statusCode = HttpStatusCode.OK;
		this.ResponseCode = (int)statusCode;
		this.ResponseMessage = statusCode.ToString();
		int lifetimeMinutes = 30;
		this.ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(lifetimeMinutes);
		this.ExpirationMinutes = lifetimeMinutes;
	}
	
	public string CorrelationalId { get; set; }
	public int ResponseCode { get; set; }
	public string ResponseMessage { get; set; }
	public string Token { get; set; }
	public DateTimeOffset ExpiresAt { get; set; }
	public string ChannelKey { get; set; }
	public string ChannelName { get; set; }
	public int ExpirationMinutes { get; set; }
}

interface IRedeemTokenRequest : IRequest
{
	string AccountType { get; set; }
	int? Amount { get; set; }
	string DocNumber { get; set; }
	string DocType { get; set; }
	string Metadata { get; set; }
	int RedeemedFromAppId { get; set; }
	string Token { get; set; }
}

class RedeemTokenRequestInfo : IRedeemTokenRequest
{
	public string CorrelationalId { get; set; }
	public string Metadata { get; set; }
	public string AccountType { get; set; }
	public int? Amount { get; set; }
	public string Token { get; set; }
	public string DocType { get; set; }
	public string DocNumber { get; set; }
	public string ChannelKey { get; set; }
	public int RedeemedFromAppId { get; set; }
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