using Kit.DotNet.Core.Utils.Constants;
using Kit.DotNet.Core.Utils.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Kit.DotNet.Core.Utils.Extensions
{
    public static class HttpClientExtension
    {

        /// <summary>
        /// Call a method GET To API-REST and return a object TEntity.
        /// </summary>
        /// <typeparam name="TEntity">Object to convert json response</typeparam>
        /// <param name="url">url of petition</param>
        /// <param name="token">access token</param>
        /// <returns>a object TEntity</returns>
        public static async Task<Response<TEntity>> GetAsync<TEntity>(this HttpClient client, string url, string token = null) where TEntity : class
        {
            ComposeToken(client, token);

            HttpResponseMessage response = await client.GetAsync(url);

            string content = await response.Content.ReadAsStringAsync();

            return new Response<TEntity>
            {
                HttpResponseMessage = response,
                Entity = JsonExtension.DeSerializeObject<TEntity>(content)
            };
        }


        public static async Task<Response<TEntity>> PostAsync<TEntity>(this HttpClient client, RequestParameters parameters) where TEntity : class
        {
            ComposeToken(client, parameters.Token);

            //TODO: To implement
            throw new NotImplementedException();
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

        #endregion
    }
}
