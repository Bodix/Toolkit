using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Dirty.Http.Response;
using Newtonsoft.Json;

namespace Dirty.Http
{
    /// <summary>
    /// Handy alternative for HttpClient.
    /// </summary>
    public static class AsyncHttpRequest
    {
        // public static async Task<HttpDataResponse<TOutput>> Post<TOutput>(string url, string content, string contentType = "application/json")
        // {
        //     HttpWebRequest request = CreateRequestWithContent(url, "POST", contentType, content);
        //
        //     return await ReadResponse(await SendRequestWithContent(request, content), JsonConvert.DeserializeObject<TOutput>);
        // }

        #region GET

        public static async Task<HttpStringResponse> Get(string url, string contentType = "text/plain")
        {
            return await ReadResponse(await SendRequest(CreateRequest(url, contentType)));
        }

        public static async Task<HttpDataResponse<TOutput>> Get<TOutput>(string url, Func<string, TOutput> converter, string contentType = "application/json")
        {
            return await ReadResponse(await SendRequest(CreateRequest(url, contentType)), converter);
        }

        public static async Task<HttpDataResponse<TOutput>> Get<TOutput>(string url, Func<string, Task<TOutput>> converter, string contentType = "application/json")
        {
            return await ReadResponse(await SendRequest(CreateRequest(url, contentType)), converter);
        }

        public static async Task<HttpObjectResponse<TOutput>> Get<TOutput>(string url, Func<byte[], TOutput> converter, string contentType = "application/octet-stream")
        {
            return await ReadResponse(await SendRequest(CreateRequest(url, contentType)), converter);
        }

        public static async Task<HttpObjectResponse<TOutput>> Get<TOutput>(string url, Func<byte[], Task<TOutput>> converter, string contentType = "application/octet-stream")
        {
            return await ReadResponse(await SendRequest(CreateRequest(url, contentType)), converter);
        }

        public static async Task<HttpDataResponse<TOutput>> GetJson<TOutput>(string url)
        {
            // return await ReadResponse(CreateRequest(url, "application/json"), JsonConvert.DeserializeObject<TOutput>);

            return await ReadResponse(await SendRequest(CreateRequest(url, "application/json")),
                async response => await Task.Run(() => JsonConvert.DeserializeObject<TOutput>(response)));
        }

        public static async Task<HttpByteArrayResponse> GetByteArray(string url)
        {
            return await ReadByteArrayResponse(await SendRequest(CreateRequest(url, "application/octet-stream")));
        }
        
        #endregion
        
        private static HttpWebRequest CreateRequest(string url, string contentType)
        {
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
            request.ContentType = contentType;
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            return request;
        }
        
        private static HttpWebRequest CreateRequestWithContent(string url, string method, string contentType, string content)
        {
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
            request.Method = method;
            request.ContentType = contentType;
            request.ContentLength = content.Length;

            return request;
        }
        
        private static async Task<HttpWebResponse> SendRequest(HttpWebRequest request)
        {
            // using (HttpWebResponse response = (HttpWebResponse) await request.GetResponseAsync())
            //     return response;
            return (HttpWebResponse) await request.GetResponseAsync();
        }

        private static async Task<HttpWebResponse> SendRequestWithContent(HttpWebRequest request, string content)
        {
            using (StreamWriter writer = new StreamWriter(await request.GetRequestStreamAsync(), Encoding.ASCII))
                await writer.WriteAsync(content);

            return await SendRequest(request);
        }

        #region Response

        private static async Task<HttpStringResponse> ReadResponse(HttpWebResponse response)
        {
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return new HttpStringResponse(response, await reader.ReadToEndAsync());
            }
        }

        private static async Task<HttpDataResponse<TOutput>> ReadResponse<TOutput>(HttpWebResponse response, Func<string, TOutput> converter)
        {
            return new HttpDataResponse<TOutput>(response, converter.Invoke(await ReadResponse(response)));
        }

        private static async Task<HttpDataResponse<TOutput>> ReadResponse<TOutput>(HttpWebResponse response, Func<string, Task<TOutput>> converter)
        {
            return new HttpDataResponse<TOutput>(response, await converter.Invoke(await ReadResponse(response)));
        }

        private static async Task<HttpByteArrayResponse> ReadByteArrayResponse(HttpWebResponse response)
        {
            using (Stream stream = response.GetResponseStream())
            using (MemoryStream memoryStream = new MemoryStream())
            {
                await stream.CopyToAsync(memoryStream);

                return new HttpByteArrayResponse(response, memoryStream.ToArray());
            }
        }

        private static async Task<HttpObjectResponse<TOutput>> ReadResponse<TOutput>(HttpWebResponse response, Func<byte[], TOutput> converter)
        {
            return new HttpObjectResponse<TOutput>(response, converter.Invoke(await ReadByteArrayResponse(response)));
        }
        
        private static async Task<HttpObjectResponse<TOutput>> ReadResponse<TOutput>(HttpWebResponse response, Func<byte[], Task<TOutput>> converter)
        {
            return new HttpObjectResponse<TOutput>(response, await converter.Invoke(await ReadByteArrayResponse(response)));
        }

        #endregion
    }
}