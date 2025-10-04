namespace FFXIVQuotesClient.HTTP.JSON;
using System.Text.Json.Serialization;

public class Quote
{
    [JsonPropertyName("user_id")]
    public int UserId { get; set; }

    [JsonPropertyName("quote")]
    public string QuoteText { get; set; }

    [JsonPropertyName("author")]
    public string Author { get; set; }

    [JsonPropertyName("date")]
    public string Date { get; set; }
}
