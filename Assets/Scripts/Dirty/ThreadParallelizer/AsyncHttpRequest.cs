using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Dirty.ThreadParallelizer
{
    public static class AsyncHttpRequest
    {
        public static async Task<TOutput> Post<TInput, TOutput>(string url, TInput requestModel,
            string contentType = "application/json", string method = "POST")
        {
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
            request.Method = method;
            request.ContentType = contentType;
            string requestString = JsonConvert.SerializeObject(requestModel);
            request.ContentLength = requestString.Length;

            using (StreamWriter writer = new StreamWriter(await request.GetRequestStreamAsync(), Encoding.ASCII))
                await writer.WriteAsync(requestString);

            return await ReadResponse(request, JsonConvert.DeserializeObject<TOutput>);
        }

        #region Get

        public static async Task<string> Get(string url, string contentType = "text/plain")
        {
            return await ReadResponse(CreateGetRequest(url, contentType));
        }

        public static async Task<TOutput> Get<TOutput>(string url, Func<string, TOutput> converter, string contentType = "text/plain")
        {
            return await ReadResponse(CreateGetRequest(url, contentType), converter);
        }

        public static async Task<TOutput> GetJson<TOutput>(string url)
        {
            return await ReadResponse(CreateGetRequest(url, "application/json"), JsonConvert.DeserializeObject<TOutput>);
        }

        public static async Task<byte[]> GetByteArray(string url)
        {
            return await ReadByteArrayResponse(CreateGetRequest(url, "application/octet-stream"));
        }

        private static HttpWebRequest CreateGetRequest(string url, string contentType)
        {
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
            request.ContentType = contentType;
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            
            return request;
        }

        #endregion

        #region Response

        private static async Task<string> ReadResponse(HttpWebRequest request)
        {
            using (HttpWebResponse response = (HttpWebResponse) await request.GetResponseAsync())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync();
            }
        }

        private static async Task<TOutput> ReadResponse<TOutput>(HttpWebRequest request, Func<string, TOutput> converter)
        {
            return converter.Invoke(await ReadResponse(request));
        }

        private static async Task<byte[]> ReadByteArrayResponse(HttpWebRequest request)
        {
            using (HttpWebResponse response = (HttpWebResponse) await request.GetResponseAsync())
            using (Stream stream = response.GetResponseStream())
            using (MemoryStream memoryStream = new MemoryStream())
            {
                await stream.CopyToAsync(memoryStream);

                return memoryStream.ToArray();
            }
        }

        #endregion
    }
}