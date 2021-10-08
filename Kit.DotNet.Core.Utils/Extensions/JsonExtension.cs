
using Newtonsoft.Json;

namespace Kit.DotNet.Core.Utils.Extensions
{
    /// <summary>
    /// JsonExtension's static class.
    /// </summary>
    public static class JsonExtension
    {
        /// <summary>
        /// Process object Json.
        /// </summary>
        /// <typeparam name="TEntity">TEntity param type</typeparam>
        /// <param name="jsonSerialize">TEntity object type</param>
        /// <returns>String object type</returns>
        public static string SerializeObject<TEntity>(TEntity jsonSerialize)
            where TEntity : class
        {
            string result = null;

            if (jsonSerialize != null)
            {
                result = JsonConvert.SerializeObject(jsonSerialize);
            }

            return result;
        }


        /// <summary>
        /// Process object Json without nulls of object.
        /// </summary>
        /// <typeparam name="TEntity">TEntity param type</typeparam>
        /// <param name="jsonSerialize">TEntity object type</param>
        /// <returns>String object type</returns>
        public static string SerializeObjectWithoutNulls<TEntity>(TEntity jsonSerialize)
            where TEntity : class
            =>  JsonConvert.SerializeObject(jsonSerialize,
                                            Formatting.None,
                                            new JsonSerializerSettings
                                            {
                                                NullValueHandling = NullValueHandling.Ignore
                                            });

        /// <summary>
        /// Process object Json.
        /// </summary>
        /// <param name="value">string param type</param>
        /// <param name="value">String object type</param>
        /// <returns>TEntity object type</returns>
        public static TEntity DeSerializeObject<TEntity>(string value) where TEntity : class
        {
            TEntity result;

            try
            {
                result = JsonConvert.DeserializeObject<TEntity>(value);
            }
            catch
            {
                return null;
            }

            return result;
        }
    }
}
