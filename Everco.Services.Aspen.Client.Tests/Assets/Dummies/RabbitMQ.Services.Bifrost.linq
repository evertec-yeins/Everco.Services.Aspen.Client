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
				this.GetCardHolder(channel);
				this.GetBalances(channel);
				this.GetMiniStatements(channel);

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

void GetCardHolder(IModel channel)
{
	string routingKey = "Processa.RabbitMQ.Services.Bifrost.Contracts.CardHolderRequest:Processa.RabbitMQ.Services.Bifrost";
	this.GetResponse(channel, routingKey, (CardHolderRequest request, CardHolderResponse response) =>
	{
		Console.WriteLine($"[.] ({DateTime.Now.ToString("HH:mm:ss.fff")}) Card Holder Request Received...");
		request.CorrelationalId = request.CorrelationalId ?? Guid.NewGuid().ToString();
		response.CorrelationalId = request.CorrelationalId;
		
		response.CardHolders = new List<CardHolder>()
		{
			new CardHolder()
			{
				Address = "Dummy Address",
				CardLabel = "Dummy Card Label",
				Creation = DateTime.Now,
				PersonId = LongRandom(10000000, 10000000000000),
				CityCode = "00",
				CityName = "Dummy City Name",
				Code = "00",
				CustomerGroup = "Dummy Customer Group",
				DocNumber = request.DocNumber,
				DocType = request.DocType,
				Email = "dummy@dummy.com.co",
				FirstName = "Pedro",
				LastName = "Pablo",
				RegionName = "Dummy Region Name",
				Telephone = (new Random()).Next(999999, 999999999).ToString()
			}
		};

		if (request.DocNumber.StartsWith("0"))
		{
			response.CardHolders = null;
			return;
		}
	});
}

void GetBalances(IModel channel)
{
	string routingKey = "Processa.RabbitMQ.Services.Bifrost.Contracts.BalanceRequest:Processa.RabbitMQ.Services.Bifrost";
	this.GetResponse(channel, routingKey, (BalanceRequest request, BalanceResponse response) =>
	{
		Console.WriteLine($"[.] ({DateTime.Now.ToString("HH:mm:ss.fff")}) Card Holder Request Received...");
		request.CorrelationalId = request.CorrelationalId ?? Guid.NewGuid().ToString();
		response.CorrelationalId = request.CorrelationalId;

		response.Accounts = new List<Account>()
		{
			new Account()
			{
				AccountNumber = (new Random()).Next(999999, 999999999).ToString().PadLeft(16, '0'),
				AccountBalance = 100000000,
				AccountTypeId = "80",
				AccountTypeName = "Monedero General",
				CardEnabled = true,
				CardId = 14141414,
				CardStatusId = "0",
				CardStatusName = "Active",
				CustomerId = LongRandom(10000000, 10000000000000),
				CustomerStatusId = 0,
				CustomerStatusName = "Dummy Customer Status",
				CustomerTypeId = LongRandom(10000000, 10000000000000),
				CustomerTypeName = "Dummy Customer Type",
				Pan = (new Random()).Next(999999, 999999999).ToString().PadLeft(16, '0')
			}
		};

		if (request.DocNumber.StartsWith("0"))
		{
			response.Accounts = null;
			return;
		}
	});
}

void GetMiniStatements(IModel channel)
{
	string routingKey = "Processa.RabbitMQ.Services.Bifrost.Contracts.MiniStatementsRequest:Processa.RabbitMQ.Services.Bifrost";
	this.GetResponse(channel, routingKey, (MiniStatementsRequest request, MiniStatementsResponse response) =>
	{
		Console.WriteLine($"[.] ({DateTime.Now.ToString("HH:mm:ss.fff")}) Card Holder Request Received...");
		request.CorrelationalId = request.CorrelationalId ?? Guid.NewGuid().ToString();
		response.CorrelationalId = request.CorrelationalId;

		response.Statements = new List<Statement>()
		{
			new Statement()
			{
				CardAcceptorName = "Pedro Pablo",
				Amount = 10000,
				CardAcceptorId = "14141414",
				Category = "Debit",
				ExtendedTransactionType = "0",
				FromAccountNumber = (new Random()).Next(999999, 999999999).ToString().PadLeft(16, '0'),
				FromAccountTypeId = "80",
				FromAccountTypeName = "Monedero General",
				Name = "Transferencia de fondos",
				ToAccountNumber = (new Random()).Next(999999, 999999999).ToString().PadLeft(16, '0'),
				ToAccountTypeId = "81",
				ToAccountTypeName = "Subsidio Familiar",
				TransactionDate = DateTime.Now,
				TransactionType = string.Empty
			}
		};

		if (request.DocNumber.StartsWith("0"))
		{
			response.Statements = null;
			return;
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
	string ChannelId { get; set; }
}

interface IResponse
{
	string CorrelationalId { get; set; }
	int ResponseCode { get; set; }
	string ResponseMessage { get; set; }
	bool Successful { get; }
}

class CardHolderRequest : IRequest
{
	public string CorrelationalId { get; set; }
	public string DocType { get; set; }
	public string DocNumber { get; set; }
	public string ChannelId { get; set; }
}

class CardHolderResponse : IResponse
{
	public string CorrelationalId { get; set; }
	public int ResponseCode { get; set; } = (int)HttpStatusCode.OK;
	public string ResponseMessage { get; set; } = HttpStatusCode.OK.ToString();
	public bool Successful => this.ResponseCode >= 200 | this.ResponseCode <= 299;
	public IList<CardHolder> CardHolders { get; set; }
}

class CardHolder
{
	public string Address { get; set; }
	public string CardLabel { get; set; }
	public string CityCode { get; set; }
	public string CityName { get; set; }
	public string Code { get; set; }
	public DateTime Creation { get; set; }
	public string DocNumber { get; set; }
	public string DocType { get; set; }
	public string Email { get; set; }
	public string FirstName { get; set; }
	public long PersonId { get; set; }
	public string LastName { get; set; }
	public string RegionName { get; set; }
	public string Telephone { get; set; }
	public string CustomerGroup { get; set; }
}

class BalanceRequest : IRequest
{
	public string CorrelationalId { get; set; }
	public string DocType { get; set; }
	public string DocNumber { get; set; }
	public string ChannelId { get; set; }
}

class BalanceResponse : IResponse
{
	public string CorrelationalId { get; set; }
	public int ResponseCode { get; set; } = (int)HttpStatusCode.OK;
	public string ResponseMessage { get; set; } = HttpStatusCode.OK.ToString();
	public bool Successful => this.ResponseCode >= 200 | this.ResponseCode <= 299;
	public List<Account> Accounts { get; set; }
}

class Account
{
	public long CustomerId { get; set; }
	public long CustomerTypeId { get; set; }
	public string CustomerTypeName { get; set; }
	public int CustomerStatusId { get; set; }
	public string CustomerStatusName { get; set; }
	public long CardId { get; set; }
	public string CardStatusId { get; set; }
	public string CardStatusName { get; set; }
	public bool CardEnabled { get; set; }
	public string Pan { get; set; }
	public string AccountTypeId { get; set; }
	public string AccountTypeName { get; set; }
	public string AccountNumber { get; set; }
	public Decimal AccountBalance { get; set; }
}

class MiniStatementsRequest : IRequest
{
	public string CorrelationalId { get; set; }
	public string DocType { get; set; }
	public string DocNumber { get; set; }
	public string AccountTypeId { get; set; }
	public string ChannelId { get; set; }
}

class MiniStatementsResponse : IResponse
{
	public string CorrelationalId { get; set; }
	public int ResponseCode { get; set; } = (int)HttpStatusCode.OK;
	public string ResponseMessage { get; set; } = HttpStatusCode.OK.ToString();
	public bool Successful => this.ResponseCode >= 200 | this.ResponseCode <= 299;
	public List<Statement> Statements { get; set; }
}

class Statement
{
	public string Name { get; set; }
	public string Category { get; set; }
	public Decimal Amount { get; set; }
	public string CardAcceptorId { get; set; }
	public string CardAcceptorName { get; set; }
	public DateTime TransactionDate { get; set; }
	public string FromAccountTypeId { get; set; }
	public string FromAccountTypeName { get; set; }
	public string FromAccountNumber { get; set; }
	public string ToAccountTypeId { get; set; }
	public string ToAccountTypeName { get; set; }
	public string ToAccountNumber { get; set; }
	public string TransactionType { get; set; }
	public string ExtendedTransactionType { get; set; }
}

long LongRandom(long min, long max, Random rand = null)
{
	rand = rand ?? new Random();
    long result = rand.Next((Int32)(min >> 32), (Int32)(max >> 32));
    result = (result << 32);
    result = result | (long)rand.Next((Int32)min, (Int32)max);
    return result;
}