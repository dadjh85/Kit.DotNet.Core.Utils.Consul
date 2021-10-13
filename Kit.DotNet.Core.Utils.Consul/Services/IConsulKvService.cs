using Kit.DotNet.Core.Utils.Consul.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kit.DotNet.Core.Utils.Consul.Services
{
    public interface IConsulKvService
    {
        Task<List<string>> GetListKv(string urlConsul);

        Task<bool> AddFileKv(ConsulConfigurationFile consulConfigurationFile, string environment);
    }
}
