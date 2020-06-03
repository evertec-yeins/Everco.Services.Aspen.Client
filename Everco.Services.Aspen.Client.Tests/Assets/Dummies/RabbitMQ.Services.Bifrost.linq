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
			UserName = "test",
			Password = "test",
			VirtualHost = "Bifrost"
		};

		debugMessage = $"[x] Using connection: 'host={factory.HostName};virtualhost={factory.VirtualHost};username={factory.UserName};password={factory.Password};exchange={this.Exchange}'";
		Console.WriteLine(debugMessage);
		flagBluider.AppendLine(debugMessage);
		using (IConnection connection = factory.CreateConnection("Bifrost Dummy RPC Server"))
		{
			using (IModel channel = connection.CreateModel())
			{
				channel.ExchangeDeclare(exchange: Exchange, type: ExchangeType.Direct, durable: false, autoDelete: true, arguments: null);
				this.GetCardHolder(channel);
				this.GetBalances(channel);
				this.GetMiniStatements(channel);

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

List<CardHolder> CardHolders => new List<CardHolder>()
{
	new CardHolder(1, "CC", "79483129"),
	new CardHolder(2, "CC", "52080323"),
	new CardHolder(3, "NJ", "75717277"),
	new CardHolder(4, "CE", "203467"),
	new CardHolder(5, "TI", "94030704708"),
	new CardHolder(6, "PS", "10888002"),
	new CardHolder(7, "CC", "52150900"),
	new CardHolder(8, "CC", "3262308"),
	new CardHolder(9, "CC", "52582664"),
	new CardHolder(10, "CC", "35512889"),
	new CardHolder(11, "CC", "52099375"),
	new CardHolder(12, "CC", "1000047909")
};

Dictionary<string, List<Account>> accounts = null;
Dictionary<string, List<Account>> Accounts
{
	get
	{
		if (this.accounts != null && this.accounts.Any())
		{
			return this.accounts;
		}

		List<Account> GetRandomAccounts(long cardId, string accountNumber)
		{
			List<Account> accounts = Enumerable.Empty<Account>().ToList();
			accounts.Add(new Account(cardId, "80", "Monedero General", accountNumber));
			accounts.Add(new Account(cardId, "81", "Subsidio familiar", accountNumber));
			accounts.Add(new Account(cardId, "82", "Subsidio Educativo", accountNumber));
			accounts.Add(new Account(cardId, "83", "Bonos", accountNumber));
			accounts.Add(new Account(cardId, "84", "Viveres General", accountNumber));
			return accounts.OrderBy(x => x.AccountTypeId).ToList();
		}

		this.accounts = new Dictionary<string, List<Account>>()
		{
			{ "CC-79483129", GetRandomAccounts(203945, "6039590286132628") },
			{ "CC-52080323", GetRandomAccounts(84, "6039597499604889") },
			{ "NJ-75717277", GetRandomAccounts(1465021, "6039591139502645") },
			{ "CE-203467", GetRandomAccounts(830632, "6039599197746541") },
			{ "TI-94030704708", GetRandomAccounts(343175, "6039590361618343") },
			{ "PS-10888002", GetRandomAccounts(1478394, "6039590830592384") },
			{ "CC-52150900", GetRandomAccounts(640067, "6039594610201554") },
			{ "CC-3262308", GetRandomAccounts(1234113, "6039590635149836") },
			{ "CC-52582664", GetRandomAccounts(138665, "6039594246705697") },
			{ "CC-35512889", GetRandomAccounts(617984, "6039599272117600") },
			{ "CC-52099375", GetRandomAccounts(131202, "6039593514025838") },
			{ "CC-1000047909", GetRandomAccounts(696026, "6039593151183452") }
		};

		return this.accounts;
	}
}

Dictionary<string, List<Statement>> statements = null;
Dictionary<string, List<Statement>> Statements
{
	get
	{
		if (this.statements != null && this.statements.Any())
		{
			return this.statements;
		}
		
		List<Statement> GetRandomStatements()
		{
			List<Statement> statements = Enumerable.Empty<Statement>().ToList();
			for (int index = 1; index <= 5; index++)
			{
				statements.Add(new Statement("80", "Monedero General"));
				statements.Add(new Statement("81", "Subsidio familiar"));
				statements.Add(new Statement("82", "Subsidio Educativo"));
				statements.Add(new Statement("83", "Bonos"));
				statements.Add(new Statement("84", "Viveres General"));
			}

			return statements.OrderBy(x => x.TransactionDate).ToList();
		}
		
		this.statements = new Dictionary<string, List<Statement>>()
		{
			{ "CC-79483129", GetRandomStatements() },
			{ "CC-52080323", GetRandomStatements() },
			{ "NJ-75717277", GetRandomStatements() },
			{ "CE-203467", GetRandomStatements() },
			{ "TI-94030704708", GetRandomStatements() },
			{ "PS-10888002", GetRandomStatements() },
			{ "CC-52150900", GetRandomStatements() },
			{ "CC-3262308", GetRandomStatements() },
			{ "CC-52582664", GetRandomStatements() },
			{ "CC-35512889", GetRandomStatements() },
			{ "CC-52099375", GetRandomStatements() },
			{ "CC-1000047909", GetRandomStatements() },
		};

		return this.statements;
	}
}

void GetCardHolder(IModel channel)
{
	string routingKey = "Processa.RabbitMQ.Services.Bifrost.Contracts.CardHolderRequest:Processa.RabbitMQ.Services.Bifrost";
	this.GetResponse(channel, routingKey, (CardHolderRequest request, CardHolderResponse response) =>
	{
		Console.WriteLine($"[x] ({DateTime.Now.ToString("HH:mm:ss.fff")}) Card Holder Request [{request.DocType}-{request.DocNumber}] Received...");
		request.CorrelationalId = request.CorrelationalId ?? Guid.NewGuid().ToString();
		response.CorrelationalId = request.CorrelationalId;
		response.CardHolders = CardHolders.Where(ch => ch.DocType == request.DocType & ch.DocNumber == request.DocNumber).ToList();
	});
}

void GetBalances(IModel channel)
{
	string routingKey = "Processa.RabbitMQ.Services.Bifrost.Contracts.BalanceRequest:Processa.RabbitMQ.Services.Bifrost";
	this.GetResponse(channel, routingKey, (BalanceRequest request, BalanceResponse response) =>
	{
		Console.WriteLine($"[x] ({DateTime.Now.ToString("HH:mm:ss.fff")}) Balances Request [{request.DocType}-{request.DocNumber}] Received...");
		request.CorrelationalId = request.CorrelationalId ?? Guid.NewGuid().ToString();
		response.CorrelationalId = request.CorrelationalId;

		string key = $"{request.DocType}-{request.DocNumber}";
		if (Accounts.TryGetValue(key, out List<Account> accounts))
		{
			response.Accounts = accounts;
		}
	});
}

void GetMiniStatements(IModel channel)
{
	string routingKey = "Processa.RabbitMQ.Services.Bifrost.Contracts.MiniStatementsRequest:Processa.RabbitMQ.Services.Bifrost";
	this.GetResponse(channel, routingKey, (MiniStatementsRequest request, MiniStatementsResponse response) =>
	{
		Console.WriteLine($"[x] ({DateTime.Now.ToString("HH:mm:ss.fff")}) Statements Request [{request.DocType}-{request.DocNumber}] Received...");
		request.CorrelationalId = request.CorrelationalId ?? Guid.NewGuid().ToString();
		response.CorrelationalId = request.CorrelationalId;

		string key = $"{request.DocType}-{request.DocNumber}";
		if (Statements.TryGetValue(key, out List<Statement> statements))
		{
			response.Statements = statements.Where(s => Regex.IsMatch(s.FromAccountTypeId, request.AccountTypeId)).ToList();
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
	public CardHolder()
	{
		this.Code = new Random().Next(9999999, 999999999).ToString("0000000000000000");
		this.FirstName = "Maribel";
		this.LastName = "Marquez Sosa";
		this.CardLabel = "MARIBEL MARQUEZ S.";
		this.Telephone = null;
		this.Address = null;
		this.Email = null;
		this.CityCode = "11001";
		this.CityName = "Bogotá";
		this.RegionName = "Bogotá";
		this.CustomerGroup = "01";
		this.Creation = DateTime.Now.AddYears(-(new Random().Next(3, 20)));
	}

	public CardHolder(long personId, string docType, string docNumber) : this()
	{
		this.PersonId = personId;
		this.DocType = docType;
		this.DocNumber = docNumber;
	}
	
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
	public BalanceResponse()
	{
		this.Accounts = Enumerable.Empty<Account>().ToList();
	}
	
	public string CorrelationalId { get; set; }
	public int ResponseCode { get; set; } = (int)HttpStatusCode.OK;
	public string ResponseMessage { get; set; } = HttpStatusCode.OK.ToString();
	public bool Successful => this.ResponseCode >= 200 | this.ResponseCode <= 299;
	public List<Account> Accounts { get; set; }
}

class Account
{
	public Account()
	{
		this.CustomerId = new Random().Next(999, 99999);
		this.CustomerTypeId = 1;
		this.CustomerTypeName = "AFILIADO";
		this.CustomerStatusId = 1;
		this.CustomerStatusName = "CLIENTE TIENE TARJETA";
		this.CardId = new Random().Next(999, 99999);
		this.CardStatusId = null;
		this.CardStatusName = "OK";
		this.CardEnabled = true;
		this.Pan = new Random().Next(99999999, 999999999).ToString("0000000000000000");
		this.AccountTypeId = "80";
		this.AccountTypeName = "Monedero General";
		this.AccountNumber = $"{this.Pan}{this.AccountTypeId}";
		this.AccountBalance = 2000000;
	}
	
	public Account(
		long cardId,
		string accountTypeId,
		string accountTypeName,
		string accountNumber) : this()
	{
		this.CardId = cardId;
		this.Pan = accountNumber;
		this.AccountTypeId = accountTypeId;
		this.AccountTypeName = accountTypeName;
		this.AccountNumber = $"{this.Pan}{this.AccountTypeId}";
	}
	
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
	public MiniStatementsResponse()
	{
		this.Statements = Enumerable.Empty<Statement>().ToList();
	}
	
	public string CorrelationalId { get; set; }
	public int ResponseCode { get; set; } = (int)HttpStatusCode.OK;
	public string ResponseMessage { get; set; } = HttpStatusCode.OK.ToString();
	public bool Successful => this.ResponseCode >= 200 | this.ResponseCode <= 299;
	public List<Statement> Statements { get; set; }
}

class Statement
{
	public Statement()
	{
		string accountTypeId = "80";
		string accountTypeName = "Monedero General";
		Random random = new Random();
		this.Name = "Transferencia de fondos";
		this.Category = Accounting.Debit.ToString();
		this.Amount = 10000;
		this.CardAcceptorId = "141414";
		this.CardAcceptorName = "Acme Corporation";
		this.TransactionType = "40";
		this.ExtendedTransactionType = "9100";
		this.TransactionDate = DateTime.Now
			.AddDays(-(random.Next(2, 10)))
			.AddHours(random.Next(0, 24))
			.AddMinutes(random.Next(0, 60))
			.AddSeconds(random.Next(0, 60));
		this.FromAccountTypeId = accountTypeId;
		this.FromAccountTypeName = accountTypeName;
		this.FromAccountNumber = $"{random.Next(1000000, 9999999)}{random.Next(1000000, 9999999)}{accountTypeId}";
		this.ToAccountTypeId = accountTypeId;
		this.ToAccountTypeName = accountTypeName;
		this.ToAccountNumber = $"{random.Next(1000000, 9999999)}{random.Next(1000000, 9999999)}{accountTypeId}";
	}
	
	public Statement(string accountTypeId, string accountTypeName) : this()
	{
		Random random = new Random();
		this.FromAccountTypeId = accountTypeId;
		this.FromAccountTypeName = accountTypeName;
		this.FromAccountNumber = $"{random.Next(1000000, 9999999)}{random.Next(1000000, 9999999)}{accountTypeId}";
		this.ToAccountTypeId = accountTypeId;
		this.ToAccountTypeName = accountTypeName;
		this.ToAccountNumber = $"{random.Next(1000000, 9999999)}{random.Next(1000000, 9999999)}{accountTypeId}";
	}
	
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

enum Accounting
{
	Debit = 0,
	Credit = 1
}