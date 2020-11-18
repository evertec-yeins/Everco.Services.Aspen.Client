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
			UserName = "test",
			Password = "test",
			VirtualHost = "Bancor"
		};

		debugMessage = $"[x] Using connection: 'host={factory.HostName};virtualhost={factory.VirtualHost};username={factory.UserName};password={factory.Password};exchange={this.Exchange}'";
		Console.WriteLine(debugMessage);
		flagBluider.AppendLine(debugMessage);
		using (IConnection connection = factory.CreateConnection("Bancor Dummy RPC Server"))
		{
			using (IModel channel = connection.CreateModel())
			{
				channel.ExchangeDeclare(exchange: Exchange, type: ExchangeType.Direct, durable: false, autoDelete: true, arguments: null);
				this.GetCustomerProducts(channel);
				this.GetProductMovements(channel);

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

Dictionary<string, List<ProductInfo>> Products => new Dictionary<string, List<ProductInfo>>()
{
	{
		"1-52080323",
		new List<ProductInfo>()
		{
			new ProductInfo("000000052080323")
		}
	},
	{
		"1-3262308",
		new List<ProductInfo>()
		{
			new ProductInfo("000000003262308")
		}
	},
	{
		"1-52582664",
		new List<ProductInfo>()
		{
			new ProductInfo("000000052582664")
		}
	},
	{
		"1-1016039207",
		new List<ProductInfo>()
		{
			new ProductInfo("000001016039207")
		}
	}
};

Dictionary<string, List<ProductMovementInfo>> movements = null;
Dictionary<string, List<ProductMovementInfo>> Movements
{
	get
	{
		if (movements != null)
		{
			return movements;
		}

		List<ProductMovementInfo> GetRandomMovements()
		{
			List<ProductMovementInfo> movements = Enumerable.Empty<ProductMovementInfo>().ToList();
			for (int index = 1; index <= 5; index++)
			{
				movements.Add(new ProductMovementInfo());
			}

			return movements.OrderBy(x => x.Date).ToList();
		}

		movements = new Dictionary<string, List<ProductMovementInfo>>()
		{
			{ "000000052080323", GetRandomMovements() },
			{ "000000003262308", GetRandomMovements() },
			{ "000000052582664", GetRandomMovements() },
			{ "000001016039207", GetRandomMovements() }
		};

		return movements;
	}
}

void GetCustomerProducts(IModel channel)
{
	string routingKey = "Processa.RabbitMQ.Services.Mobility.Contracts.Commands.CustomerProductsRequest:Processa.RabbitMQ.Services.Mobility.Contracts";
	this.GetResponse(channel, routingKey, (CustomerProductsRequest request, CustomerProductsResponse response) =>
	{
		Console.WriteLine($"[x] ({DateTime.Now.ToString("HH:mm:ss.fff")}) Customer Products Request Received...");
		request.CorrelationalId = request.CorrelationalId ?? Guid.NewGuid().ToString("N").Substring(0, 29);
		response.CorrelationalId = request.CorrelationalId;

		string key = $"{request.DocType}-{request.DocNumber}";
		if (Products.TryGetValue(key, out List<ProductInfo> products))
		{
			response.Products = products;
		}
	});
}

void GetProductMovements(IModel channel)
{
	string routingKey = "Processa.RabbitMQ.Services.Mobility.Contracts.Commands.AccountMovementsRequest:Processa.RabbitMQ.Services.Mobility.Contracts";
	this.GetResponse(channel, routingKey, (ProductMovementsRequest request, ProductMovementsResponse response) =>
	{
		Console.WriteLine($"[x] ({DateTime.Now.ToString("HH:mm:ss.fff")}) Product Movements Request Received...");
		request.CorrelationalId = request.CorrelationalId ?? Guid.NewGuid().ToString("N").Substring(0, 29);
		response.CorrelationalId = request.CorrelationalId;

		if (Movements.TryGetValue(request.AccountId, out List<ProductMovementInfo> movements))
		{
			response.Movements = movements;
		}
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
	bool Successful { get; }
}

class CustomerProductsRequest : IRequest
{
	public CustomerProductsRequest()
	{
		this.CorrelationalId = Guid.NewGuid().ToString("N").Substring(0, 29);
		this.DocType = "1";
	}
	
	public string CorrelationalId { get; set; }
	public string DocType { get; set; }
	public string DocNumber { get; set; }
}

class CustomerProductsResponse : IResponse
{
	public CustomerProductsResponse()
	{
		this.Products = Enumerable.Empty<ProductInfo>().ToList();
	}
	
	public string CorrelationalId { get; set; }
	public int ResponseCode { get; set; } = (int)HttpStatusCode.OK;
	public string ResponseMessage { get; set; } = HttpStatusCode.OK.ToString();
	public bool Successful => this.ResponseCode >= 200 | this.ResponseCode <= 299;
	public IList<ProductInfo> Products { get; set; }
}

class ProductMovementsRequest : IRequest
{
	public ProductMovementsRequest()
	{
		this.CorrelationalId = Guid.NewGuid().ToString("N").Substring(0, 29);	
	}
	
	public string CorrelationalId { get; set; }
	public string AccountId { get; set; }
}

class ProductMovementsResponse : IResponse
{
	public ProductMovementsResponse()
	{
		this.Movements = Enumerable.Empty<ProductMovementInfo>().ToList();
	}
	
	public string CorrelationalId { get; set; }
	public int ResponseCode { get; set; } = (int)HttpStatusCode.OK;
	public string ResponseMessage { get; set; } = HttpStatusCode.OK.ToString();
	public bool Successful => this.ResponseCode >= 200 | this.ResponseCode <= 299;
	public IList<ProductMovementInfo> Movements { get; set; }
}

class ProductInfo
{
	public ProductInfo()
	{
		string accountNumber = new Random().Next(100000000, 999999999).ToString("0000000000000000");
		decimal balance = 1000000;
		decimal partialPayment = balance * 5 / 100;
		
		this.AccountNumber = accountNumber;
		this.AllowsPayment = true;
		this.Balance = balance;
		this.Id = accountNumber;
		this.IsLockedOut = false;
		this.Name = "Linea Cupo Rotativo";
		this.Properties = new List<ProductProperty>()
		{
			new ProductProperty()
			{
				Key = "LastTran",
				Label = "Última transacción",
				Value = DateTime.Now.ToString("G"),
			},
			new ProductProperty()
			{
				Key = "NextPayment",
				Label = "Próximo pago",
				Value = DateTime.Now.ToString("G"),
			},
			new ProductProperty()
			{
				Key = "FullPayment",
				Label = "Pago total",
				Value = balance.ToString("C0"),
			},
			new ProductProperty()
			{
				Key = "PartialPayment",
				Label = "Valor cuota",
				Value = partialPayment.ToString("C0"),
			}
		};
	}
	
	public ProductInfo(string accountNumber) : this()
	{
		this.AccountNumber = accountNumber;
		this.Id = accountNumber;
	}
	
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
}

class ProductMovementInfo
{
	public ProductMovementInfo()
	{
		Random random = new Random();
		this.CardAcceptor = "Acme Corporation";
		this.Category = Accounting.Debit;
		this.Date = DateTime.Now
			.AddDays(-(random.Next(2, 10)))
			.AddHours(random.Next(0, 24))
			.AddMinutes(random.Next(0, 60))
			.AddSeconds(random.Next(0, 60));
		this.TranName = "Retiro ATM";
		this.Value = 100000;
	}
	
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