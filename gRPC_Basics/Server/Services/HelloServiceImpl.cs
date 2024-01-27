using Grpc.Core;
using Server.Hello;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Server.Hello.HelloService;

namespace Server.Services
{
    public class HelloServiceImpl : HelloServiceBase
    {
        public override Task<HelloResponse> Welcome(HelloRequest request, ServerCallContext context)
        {
            var message = $"Hello {request.Identity.FName} {request.Identity.LName} and children + " +
                $"{string.Join(",", request.Children.Select(c => c.Identity.FName))}";

                //$" your birthDate is {request.BirthDate.ToDateTime():yyyy-MM-dd}";
            return Task.FromResult(new HelloResponse() { Message = message });
        }
    }
}
