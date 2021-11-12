using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Kit.DotNet.Core.Utils.Extensions;
using Kit.DotNet.Core.Utils.Models;
using System.Net;
using System.IO;
using System.Linq;
using System.Reflection;
using Kit.DotNet.Core.Utils.Consul.Models;
using Kit.DotNet.Core.Utils.Consul.Services.ConsulClientService;

namespace Kit.DotNet.Core.Utils.Consul.Services.ConsulKvService
{
    /// <summary>
    /// Service that performs configuration file management in consul
    /// </summary>
    public class ConsulKvService : IConsulKvService
    {
        #region Properties

        private readonly IConsulClientService _consulClientService;

        #endregion

        #region Constructor

        public ConsulKvService(IConsulClientService consulClientService)
        {

            _consulClientService = consulClientService ?? throw new ArgumentNullException(nameof(consulClientService));
        }

        #endregion


        public async Task<bool> AddFileKv(ConsulConfigurationFile consulConfigurationFile, string environment)
        {
            if(consulConfigurationFile == null)
                throw new InvalidOperationException("the ConsulConfigurationFile object cannot be null");

            string appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            List<string> fileNames = ProccessFileNames(consulConfigurationFile, environment);

            bool isUploadAllFiles = true;

            HttpClient client = _consulClientService.CreateConsulClient(consulConfigurationFile.Address);

            Response<List<string>> urlFilesInConsul = await _consulClientService.GetListKv(client);

            foreach (var item in fileNames)
            {
                bool isFileExists = urlFilesInConsul.Entity.Any(x => x.Trim() == consulConfigurationFile.RelativeRouteFileConsul + item.Trim());

                if (consulConfigurationFile.ReloadConfigurationAllStartup || (!consulConfigurationFile.ReloadConfigurationAllStartup && !isFileExists))
                {
                    Response<string> response =  await _consulClientService.UploadFile(client, consulConfigurationFile, item, File.ReadAllText($"{appPath}/{item}"));

                    bool result = Convert.ToBoolean(response.Entity);

                    if (!result)
                        isUploadAllFiles = result;
                }
            }

            return isUploadAllFiles;
        }

      
        #region Private methods

        /// <summary>
        /// processes the name of the configuration files obtaining the configuration file per environment
        /// </summary>
        /// <param name="consulConfigurationFile">object with the configuration options for file uploading</param>
        /// <param name="environment">the .NET Core application environment</param>
        /// <returns>a list of string with all files to process</returns>
        private List<string> ProccessFileNames(ConsulConfigurationFile consulConfigurationFile, string environment)
        {
            if(consulConfigurationFile.UrlFiles == null)
            {
                return GetFileName(consulConfigurationFile.UploadEnvironmentFile, consulConfigurationFile.UrlFile, environment);
            }
            else
            {
                List<string> files = new List<string>();
                foreach(var item in consulConfigurationFile.UrlFiles)
                {
                    files.AddRange(GetFileName(consulConfigurationFile.UploadEnvironmentFile, item, environment));
                }

                return files;
            }
        }

        /// <summary>
        /// method for get the name of the files to process
        /// </summary>
        /// <param name="uploadEnvironmentFile">a bool with the option to obtain the environment file</param>
        /// <param name="urlFile">the url of file to process in consul server</param>
        /// <param name="environment">the .NET Core application environment</param>
        /// <returns>a list of string with the names of files to process</returns>
        private List<string> GetFileName(bool uploadEnvironmentFile, string urlFile, string environment)
        {
            if (uploadEnvironmentFile)
            {
                List<string> partialName = urlFile.SplitToList('.');

                return new List<string>
                {
                    urlFile,
                    $"{partialName.First()}.{environment}.{partialName.LastOrDefault()}"
                };
            }
            else
                return new List<string> { urlFile };
        }

        #endregion
    }
}
