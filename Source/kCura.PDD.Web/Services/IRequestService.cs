namespace kCura.PDD.Web.Services
{
    using System.Collections.Generic;
    using System.Net.Http;

    public interface IRequestService
    {
        List<KeyValuePair<string, string>> GetQueryParams(HttpRequestMessage request);

        List<KeyValuePair<string, string>> GetQueryParamsDecoded(HttpRequestMessage request);
    }
}
