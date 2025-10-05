using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FFXIVQuotesClient.Http.Json;
using FFXIVQuotesClient.Http.Responses;

namespace FFXIVQuotesClient.Http;

public class QuotesApiClient
{
    private readonly HttpClient httpClient = new();
    private string baseUrl = "http://localhost/api/v1/";
    private string apiToken = "";
    public string BaseUrl
    {
        get => baseUrl;
        set
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (IsValidUri(value))
                {
                    var validated = value.EndsWith('/') ? value : value + '/';
                    baseUrl = validated;
                    Plugin.Log.Information($"BaseUrl set to '{validated}'");
                }
                else
                {
                    Plugin.Log.Warning($"Api BaseUrl format '{value}' is invalid");
                }
            }
            else
            {
                Plugin.Log.Warning("Api BaseUrl is empty or null");
            }
        }
    }
    
    public string ApiToken
    {
        get => apiToken;
        set
        {
            if (!string.IsNullOrEmpty(value))
            {
                apiToken  = value;
                Plugin.Log.Information($"ApiToken set to '{string.Concat(Enumerable.Repeat('*', value.Length))}'");
            }
            else
            {
                Plugin.Log.Warning("Api ApiToken is empty or null");
            }
        }
    }

    public QuotesApiClient(Configuration configuration)
    {
        BaseUrl = configuration.BaseUrl;
        ApiToken = configuration.ApiToken;
        httpClient.Timeout = TimeSpan.FromSeconds(5);
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }
    
    public bool IsValidUri(string uri)
    {
        return Uri.TryCreate(uri, UriKind.RelativeOrAbsolute, out _);
    }
    
    public async Task<QuotesResponse> Search(int userId, int maxQuotes = 5)
    {
        try
        {
            var request = CreateRequest(HttpMethod.Get, $"quotes/search/user/{userId}?max_quotes={maxQuotes}");
            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<QuotesResponse>() ?? throw new InvalidOperationException();
        }
        catch (HttpRequestException e)
        {
            Plugin.Log.Error($"[QuotesApiClient.Search] {e.StatusCode} {e.HttpRequestError}");
        }
        catch (Exception e)
        {
            Plugin.Log.Error($"[QuotesApiClient.Search] {e.Message}");
        }

        return new QuotesResponse();
    }
    
    public async Task<bool> Store(Quote quote)
    {
        try
        {
            using StringContent jsonContent = new(JsonSerializer.Serialize(quote), Encoding.UTF8, "application/json");
            var request = CreateRequest(HttpMethod.Post, "quotes", jsonContent);
            var response = await httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException e)
        {
            Plugin.Log.Error($"[QuotesApiClient.Store] {e.StatusCode} {e.HttpRequestError}");
        }
        catch (Exception e)
        {
            Plugin.Log.Error($"[QuotesApiClient.Store] {e.Message}");
        }

        return false;
    }
    
    private HttpRequestMessage CreateRequest(HttpMethod method, string endpoint, HttpContent? content = null)
    {
        var fullUrl = new Uri(new Uri(baseUrl), endpoint);
        var request = new HttpRequestMessage(method, fullUrl);
        
        if (!string.IsNullOrEmpty(apiToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiToken);
        }
        
        if (content != null)
        {
            request.Content = content;
        }
        
        return request;
    }
}
