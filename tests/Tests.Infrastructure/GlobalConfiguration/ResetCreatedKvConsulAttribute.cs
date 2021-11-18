using Consul;
using System;
using System.Reflection;
using Xunit.Sdk;

namespace Tests.Tests.Infrastructure.GlobalConfiguration
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ResetCreatedKvConsulAttribute : BeforeAfterTestAttribute
    {
        public const string PREFIX_CONSUL_TEST = "Tests.Kit.DotNet.Core.Utils.Consul.Tests/";

        public override void After(MethodInfo methodUnderTest)
        {
            var client = new ConsulClient();
            client.KV.DeleteTree(PREFIX_CONSUL_TEST).Wait();
            client.Dispose();
        }
    }
}
