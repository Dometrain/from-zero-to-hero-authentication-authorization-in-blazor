
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

using System.Net.Http;
using System.Net;

namespace EntraBlazorWebApp;

public class LocalApiCallsHttpHandler(IHttpContextAccessor httpContextAccessor) : HttpClientHandler
{

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var httpContext = httpContextAccessor.HttpContext ??
        throw new InvalidOperationException("No HttpContext available from the IHttpContextAccessor!");

        var cookies = httpContext.Request.Cookies;
        foreach (var cookie in cookies)
        {
            request.Headers.Add("Cookie", new CookieHeaderValue(cookie.Key, cookie.Value).ToString());
        }
       
        return base.SendAsync(request, cancellationToken);
    }
}
