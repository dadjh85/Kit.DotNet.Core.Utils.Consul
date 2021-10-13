﻿
using System.Collections.Generic;

namespace Kit.DotNet.Core.Utils.Consul.Models
{
    public class ConsulConfigurationFile
    {
        public string Address { get; set; }
        public string UrlFile { get; set; } = "appsettings.json";
        public List<string> UrlFiles { get; set; }
        public bool UploadEnvironmentFile { get; set; } = true;
        public bool ReloadConfigurationAllStartup { get; set; } = true;
    }
}
