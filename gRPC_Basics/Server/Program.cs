using Server.Data;
using Server.Hello;
using Server.Services;
using G = Grpc.Core;
new ServerDbContext().Database.EnsureDeleted();
new ServerDbContext().Database.EnsureCreated();

G.Server server = new G.Server()
{
    Ports = { new G.ServerPort("localhost", 7777, G.ServerCredentials.Insecure) },
    Services = { HelloService.BindService(new HelloServiceImpl()),
        PersonService.BindService(new PersonServiceImpl()),
    MathService.BindService(new MathServiceImpl())}
};
try
{
    server.Start();
    Console.WriteLine("server is running");
    Console.ReadLine();
}
catch (Exception)
{

    throw;
}
finally
{
    if (server is not null)
        await server.ShutdownAsync();
}