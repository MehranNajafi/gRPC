using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Server.Hello;

Channel channel = new Channel("localhost:7777", ChannelCredentials.Insecure);
try
{
    await channel.ConnectAsync();
    var personService = new PersonService.PersonServiceClient(channel);
    int page = 1;
    var getbyPage = personService.GetByPage();
    _ = Task.Run(async () =>
    {
        while (await getbyPage.ResponseStream.MoveNext())
        {
            Console.Clear();
            var person = getbyPage.ResponseStream.Current;
            Console.WriteLine("page is " + person.Page);
            foreach (var item in person.People)
                Console.WriteLine(item.FName, " ", item.LName);
        }
    });
    ConsoleKeyInfo? key = null;
    while (key?.Key != ConsoleKey.Q)
    {
        if (key?.Key == ConsoleKey.RightArrow) page++;
        else if (key?.Key == ConsoleKey.LeftArrow) page = Math.Max(1, page - 1);
        await getbyPage.RequestStream.WriteAsync(new GetByPageRequest { Page = page });
        key = Console.ReadKey();
    }
    await getbyPage.RequestStream.CompleteAsync();
    Console.ReadLine();
}
catch (Exception)
{
    throw;
}
finally
{
    if (channel is not null)
        await channel.ShutdownAsync();
}

static async Task CreatePerson(PersonService.PersonServiceClient personService)
{
    Console.WriteLine("Enter your name :");
    var fName = Console.ReadLine();
    Console.WriteLine("Enter your familyName :");
    var lName = Console.ReadLine();
    var response = await personService.CreatePersonAsync(new CreatePersonRequest { FName = fName, LName = lName });
    Console.WriteLine(response.Id);
}

static async Task ResponseWithCollection(Channel channel)
{
    var helloService = new HelloService.HelloServiceClient(channel);
    var helloRequest = new HelloRequest
    {
        Identity = new Common.Identity
        {
            FName = "Mehran",
            LName = "Najafi"
        },
        BirthDate = Timestamp.FromDateTime(new DateTime(1992, 8, 7))
    };
    helloRequest.Children.Add(new[] {new Child { Identity = new Common.Identity{ FName="M1" } },
    new Child { Identity = new Common.Identity{FName="M2"} },
    new Child { Identity = new Common.Identity{FName="M3"} } });
    var response = await helloService.WelcomeAsync(helloRequest);
    Console.WriteLine(response.Message);
}

static async Task GetAllPerson(Channel channel)
{
    var personService = new PersonService.PersonServiceClient(channel);
    var response = await personService.GetAllAsync(new GetAllRequest { });
    foreach (var person in response.People) { Console.WriteLine(person.FName); }
}

static async Task Factorial(Channel channel)
{
    var mathService = new MathService.MathServiceClient(channel);
    var response = mathService.Factorial(new FactorialRequest { Value = 10 });
    while (await response.ResponseStream.MoveNext())
    {
        Console.WriteLine($"factorial {response.ResponseStream.Current.Value} is" +
            $" {response.ResponseStream.Current.Result}");
    }
}

static async Task Average(Channel channel)
{
    var mathService = new MathService.MathServiceClient(channel);
    var average = mathService.Avg();
    var number = Console.ReadLine();
    while (number != "q")
    {
        if (int.TryParse(number, out var n))
            await average.RequestStream.WriteAsync(new ComputerAvgRequest { Value = n });
        number = Console.ReadLine();
    }
    await average.RequestStream.CompleteAsync();
    var result = await average;
    Console.WriteLine("Average is " + result.Result.ToString(".0000"));
}

static async Task Sum(Channel channel)
{
    var mathService = new MathService.MathServiceClient(channel);
    var sum = mathService.Sum();
    _ = Task.Run(async () =>
    {
        while (await sum.ResponseStream.MoveNext())
            Console.WriteLine("sum result is " + sum.ResponseStream.Current.Result.ToString());
    });
    Console.WriteLine("the client connected to the server");
    var number = Console.ReadLine();
    while (number != "q")
    {
        if (int.TryParse(number, out var n))
            await sum.RequestStream.WriteAsync(new ComputerSumRequest { Value = n });
        number = Console.ReadLine();
    }
    await sum.RequestStream.CompleteAsync();
}