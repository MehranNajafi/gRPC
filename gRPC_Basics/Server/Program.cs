using Grpc.Core;
using Grpc.Reflection;
using Grpc.Reflection.V1Alpha;
using Maths;
using People;
using Server.Data;
using Server.Hello;
using Server.Services;
using G = Grpc.Core;
new ServerDbContext().Database.EnsureDeleted();
new ServerDbContext().Database.EnsureCreated();

//var serverCert = File.ReadAllText("G:\\Github\\gRPC\\gRPC_Basics\\Server\\server.crt");
//var serverKey = File.ReadAllText("G:\\Github\\gRPC\\gRPC_Basics\\Server\\server.key");
//var rootCA = File.ReadAllText("G:\\Github\\gRPC\\gRPC_Basics\\Server\\ca.crt");

var reflectionService = new ReflectionServiceImpl(
    HelloService.Descriptor,
    MathService.Descriptor,
    PersonService.Descriptor,
    ServerReflection.Descriptor);

//SslServerCredentials ssl = new SslServerCredentials(new[] {
//    new KeyCertificatePair(serverCert, serverKey)
//}, rootCA, SslClientCertificateRequestType.RequestAndRequireAndVerify);

G.Server server = new G.Server()
{
    Ports = { new G.ServerPort("localhost", 1070, ServerCredentials.Insecure) },
    Services = { HelloService.BindService(new HelloServiceImpl()),
        PersonService.BindService(new PersonServiceImpl()),
    MathService.BindService(new MathServiceImpl()),
    ServerReflection.BindService(reflectionService)}
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