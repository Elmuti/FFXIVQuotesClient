namespace FFXIVQuotesClient.HTTP.Responses;

using System.Text.Json.Serialization;
using System.Collections.Generic;

public class QuotesResponse
{
    [JsonPropertyName("quotes")]
    public List<QuoteItem> Quotes { get; set; }
}

public class QuoteItem
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("quote")]
    public string QuoteText { get; set; }

    [JsonPropertyName("author")]
    public string Author { get; set; }

    [JsonPropertyName("date")]
    public string Date { get; set; }
}
