<Query Kind="Program">
  <NuGetReference>EasyNetQ</NuGetReference>
  <Namespace>EasyNetQ</Namespace>
  <Namespace>RabbitMQ.Client</Namespace>
  <Namespace>System.Net</Namespace>
</Query>

void Main()
{
	string flagPath = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), $"{Util.CurrentQuery.Name}.flag");
	var bus = RabbitHutch.CreateBus(@"host=localhost;username=guest;password=guest");
	$"{typeof(MyRequest).FullName}, {Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location)}".Dump("RoutingKey");
	
	try
	{
		if (!(bus.IsConnected))
		{
			throw new InvalidOperationException("IsConnected:False");
		}

		bus.Respond<MyRequest, MyResponse>(request => 
		{ 
			var response =  new Processor().GetResponse(request);
			response.Dump("Response recieved");
			return response;
		});
		
		bus.Request<MyRequest, MyResponse>(new MyRequest());
	}
	finally
	{
		if (File.Exists(flagPath))
		{
			File.Delete(flagPath);
		}
	}
	
	Console.ReadLine();
	bus?.Dispose();
}

public class MyRequest
{
	public MyRequest(int? a = null, int? b = null,  char @operator = '+')
	{
		a = a ?? new Random(Guid.NewGuid().GetHashCode()).Next(1, 10);
		b = b ?? new Random(Guid.NewGuid().GetHashCode()).Next(1, 10);
		this.Input = $"{a}{@operator}{b}";
	}
	
	public string Input { get; set; }
}

//[Queue("TestMessagesQueue", ExchangeName = "MyTestExchange")]
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
	public static MyResponse MismatchedRequest => new MyResponse { ResponseCode = (int)HttpStatusCode.BadRequest, ResponseMessage = "Invalid Input", Result = int.MinValue};
	public static MyResponse FromInvalidOperator(string @operator) => new MyResponse { ResponseCode = (int)HttpStatusCode.BadRequest, ResponseMessage = $"Invalid Operator: {@operator}", Result = -1};
	public static MyResponse FromException(Exception exception) => new MyResponse { ResponseCode = (int)HttpStatusCode.InternalServerError, Result = 0, Text = $"{exception.GetType().Name}:{exception.Message}"};
}

public class Processor
{
	public MyResponse GetResponse(MyRequest request)
	{
		request.Dump("Request recieved");
		var m = Regex.Match(request.Input, @"(?<left>\d{1,})(?<operator>\+|\-|\*|\/)(?<right>\d{1,})");
		
		if (!m.Success)
		{
			return MyResponse.MismatchedRequest;
		}
		
		try
		{
			int left = Convert.ToInt32(m.Groups["left"].Value);
			int right = Convert.ToInt32(m.Groups["right"].Value);
			string @operator = m.Groups["operator"].Value;

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