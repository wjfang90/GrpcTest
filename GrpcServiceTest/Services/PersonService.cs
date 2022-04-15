using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcTest;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrpcServiceTest.Services
{
    public class PersonService : People.PeopleBase
    {
        private readonly ILogger<PersonService> _logger;
        public PersonService(ILogger<PersonService> logger)
        {
            _logger = logger;
        }

        public override Task<HiReply> SayHi(HiRequest request, ServerCallContext context)
        {
            var result = new HiReply
            {
                Message = "Hi " + request.Name,
                ReplyTime = Timestamp.FromDateTime(DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)),
                WordCount = null,
            };

            result.Rules.Add(new[] { "fang", "test" });//RepeatedField没有公共setter，只能add
            result.DataDict.Add(new Dictionary<string, string>() { { "key","value" } }); //MapField 没有公共setter，只能add

            return Task.FromResult(result);
        }
    }
}
