using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ShcherbakUnityAwaiter
{
    public static class HttpRequest
    {
        /// <summary>
        /// Base HTTP POST. Although can be used for PUT, PATCH, DELETE
        /// </summary>
        public static async Task<TOutput> Post<InputType, TOutput>(string url, InputType requestModel,
            string contentType = "application/json", string method = "POST")
        {
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
            request.Method = method;
            request.ContentType = contentType;
            string requestString = JsonConvert.SerializeObject(requestModel);
            request.ContentLength = requestString.Length;

            var requestedStream = await request.GetRequestStreamAsync();
            StreamWriter requestWriter = new StreamWriter(requestedStream, System.Text.Encoding.ASCII);
            await requestWriter.WriteAsync(requestString);
            requestWriter.Close();

            WebResponse webResponse = request.GetResponse();
            TOutput deserializedObject = default;
            using (
                Stream webStream = webResponse.GetResponseStream())
            {
                using (StreamReader responseReader = new StreamReader(webStream))
                {
                    string jsonString = await responseReader.ReadToEndAsync();

                    deserializedObject = JsonConvert.DeserializeObject<TOutput>(jsonString);
                }
            }

            return deserializedObject;
        }

        /// <summary>
        ///  Base HTTP GET.
        /// </summary>
        public static async Task<TOutput> Get<TOutput>(string url, string contentType = "application/json")
        {
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
            request.ContentType = contentType;
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            
            TOutput deserializedObject;

            using (HttpWebResponse response = (HttpWebResponse) await request.GetResponseAsync())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                string jsonString = await reader.ReadToEndAsync();

                deserializedObject = JsonConvert.DeserializeObject<TOutput>(jsonString);
            }

            return deserializedObject;
        }
    }
}