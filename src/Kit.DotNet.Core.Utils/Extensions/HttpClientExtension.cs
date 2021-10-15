using Kit.DotNet.Core.Utils.Constants;
using Kit.DotNet.Core.Utils.Models;
using System.Net.Http;
using System.Threading.Tasks;

namespace Kit.DotNet.Core.Utils.Extensions
{
    /// <summary>
    /// Extension that simplifies HTTP calls
    /// Allows processing of response json to return a typed object
    /// </summary>
    public static class HttpClientExtension
    {

        /// <summary>
        /// Call a method GET To API-REST and return a object TEntity.
        /// </summary>
        /// <typeparam name="TEntity">Object to convert json response</typeparam>
        /// <param name="parameters">an object of type RequestParameters</param>
        /// <returns>a object TEntity</returns>
        public static async Task<Response<TEntity>> GetAsync<TEntity>(this HttpClient client, RequestParameters parameters) where TEntity : class
        {
            ComposeToken(client, parameters.Token);

            HttpResponseMessage response = await client.GetAsync(parameters.Url);

            return await response.ProcessResponse<TEntity>();
        }

        /// <summary>
        /// Call a method POST To API-REST and return a object TEntity.
        /// </summary>
        /// <typeparam name="TEntity">Object to convert json response</typeparam>
        /// <param name="client">an object of type RequestParameters</param>
        /// <param name="parameters">an object of type RequestParameters</param>
        /// <returns>a object TEntity</returns>

        public static async Task<Response<TEntity>> PostAsync<TEntity>(this HttpClient client, RequestParameters parameters) where TEntity : class
        {
            ComposeToken(client, parameters.Token);

            LoadRequestHeaders(client, parameters);

            HttpResponseMessage response = await client.PostAsync(parameters.Url, parameters.HttpContent);

            return await response.ProcessResponse<TEntity>();
        }

        /// <summary>
        /// Call a method PUT To API-REST and return a object TEntity.
        /// </summary>
        /// <typeparam name="TEntity">Object to convert json response</typeparam>
        /// <param name="client">an object of type RequestParameters</param>
        /// <param name="parameters">an object of type RequestParameters</param>
        /// <returns>a object TEntity</returns>
        public static async Task<Response<TEntity>> PutAsync<TEntity>(this HttpClient client, RequestParameters parameters) where TEntity : class
        {
            ComposeToken(client, parameters.Token);

            LoadRequestHeaders(client, parameters);

            HttpResponseMessage response = await client.PutAsync(parameters.Url, parameters.HttpContent);

            return await response.ProcessResponse<TEntity>();
        }

        /// <summary>
        /// Call a method PATCH To API-REST and return a object TEntity.
        /// </summary>
        /// <typeparam name="TEntity">Object to convert json response</typeparam>
        /// <param name="client">an object of type RequestParameters</param>
        /// <param name="parameters">an object of type RequestParameters</param>
        /// <returns>a object TEntity</returns>
        public static async Task<Response<TEntity>> PatchAsync<TEntity>(this HttpClient client, RequestParameters parameters) where TEntity : class
        {
            ComposeToken(client, parameters.Token);

            LoadRequestHeaders(client, parameters);

            HttpResponseMessage response = await client.PatchAsync(parameters.Url, parameters.HttpContent);

            return await response.ProcessResponse<TEntity>();
        }

        /// <summary>
        /// Call a method DELETE To API-REST and return a object TEntity.
        /// </summary>
        /// <typeparam name="TEntity">Object to convert json response</typeparam>
        /// <param name="parameters">an object of type RequestParameters</param>
        /// <returns>a object TEntity</returns>
        public static async Task<Response<TEntity>> DeleteAsync<TEntity>(this HttpClient client, RequestParameters parameters) where TEntity : class
        {
            ComposeToken(client, parameters.Token);

            HttpResponseMessage response = await client.DeleteAsync(parameters.Url);

            return await response.ProcessResponse<TEntity>();
        }

        #region Private methods


        /// <summary>
        /// Compose the token with Bearer if not contain this word.
        /// </summary>
        /// <param name="token">the Bearer token of the request</param>
        private static void ComposeToken(HttpClient client, string token)
        {

            if (token != null)
            {
                if (client.DefaultRequestHeaders.Authorization != null)
                {
                    client.DefaultRequestHeaders.Remove(ConstantsSecurity.AUTH_ATTRIBUTE);
                }

                client.DefaultRequestHeaders.Add(ConstantsSecurity.AUTH_ATTRIBUTE, token.Contains(ConstantsSecurity.START_AUTHORIZATION) ? token : ConstantsSecurity.START_AUTHORIZATION + token);
            }
        }

        /// <summary>
        /// Lod a list of headers
        /// </summary>
        /// <param name="client">an object of type HttpClient</param>
        /// <param name="parameters">an object of type RequestParameters</param>
        private static void LoadRequestHeaders(HttpClient client, RequestParameters parameters)
        {
            if (parameters != null && parameters.RequestHeaders != null)
            {
                foreach (RequestHeader item in parameters.RequestHeaders)
                {
                    client.DefaultRequestHeaders.Add(item.Header, item.Value);
                }
            }
        }

        #endregion
    }
}
