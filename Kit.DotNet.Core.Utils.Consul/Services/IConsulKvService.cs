using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kit.DotNet.Core.Utils.Consul.Services
{
    public interface IConsulKvService
    {
        Task<List<string>> GetListKv(string urlConsul);

        Task<bool> AddFileKv(string urlConsul, string environment = "Development", string urlFile = "appsettings.json", bool uploadEnvironmentFile = true);
    }
}
