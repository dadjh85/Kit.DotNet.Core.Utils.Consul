using Kit.DotNet.Core.Utils.Consul.Models;
using Kit.DotNet.Core.Utils.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Kit.DotNet.Core.Utils.Consul.Services
{
    /// <summary>
    /// Interface of the service ConsulKvService
    /// </summary>
    public interface IConsulKvService
    {
        /// <summary>
        /// method for uploading a file or a list of files to the consul server.
        /// </summary>
        /// <param name="consulConfigurationFile">object with the configuration options for file uploading</param>
        /// <param name="environment">the .NET Core application environment</param>
        /// <returns>a boolean with the result of execution</returns>
        Task<bool> AddFileKv(ConsulConfigurationFile consulConfigurationFile, string environment);

        /// <summary>
        /// gets the list of file names uploaded to the consul server.
        /// </summary>
        /// <param name="client">the client for call API-REST</param>
        /// <returns>a list of strings with the name of all files in consul</returns>
        Task<List<string>> GetListKv(HttpClient client);

        /// <summary>
        /// method for create a object HttpClient for call to consul server
        /// </summary>
        /// <param name="urlConsul">the url of consul server</param>
        /// <returns>a object HttpClient</returns>
        HttpClient CreateConsulClient(string urlConsul);
    }
}
