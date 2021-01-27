using System.Net;
using Dirty.Http.Response.Data;

namespace Dirty.Http.Response
{
    public readonly struct HttpByteArrayResponse : IHttpResponseData
    {
        private readonly IHttpResponseData httpResponseData;
        
        public HttpStatusCode StatusCode => httpResponseData.StatusCode;
        public byte[] Bytes { get; }
        
        public HttpByteArrayResponse(HttpWebResponse response, byte[] bytes)
        {
            httpResponseData = new HttpResponseData(response);

            Bytes = bytes;
        }
        
        public static implicit operator byte[] (HttpByteArrayResponse response)
        {
            return response.Bytes;
        }
    }
}