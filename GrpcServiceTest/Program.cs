using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GrpcTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        // Additional configuration is required to successfully run gRPC on macOS.
        // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    //���ò�����TLS��HTTP/2�ս��
                    //Kestrel ��֧�� macOS �͸���� Windows �汾���� Windows 7���ϵĴ��� TLS �� HTTP/2
                    webBuilder.ConfigureKestrel(option => option.ListenLocalhost(5001, action => action.Protocols
                       = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2));
                    webBuilder.UseStartup<Startup>();
                });
    }
}