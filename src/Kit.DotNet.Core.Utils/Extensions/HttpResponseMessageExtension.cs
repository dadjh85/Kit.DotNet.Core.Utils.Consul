using Kit.DotNet.Core.Utils.Models;
using System.Net.Http;
using System.Threading.Tasks;

namespace Kit.DotNet.Core.Utils.Extensions
{
    public static class HttpResponseMessageExtension
    {
        /// <summary>
        /// Process an HttpResponseMessage object to return a Response<TEntity> object where TEntity is a typed response object.
        /// </summary>
        /// <typeparam name="TEntity">a generic object</typeparam>
        /// <param name="response">un objeto de tipo HttpResponseMessage</param>
        /// <returns>un objeto de tipo Response<TEntity></returns>
        public static async Task<Response<TEntity>> ProcessResponse<TEntity>(this HttpResponseMessage response) where TEntity : class
            => new Response<TEntity>
            {
                HttpResponseMessage = response,
                Entity = JsonExtension.DeSerializeObject<TEntity>(await response.Content.ReadAsStringAsync())
            };
    };
}
