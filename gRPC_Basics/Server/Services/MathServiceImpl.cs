using Azure.Core;
using Grpc.Core;
using Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Maths.MathService;

namespace Server.Services
{
    public class MathServiceImpl : MathServiceBase
    {
        public override async Task Factorial(FactorialRequest request, IServerStreamWriter<FactorialResponse> responseStream, ServerCallContext context)
        {
            long total = 1;
            for (int i = 1; i <= request.Value; i++)
            {
                total *= i;
                await responseStream.WriteAsync(new FactorialResponse { Value = i, Result = total });
                await Task.Delay(1000);
            }
        }
        public override async Task<ComputerAvgResponse> Avg(IAsyncStreamReader<ComputerAvgRequest> requestStream, ServerCallContext context)
        {
            int count = 0;
            float total = 0;
            while (await requestStream.MoveNext())
            {
                count++;
                total = requestStream.Current.Value;
            }
            return new ComputerAvgResponse
            {
                Result = total / count
            };
        }
        public override async Task Sum(IAsyncStreamReader<ComputerSumRequest> requestStream, IServerStreamWriter<ComputerSumResponse> responseStream, ServerCallContext context)
        {
            int total = 0;
            while (await requestStream.MoveNext())
            {
                total += requestStream.Current.Value;
                await responseStream.WriteAsync(new ComputerSumResponse { Result = total }, context.CancellationToken);
            }
        }
    }
}


