using System.Net;

namespace Dirty.Http.Response.Data
{
    public readonly struct HttpResponseData : IHttpResponseData
    {
        public HttpResponseData(HttpWebResponse response)
        {
            StatusCode = response.StatusCode;

            response.Dispose();
        }

        public HttpStatusCode StatusCode { get; }
    }
}