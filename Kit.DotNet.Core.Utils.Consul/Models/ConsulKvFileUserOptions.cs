using System.Collections.Generic;

namespace Kit.DotNet.Core.Utils.Consul.Models
{
    public class ConsulKvFileUserOptions
    {
        public string Address { get; set; }
        public bool? UploadEnviromentFile { get; set; }
        public bool? ReloadConfigurationAllStartup { get; set; } = true;
        public List<string> UrlFiles { get; set; }
    }
}
