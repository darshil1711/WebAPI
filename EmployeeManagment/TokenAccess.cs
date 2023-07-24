using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

public class TokenAccess : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TokenAccess(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>Sends an HTTP request to the inner handler to send to the server as an asynchronous operation.</summary>
    /// <param name="request">The HTTP request message to send to the server.</param>
    /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = _httpContextAccessor.HttpContext.Session.GetString("JWToken");
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", JObject.Parse(token)["token"].ToString());
        }

        return await base.SendAsync(request, cancellationToken);
    }
}