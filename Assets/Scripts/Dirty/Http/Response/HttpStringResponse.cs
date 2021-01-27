using System.Net;
using Dirty.Http.Response.Data;

namespace Dirty.Http.Response
{
    public readonly struct HttpStringResponse : IHttpResponseData
    {
        private readonly IHttpResponseData httpResponseData;
        
        public HttpStatusCode StatusCode => httpResponseData.StatusCode;
        public string Message { get; }

        public HttpStringResponse(HttpWebResponse response, string message)
        {
            httpResponseData = new HttpResponseData(response);
            
            Message = message;
        }
        
        public static implicit operator string (HttpStringResponse response)
        {
            return response.Message;
        }
    }
}