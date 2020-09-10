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
			VirtualHost = "Kraken"
		};

		debugMessage = $"[x] Using connection: 'host={factory.HostName};virtualhost={factory.VirtualHost};username={factory.UserName};password={factory.Password};exchange={this.Exchange}'";
		Console.WriteLine(debugMessage);
		flagBluider.AppendLine(debugMessage);
		using (IConnection connection = factory.CreateConnection("Kraken Dummy RPC Server"))
		{
			using (IModel channel = connection.CreateModel())
			{
				channel.ExchangeDeclare(exchange: this.Exchange, type: ExchangeType.Direct, durable: false, autoDelete: true, arguments: null);
				this.SendNotification(channel);

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

void SendNotification(IModel channel)
{
	string routingKey = "Processa.RabbitMQ.Services.Kraken.Domain.Contracts.NotificationRequest:Processa.RabbitMQ.Services.Kraken.Domain";
	this.GetResponse(channel, routingKey, (SMSRequest request, SMSResponse response) =>
	{
		Console.WriteLine($"[x] ({DateTime.Now.ToString("HH:mm:ss.fff")}) SMS Request [{request.Message}] Received...");
		request.CorrelationalId = request.CorrelationalId ?? Guid.NewGuid().ToString();
		response.CorrelationalId = request.CorrelationalId;
		
		if (request.Channel == "DEMOEXPFAILED00000000")
		{
			response.ResponseCode = (int)HttpStatusCode.ExpectationFailed;
			response.ResponseMessage = "Comportamiento controlado para imitar el escenario donde no se puede enviar la notificación.";
			Console.WriteLine($"[i] ({DateTime.Now.ToString("HH:mm:ss.fff")}) SMS Request [{request.Message}] Failed.");
			return;
		}

		if (request.Channel == "DEMONOTFOUNDD00000001")
		{
			response.ResponseCode = (int)HttpStatusCode.NotFound;
			response.ResponseMessage = $"El usuario con documento: '{request.DocType}-{request.DocNumber}' no existe en el sistema.";
			Console.WriteLine($"[i] ({DateTime.Now.ToString("HH:mm:ss.fff")}) SMS Request [{request.Message}] Failed.");
			return;
		}

		Console.WriteLine($"[i] ({DateTime.Now.ToString("HH:mm:ss.fff")}) SMS Request [{request.Message}] Ended. Message was sent.");
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
	string Channel { get; set; }
}

interface IResponse
{
	string CorrelationalId { get; set; }
	int ResponseCode { get; set; }
	string ResponseMessage { get; set; }
}

class SMSRequest : IRequest
{
	public string CorrelationalId { get; set; }
	public string Channel { get; set; }
	public string DocType { get; set; }
	public string DocNumber { get; set; }
	public string Message { get; set; }
}

class SMSResponse : IResponse
{
	public SMSResponse()
	{
		const HttpStatusCode statusCode = HttpStatusCode.OK;
		this.CustomData = new Dictionary<string, string>();
		this.ResponseCode = (int)statusCode;
		this.ResponseMessage = statusCode.ToString();
	}
	
	public string CorrelationalId { get; set; }
	public int ResponseCode { get; set; }
	public string ResponseMessage { get; set; }
	public Dictionary<string, string> CustomData { get; set; }
}