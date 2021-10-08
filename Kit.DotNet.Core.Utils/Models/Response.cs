using System.Net.Http;

namespace Kit.DotNet.Core.Utils.Models
{
    public class Response<T> where T : class
    {
        public HttpResponseMessage HttpResponseMessage { get; set; }
        public T Entity { get; set; }
    }
}
