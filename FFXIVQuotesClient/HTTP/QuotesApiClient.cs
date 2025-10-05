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
    private Uri baseAddress = new("http://localhost/api/v1/");
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
                    baseAddress = new Uri(validated);
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
        RefreshHeaders();
        var response = await httpClient.GetAsync($"quotes/search/user/{userId}?max_quotes={maxQuotes}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<QuotesResponse>() ?? throw new InvalidOperationException();
    }
    
    public async Task<bool> Store(Quote quote)
    {
        RefreshHeaders();
        using StringContent jsonContent = new(JsonSerializer.Serialize(quote), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync($"quotes", jsonContent);
        return response.IsSuccessStatusCode;
    }

    private void RefreshHeaders()
    {
        httpClient.BaseAddress = baseAddress;
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApiToken);
    }
}
