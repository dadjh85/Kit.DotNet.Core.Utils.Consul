using Kit.DotNet.Core.Utils.Consul.Models;
using Kit.DotNet.Core.Utils.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Kit.DotNet.Core.Utils.Consul.Services.ConsulClientService
{
    public interface IConsulClientService
    {
        /// <summary>
        /// method that makes a put call to the consul api, which allows a file to be uploaded to the server
        /// </summary>
        /// <param name="client">the client for call API-REST</param>
        /// <param name="consulConfigurationFile">object with the configuration options for file uploading</param>
        /// <param name="fileName">the name of file</param>
        /// <param name="contentFile">the content of file</param>
        /// <returns>the response http and a string with the result of execution</returns>
        Task<Response<string>> UploadFile(HttpClient client, ConsulConfigurationFile consulConfigurationFile, string fileName, string contentFile);

        /// <summary>
        /// gets the list of file names uploaded to the consul server.
        /// </summary>
        /// <param name="client">the client for call to consul</param>
        /// <returns>a list of strings with the name of all files in consul</returns>
        Task<Response<List<string>>> GetListKv(HttpClient client);

        /// <summary>
        /// method for create a object HttpClient for call to consul server
        /// </summary>
        /// <param name="urlConsul">the url of consul server</param>
        /// <returns>a object HttpClient</returns>
        HttpClient CreateConsulClient(string urlConsul);
    }
}
