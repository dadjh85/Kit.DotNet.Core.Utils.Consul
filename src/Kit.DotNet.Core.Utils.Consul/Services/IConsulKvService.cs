using Kit.DotNet.Core.Utils.Consul.Models;
using System.Collections.Generic;
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
        /// <param name="urlConsul">the url of the consul server</param>
        /// <returns>a list of strings with the name of all files in consul</returns>
        Task<List<string>> GetListKv(string urlConsul);
    }
}
