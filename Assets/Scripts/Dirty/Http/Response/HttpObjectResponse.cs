using System.Net;
using Dirty.Http.Response.Data;

namespace Dirty.Http.Response
{
    public readonly struct HttpObjectResponse<T> : IHttpResponseData
    {
        private readonly IHttpResponseData httpResponseData;
        
        public HttpStatusCode StatusCode => httpResponseData.StatusCode;
        public T Object { get; }
        
        public HttpObjectResponse(HttpWebResponse response, T @object)
        {
            httpResponseData = new HttpResponseData(response);

            Object = @object;
        }
        
        public static implicit operator T (HttpObjectResponse<T> response)
        {
            return response.Object;
        }
    }
}