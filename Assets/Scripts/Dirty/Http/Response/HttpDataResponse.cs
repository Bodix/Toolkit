using System.Net;
using Dirty.Http.Response.Data;

namespace Dirty.Http.Response
{
    public readonly struct HttpDataResponse<T> : IHttpResponseData
    {
        private readonly IHttpResponseData httpResponseData;
        
        public HttpStatusCode StatusCode => httpResponseData.StatusCode;
        public T Data { get; }
        
        public HttpDataResponse(HttpWebResponse response, T data)
        {
            httpResponseData = new HttpResponseData(response);

            Data = data;
        }
        
        public static implicit operator T (HttpDataResponse<T> response)
        {
            return response.Data;
        }
    }
}