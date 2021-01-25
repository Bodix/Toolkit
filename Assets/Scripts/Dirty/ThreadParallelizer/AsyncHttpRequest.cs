using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Dirty.ThreadParallelizer
{
    public static class AsyncHttpRequest
    {
        public static async Task<TOutput> Post<TInput, TOutput>(string url, string content,
            string contentType = "application/json", string method = "POST")
        {
            HttpWebRequest request = CreateRequestWithContent(url, method, contentType, content);

            // using (StreamWriter writer = new StreamWriter(await request.GetRequestStreamAsync(), Encoding.ASCII))
            //     await writer.WriteAsync(requestString);

            return await ReadResponse(request, JsonConvert.DeserializeObject<TOutput>);
        }

        private static HttpWebRequest CreateRequestWithContent(string url, string method, string contentType, string content)
        {
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
            request.Method = method;
            request.ContentType = contentType;
            request.ContentLength = content.Length;

            return request;
        }

        #region Get

        public static async Task<string> Get(string url, string contentType = "text/plain")
        {
            return await ReadResponse(CreateRequest(url, contentType));
        }

        public static async Task<TOutput> Get<TOutput>(string url, Func<string, TOutput> converter, string contentType = "text/plain")
        {
            return await ReadResponse(CreateRequest(url, contentType), converter);
        }
        
        public static async Task<TOutput> Get<TOutput>(string url, Func<string, Task<TOutput>> converter, string contentType = "text/plain")
        {
            return await ReadResponse(CreateRequest(url, contentType), converter);
        }

        public static async Task<TOutput> GetJson<TOutput>(string url)
        {
            // return await ReadResponse(CreateRequest(url, "application/json"), JsonConvert.DeserializeObject<TOutput>);

            return await ReadResponse(CreateRequest(url, "application/json"),
                async response => await Task.Run(() => JsonConvert.DeserializeObject<TOutput>(response)));
        }

        public static async Task<byte[]> GetByteArray(string url)
        {
            return await ReadByteArrayResponse(CreateRequest(url, "application/octet-stream"));
        }

        private static HttpWebRequest CreateRequest(string url, string contentType)
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
        
        private static async Task<TOutput> ReadResponse<TOutput>(HttpWebRequest request, Func<string, Task<TOutput>> converter)
        {
            return await converter.Invoke(await ReadResponse(request));
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