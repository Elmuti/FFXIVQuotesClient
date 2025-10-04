using System;
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
    private string baseUrl = "";
    private string apiToken = "";
    public string BaseUrl
    {
        get => baseUrl;
        set
        {
            if (!string.IsNullOrEmpty(value))
            {
                try
                {
                    httpClient.BaseAddress = new Uri(value);
                    baseUrl = value;
                }
                catch (UriFormatException e)
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
            apiToken  = value;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiToken);
        }
    }

    public QuotesApiClient(Configuration configuration)
    {
        BaseUrl = configuration.BaseUrl;
        ApiToken = configuration.ApiToken;
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<QuotesResponse> Search(int userId, int maxQuotes = 5)
    {
        var response = await httpClient.GetAsync($"quotes/search/user/{userId}?max_quotes={maxQuotes}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<QuotesResponse>() ?? throw new InvalidOperationException();
    }
    
    public async Task<bool> Store(Quote quote)
    {
        using StringContent jsonContent = new(JsonSerializer.Serialize(quote), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync($"quotes", jsonContent);
        return response.IsSuccessStatusCode;
    }
}
