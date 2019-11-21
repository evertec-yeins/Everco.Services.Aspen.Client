<Query Kind="Program">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <NuGetReference>RabbitMQ.Client</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
  <Namespace>RabbitMQ.Client</Namespace>
  <Namespace>RabbitMQ.Client.Events</Namespace>
  <Namespace>System.Net</Namespace>
  <Namespace>System</Namespace>
</Query>

void Main()
{
	string flagPath = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), $"{Util.CurrentQuery.Name}.flag");
	StringBuilder flagBluider = new StringBuilder();
	string debugMessage = $"[x] Starting RpcServer [{Util.CurrentQuery.Name}]...";
	flagBluider.AppendLine(debugMessage);
	Console.WriteLine(debugMessage);

	try
	{
		ConnectionFactory factory = new ConnectionFactory()
		{
			HostName = "localhost",
			UserName = "guest",
			Password = "guest",
			VirtualHost = "/"
		};

		debugMessage = $"[x] Using connection: 'host={factory.HostName};virtualhost={factory.VirtualHost};username={factory.UserName};password={factory.Password};exchange={this.Exchange}'";
		Console.WriteLine(debugMessage);
		flagBluider.AppendLine(debugMessage);
		using (IConnection connection = factory.CreateConnection("Bifrost Dummy RPC Server"))
		{
			using (IModel channel = connection.CreateModel())
			{
				// El exchange ya existe, no se necesita declararlo.
				//channel.ExchangeDeclare(exchange: Exchange, type: ExchangeType.Direct, durable: false, autoDelete: true, arguments: null);
				this.GetCustomerProducts(channel);
				this.GetProductMovements(channel);

				debugMessage = "[x] Awaiting requests...";
				Console.WriteLine(debugMessage);
				flagBluider.AppendLine(debugMessage);

				File.WriteAllText(flagPath, flagBluider.ToString());
				Console.WriteLine("Press [enter] to exit.");
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

string Exchange => "easy_net_q_rpc";

void GetCustomerProducts(IModel channel)
{
	string routingKey = "Processa.RabbitMQ.Services.Mobility.Contracts.Commands.CustomerProductsRequest:Processa.RabbitMQ.Services.Mobility.Contracts";
	this.GetResponse(channel, routingKey, (CustomerProductsRequest request, CustomerProductsResponse response) =>
	{
		Console.WriteLine($"[.] ({DateTime.Now.ToString("HH:mm:ss.fff")}) Customer Products Request Received");
		request.CorrelationalId = request.CorrelationalId ?? Guid.NewGuid().ToString("N").Substring(0, 29);
		response.CorrelationalId = request.CorrelationalId;
		int accountNumber = new Random().Next(100000000, 999999999);
		decimal balance = new Random().Next(1000, 9999999);
		decimal partialPayment = balance * 5 / 100;
		response.Products = new List<ProductInfo>()
		{
			new ProductInfo()
			{
				AccountNumber = accountNumber.ToString("0000000000000000"),
				AllowsPayment = true,
				Balance = balance,
				Id = accountNumber.ToString(),
				IsLockedOut = false,
				Name = "Linea Cupo Rotativo",
				Properties = new List<ProductProperty>()
				{
					new ProductProperty()
					{
						Key = "LastTran",
						Label = "Última transacción",
						Value = DateTime.Now.ToString("G"),
						Raw = DateTime.Now
					},
					new ProductProperty()
					{
						Key = "NextPayment",
						Label = "Próximo pago",
						Value = DateTime.Now.ToString("G"),
						Raw = DateTime.Now
					},
					new ProductProperty()
					{
						Key = "FullPayment",
						Label = "Pago total",
						Value = balance.ToString("C0"),
						Raw = balance
					},
					new ProductProperty()
					{
						Key = "PartialPayment",
						Label = "Valor cuota",
						Value = partialPayment.ToString("C0"),
						Raw = partialPayment
					}
				}
			}
		};
	});
}

void GetProductMovements(IModel channel)
{
	string routingKey = "Processa.RabbitMQ.Services.Mobility.Contracts.Commands.AccountMovementsRequest:Processa.RabbitMQ.Services.Mobility.Contracts";
	this.GetResponse(channel, routingKey, (ProductMovementsRequest request, ProductMovementsResponse response) =>
	{
		Console.WriteLine($"[.] ({DateTime.Now.ToString("HH:mm:ss.fff")}) Product Movements Request Received");
		request.CorrelationalId = request.CorrelationalId ?? Guid.NewGuid().ToString("N").Substring(0, 29);
		response.CorrelationalId = request.CorrelationalId;
		response.Movements = new List<ProductMovementInfo>() 
		{
			new ProductMovementInfo()
			{
				CardAcceptor = "Almacenes Acme",
				Category = Accounting.Debit,
				Date = DateTime.Now,
				TranName = "Retiro ATM",
				Value = new Random().Next(10000, 999999)
			},
			new ProductMovementInfo()
			{
				CardAcceptor = "Almacenes Acme",
				Category = Accounting.Credit,
				Date = DateTime.Now,
				TranName = "Desembolso",
				Value = new Random().Next(10000, 999999)
			},
			new ProductMovementInfo()
			{
				CardAcceptor = "Almacenes Acme",
				Category = Accounting.Debit,
				Date = DateTime.Now,
				TranName = "Retiro ATM",
				Value = new Random().Next(-999999, -10000)
			},
			new ProductMovementInfo()
			{
				CardAcceptor = "Almacenes Acme",
				Category = Accounting.Debit,
				Date = DateTime.Now,
				TranName = "Retiro ATM",
				Value = new Random().Next(10000, 999999)
			},
			new ProductMovementInfo()
			{
				CardAcceptor = "Almacenes Acme",
				Category = Accounting.Debit,
				Date = DateTime.Now,
				TranName = "Retiro ATM",
				Value = new Random().Next(10000, 999999)
			},
		};
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
		var body = ea.Body;
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
	bool Successful { get; }
}

class CustomerProductsRequest : IRequest
{
	public string CorrelationalId { get; set; } = Guid.NewGuid().ToString("N").Substring(0, 29); 
	public string DocType { get; set; } = "1";
	public string DocNumber { get; set; }
}

class CustomerProductsResponse : IResponse
{
	public string CorrelationalId { get; set; }
	public int ResponseCode { get; set; } = (int)HttpStatusCode.OK;
	public string ResponseMessage { get; set; } = HttpStatusCode.OK.ToString();
	public bool Successful => this.ResponseCode >= 200 | this.ResponseCode <= 299;
	public IList<ProductInfo> Products { get; set; }
}

class ProductMovementsRequest : IRequest
{
	public string CorrelationalId { get; set; } = Guid.NewGuid().ToString("N").Substring(0, 29);
	public string AccountId { get; set; }
}

class ProductMovementsResponse : IResponse
{
	public string CorrelationalId { get; set; }
	public int ResponseCode { get; set; } = (int)HttpStatusCode.OK;
	public string ResponseMessage { get; set; } = HttpStatusCode.OK.ToString();
	public bool Successful => this.ResponseCode >= 200 | this.ResponseCode <= 299;
	public IList<ProductMovementInfo> Movements { get; set; }
}

class ProductInfo
{
	public string Id { get; set; }
	public string Name { get; set; }
	public string AccountNumber { get; set; }
	public bool IsLockedOut { get; set; }
	public bool AllowsPayment { get; set; }
	public decimal Balance { get; set; }
	public IList<ProductProperty> Properties { get; set; }
}

class ProductProperty
{
	public string Label { get; set; }
	public string Key { get; set; }
	public string Value { get; set; }
	public object Raw { get; set; }
}

class ProductMovementInfo
{
	public string CardAcceptor { get; set; }
	public DateTime Date { get; set; }
	public decimal Value { get; set; }
	public string TranName { get; set; }
	public Accounting Category { get; set; }
}

enum Accounting
{
	Debit = 0,
	Credit = 1
}