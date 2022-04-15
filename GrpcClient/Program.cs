using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace GrpcClient
{
    class Program
    {
        async static Task Main(string[] args)
        {
            var fileName = System.IO.Path.Combine(AppContext.BaseDirectory, "test.txt");
            await System.IO.File.WriteAllLinesAsync(fileName, new[] { "hello" });

            GC.Collect();

            await GreeterSayHello();

            await PersonSayHi();

        }

        private static async Task GreeterSayHello()
        {
            //设置允许不安全的HTTP2支持
            //只有 .NET Core 3.x 需要 System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport 开关。 .NET 5 中不需要任何额外配置，也没有这项要求
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            var serviceAddr = "http://localhost:5001";

            using var channel = GrpcChannel.ForAddress(serviceAddr);

            var greeterClient = new Greeter.GreeterClient(channel);

            var helloRequest = new HelloRequest()
            {
                Name = "fang"
            };

            try
            {
                var greeterReply = await greeterClient.SayHelloAsync(helloRequest, deadline: DateTime.UtcNow.AddSeconds(10));

                Console.WriteLine("Greeting: " + greeterReply.Message);
            }
            catch (RpcException ex)
            {
                var greeterReply = await greeterClient.SayHelloAsync(helloRequest, deadline: DateTime.UtcNow.AddSeconds(10));
                Console.WriteLine(" RpcException Greeting: " + greeterReply.Message);
            }

        }

        private static async Task PersonSayHi()
        {
            //设置允许不安全的HTTP2支持
            //只有 .NET Core 3.x 需要 System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport 开关。 .NET 5 中不需要任何额外配置，也没有这项要求
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            var serviceAddr = "http://localhost:5001";

            //配置重试策略

            var config = new MethodConfig()
            {
                Names = { MethodName.Default },
                RetryPolicy = new RetryPolicy()
                {
                    MaxAttempts = 3,
                    InitialBackoff = TimeSpan.FromSeconds(1),
                    MaxBackoff = TimeSpan.FromSeconds(5),
                    BackoffMultiplier = 1.5,
                    RetryableStatusCodes = { StatusCode.Unavailable }
                }
            };

            var options = new GrpcChannelOptions()
            {
                ServiceConfig = new ServiceConfig()
                {
                    MethodConfigs = { config }
                }
            };

            using var channel = GrpcChannel.ForAddress(serviceAddr, options);

            var personClient = new People.PeopleClient(channel);

            var hiRequest = new HiRequest()
            {
                Name = "fang"
            };

            var personReply = await personClient.SayHiAsync(hiRequest, deadline: DateTime.UtcNow.AddSeconds(10));

            Console.WriteLine("Person: " + personReply.Message + "-------" + personReply.ReplyTime.ToDateTime());
        }
    }
}
