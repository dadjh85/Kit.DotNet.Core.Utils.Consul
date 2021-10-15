using Kit.DotNet.Core.Utils.Consul.Services;
using Moq;
using System.Net.Http;
using Xunit;

namespace Tests.Kit.DotNet.Core.Utils.Consul.Tests
{
    public class ConsulKvServiceTest
    {

        #region Private Methods

        private IConsulKvService CreateIConsulKvService()
        {
            Mock<IHttpClientFactory> consulclientFactory = new Mock<IHttpClientFactory>();

            return new ConsulKvService(consulclientFactory.Object);
        }

        #endregion
    }
}
