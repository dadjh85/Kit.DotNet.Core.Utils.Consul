using System.Collections.Generic;

namespace Kit.DotNet.Core.Utils.Consul.Models
{
    /// <summary>
    /// class with the configuration options that the user wants to set in his project.
    /// </summary>
    public class ConsulKvFileUserOptions
    {
        public string Address { get; set; }
        public bool? UploadEnviromentFile { get; set; }
        public bool? ReloadConfigurationAllStartup { get; set; } = true;
        public List<string> UrlFiles { get; set; }
    }
}
