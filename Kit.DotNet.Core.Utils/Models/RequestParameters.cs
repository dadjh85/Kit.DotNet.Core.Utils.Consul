using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Kit.DotNet.Core.Utils.Models
{
    public class RequestParameters
    {
        public string Url { get; set; }
        public string Token { get; set; }
        public AuthenticationHeaderValue AuthenticationHeaderValue { get; set; }
        public List<RequestHeader> RequestHeaders { get; set; }
        public HttpContent HttpContent { get; set; }
    }
}
