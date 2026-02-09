using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Sample.Service2.Options;
using Sample.Service2.Interfaces;
using System.Web;
using System.Net.Http.Headers;

namespace Sample.Service2.Services;

public class MyHttpClient: IMyHttpClient
{
    private readonly AzureOption _option;
    private readonly IConfidentialClientApplication _app;
    private readonly string[] _scopes;
    private readonly HttpClient _http;
    private readonly ILogger<MyHttpClient> _logger;

    public MyHttpClient(IOptions<AzureOption> option, HttpClient http, ILogger<MyHttpClient> logger)
    {
        // Injection of dependencies
        _option = option.Value;
        _http = http;
        _logger = logger;

        // Initialize the confidential client application and scopes
        _scopes = new string[] { _option.Scope ?? string.Empty };
        _app = ConfidentialClientApplicationBuilder
            .Create(_option.ClientId ?? string.Empty)
            .WithClientSecret(_option.ClientSecret ?? string.Empty)
            .WithAuthority(new Uri(_option.Authority ?? string.Empty))
            .Build();
    }

    public async Task<AuthenticationHeaderValue> GetAuthenticationHeaderAsync()
    {
        AuthenticationResult result = await _app
            .AcquireTokenForClient(_scopes)
            .ExecuteAsync();
        return new AuthenticationHeaderValue("Bearer", result.AccessToken);
    }

    public async Task GetSample(Dictionary<string, string> queryString)
    {
        string qs = "?" + string.Join("&", queryString.Select(
            kvp => $"{HttpUtility.UrlEncode(kvp.Key)}={HttpUtility.UrlEncode(kvp.Value)}"));
        string relativeURL = "sample" + qs;

        using (var request = new HttpRequestMessage(HttpMethod.Get, relativeURL))
        {
            request.Headers.Authorization = await GetAuthenticationHeaderAsync();
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            using (var response = await _http.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Response: {responseBody}", responseBody);
            }
        }
    }
    
    public async Task PostSample()
    {
        string relativeURL = "sample";
        using (var request = new HttpRequestMessage(HttpMethod.Post, relativeURL))
        {
            request.Headers.Authorization = await GetAuthenticationHeaderAsync();
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Content = new StringContent(
                content: """{ "sampleKey": "sampleValue" }""",
                encoding: System.Text.Encoding.UTF8,
                mediaType: "application/json"
            );
            using (var response = await _http.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Response: {responseBody}", responseBody);
            }
        }
    }
}
