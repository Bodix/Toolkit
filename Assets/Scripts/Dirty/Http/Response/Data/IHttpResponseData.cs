using System.Net;

namespace Dirty.Http.Response.Data
{
    public interface IHttpResponseData
    {
        HttpStatusCode StatusCode { get; }
    }
}