<Query Kind="Program">
  <NuGetReference>EasyNetQ</NuGetReference>
  <Namespace>EasyNetQ</Namespace>
  <Namespace>RabbitMQ.Client</Namespace>
  <Namespace>System.Net</Namespace>
  <Namespace>RabbitMQ.Client.Events</Namespace>
  <Namespace>Newtonsoft.Json</Namespace>
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
			VirtualHost = "/"
		};

		debugMessage = $"[x] Using connection: 'host={factory.HostName};virtualhost={factory.VirtualHost};username={factory.UserName};password={factory.Password};exchange={this.Exchange}'";
		Console.WriteLine(debugMessage);
		flagBluider.AppendLine(debugMessage);
		using (IConnection connection = factory.CreateConnection("Calc RPC Server"))
		{
			using (IModel channel = connection.CreateModel())
			{
				channel.ExchangeDeclare(exchange: Exchange, type: ExchangeType.Direct, durable: false, autoDelete: true, arguments: null);
				this.GetCalcResponse(channel);

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

void GetCalcResponse(IModel channel)
{
	string routingKey = "Temporal.Tests.Demo.CalcRequest";
	channel.QueueDeclare(queue: routingKey, durable: false, exclusive: false, autoDelete: true, arguments: null);
	channel.QueueBind(queue: routingKey, exchange: Exchange, routingKey: routingKey, arguments: null);
	EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
	channel.BasicConsume(queue: routingKey, autoAck: false, consumer: consumer);
	consumer.Received += (model, ea) =>
	{
		Console.WriteLine($"[x] ({DateTime.Now.ToString("HH:mm:ss.fff")}) Calc Request Received...");
		var body = ea.Body.ToArray();
		var props = ea.BasicProperties;
		var replyProps = channel.CreateBasicProperties();
		replyProps.CorrelationId = props.CorrelationId;
		MyResponse response = MyResponse.Undefined();

		try
		{
			string message = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(body))?.ToString();
			MyRequest request = JsonConvert.DeserializeObject<MyRequest>(message);
			Processor processor = new Processor();
			response = processor.GetResponse(request);
			Console.WriteLine($"[x] ({DateTime.Now.ToString("HH:mm:ss.fff")}) Result => {request.Input} = {response.Result}");
		}
		finally
		{
			var responseBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(response));
			channel.BasicPublish(exchange: string.Empty, routingKey: props.ReplyTo, basicProperties: replyProps, body: responseBytes);
			channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
		}
	};
}

public class MyRequest
{
	public MyRequest(int? a = null, int? b = null, char @operator = '+')
	{
		a = a ?? new Random(Guid.NewGuid().GetHashCode()).Next(1, 10);
		b = b ?? new Random(Guid.NewGuid().GetHashCode()).Next(1, 10);
		this.Input = $"{a}{@operator}{b}";
	}

	public string Input { get; set; }
}

public class MyResponse
{
	private MyResponse()
	{
	}

	public MyResponse(long result)
	{
		this.ResponseCode = 200;
		this.ResponseMessage = "OK";
		this.Result = result;
	}
	public string Text { get; set; }
	public int ResponseCode { get; set; }
	public string ResponseMessage { get; set; }
	public long Result { get; set; }
	public static MyResponse MismatchedRequest => new MyResponse { ResponseCode = (int)HttpStatusCode.BadRequest, ResponseMessage = "Invalid Input", Result = int.MinValue };
	public static MyResponse FromInvalidOperator(string @operator) => new MyResponse { ResponseCode = (int)HttpStatusCode.BadRequest, ResponseMessage = $"Invalid Operator: {@operator}", Result = -1};
	public static MyResponse FromException(Exception exception) => new MyResponse { ResponseCode = (int)HttpStatusCode.InternalServerError, Result = 0, Text = $"{exception.GetType().Name}:{exception.Message}"};
	public static MyResponse Undefined() => new MyResponse { ResponseCode = (int)HttpStatusCode.NoContent, Result = 0, Text = $"Undefined response"};
}

public class Processor
{
	public MyResponse GetResponse(MyRequest request)
	{
		const RegexOptions RegexOptions = RegexOptions.Singleline | RegexOptions.IgnoreCase;
		Match match = Regex.Match(request.Input, @"(?<left>\d{1,})(?<operator>\+|\-|\*|\/|x)(?<right>\d{1,})", RegexOptions);
		if (!match.Success)
		{
			return MyResponse.MismatchedRequest;
		}
		
		try
		{
			int left = Convert.ToInt32(match.Groups["left"].Value);
			int right = Convert.ToInt32(match.Groups["right"].Value);
			string @operator = match.Groups["operator"].Value;

			switch (@operator)
			{
				case "+":
					return new MyResponse(left + right);

				case "-":
					return new MyResponse(left - right);

				case "/":
					return new MyResponse(left / right);

				case "*":
					return new MyResponse(left * right);

				default:
					return MyResponse.FromInvalidOperator(@operator);
			}
		}
		catch (Exception exception)
		{
			return MyResponse.FromException(exception);
		}
	}
}

/*
// PowerShell Request Example:
$ConnectionString = 'host=localhost;virtualhost=/;username=test;password=test'
$ValueA = (Get-Random -Minimum 10 -Maximum 100)
$ValueB = (Get-Random -Minimum 10 -Maximum 100)
$Operator = '+'
$Body = @{
    Input = '{0}{1}{2}' -f $ValueA, $Operator, $ValueB
}
$Body
$Queue = 'Temporal.Tests.Demo.CalcRequest'
$Exchange = 'temporal_tests_rpc'
$Timeout = (New-TimeSpan -Seconds 10)
Invoke-RabbitRequest -ConnectionString $ConnectionString -Body $Body -Queue $Queue -Exchange $Exchange -Timeout $Timeout
*/

/*
// RabbitClient Request Example:
string connectionString = "host=localhost;virtualhost=/;username=test;password=test;exchange=temporal_tests_rpc";
string routingKey = "Temporal.Tests.Demo.CalcRequest";
Random random = new Random();
int valueA = random.Next(10, 100);
int valueB= random.Next(10, 100);
string @operator = "+";
dynamic request = new ExpandoObject();
request.Input = $"{valueA}{@operator}{valueB}";
LINQPad.Extensions.Dump(request, "Request");
ExpandoObject response = RpcRabbitClient<ExpandoObject, ExpandoObject>
	.Initialize()
	.WithConnectionString(connectionString)
	.BindToRoutingKey(routingKey)
	.ForRequest(request)
	.Invoke();
LINQPad.Extensions.Dump(response, "Response");
*/