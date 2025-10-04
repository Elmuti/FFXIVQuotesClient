using System;
using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using FFXIVQuotesClient.Http;
using FFXIVQuotesClient.Http.Json;
using FFXIVQuotesClient.Http.Responses;
using FFXIVQuotesClient.Windows;

namespace FFXIVQuotesClient;

public sealed class Plugin : IDalamudPlugin
{
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;
    [PluginService] internal static IPluginLog Log { get; private set; } = null!;

    private const string CommandName = "/quotes";
    private ConfigWindow ConfigWindow { get; init; }
    private MainWindow MainWindow { get; init; }
    private QuotesWindow QuotesWindow { get; init; }
    private CreateQuoteWindow CreateQuoteWindow { get; init; }
    private QuotesApiClient ApiClient { get; init; }
    public event Action<string, string> OnSettingsSaved;
    public event Action<string, string> OnQuoteCreated;
    public event Action<int, int> OnQuoteRetrieved;

    public Configuration Configuration { get; init; }

    public readonly WindowSystem WindowSystem = new("FFXIVQuotesClient");
    
    private void OnCommand(string command, string args)
    {
        MainWindow.Toggle();
    }

    public void InvokeOnQuoteRetrieved(int userId, int maxQuotes)
    {
        OnQuoteRetrieved.Invoke(userId, maxQuotes);
    }
    
    public void InvokeOnSettingsSaved(string baseUrl, string apiToken)
    {
        OnSettingsSaved.Invoke(baseUrl, apiToken);
    }
    
    public void InvokeOnQuoteCreated(string author, string quote)
    {
        OnQuoteCreated.Invoke(author, quote);
    }
    
    public Plugin()
    {
        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        ApiClient = new(Configuration);
        ApiClient.BaseUrl = Configuration.BaseUrl;
        ApiClient.ApiToken = Configuration.ApiToken;
        ConfigWindow = new ConfigWindow(this);
        MainWindow = new MainWindow(this);
        QuotesWindow = new QuotesWindow(this);
        CreateQuoteWindow = new CreateQuoteWindow(this);
        WindowSystem.AddWindow(ConfigWindow);
        WindowSystem.AddWindow(MainWindow);
        WindowSystem.AddWindow(QuotesWindow);
        WindowSystem.AddWindow(CreateQuoteWindow);
        CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "Open FFXIVQuotesClient UI"
        });
        
        PluginInterface.UiBuilder.Draw += WindowSystem.Draw;
        PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUi;
        PluginInterface.UiBuilder.OpenMainUi += ToggleMainUi;

        OnSettingsSaved += (baseUrl, apiToken) =>
        {
            Configuration.BaseUrl = baseUrl;
            Configuration.ApiToken = apiToken;
            ApiClient.BaseUrl = baseUrl;
            ApiClient.ApiToken = apiToken;
            Configuration.Save();
        };

        OnQuoteCreated += async (author, quote) =>
        {
            await ApiClient.Store(new Quote
            {
                UserId = 1,
                QuoteText = quote,
                Author = author,
                Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            });
        };
        
        OnQuoteRetrieved += async (userId, maxQuotes) =>
        {
            var response = await ApiClient.Search(userId, maxQuotes);
            QuotesWindow.RetrievedQuotes.Clear();
            response.Quotes.ForEach((quote => QuotesWindow.RetrievedQuotes.Add(quote)));
        };
        
        Log.Information($"{PluginInterface.Manifest.Name} initialized");
    }

    public void Dispose()
    {
        PluginInterface.UiBuilder.Draw -= WindowSystem.Draw;
        PluginInterface.UiBuilder.OpenConfigUi -= ToggleConfigUi;
        PluginInterface.UiBuilder.OpenMainUi -= ToggleMainUi;
        WindowSystem.RemoveAllWindows();
        Configuration.Save();
        
        ConfigWindow.Dispose();
        MainWindow.Dispose();
        QuotesWindow.Dispose();
        CreateQuoteWindow.Dispose();
        CommandManager.RemoveHandler(CommandName);
    }
    
    
    public void ToggleCreateQuoteUi() => CreateQuoteWindow.Toggle();
    public void ToggleQuotesUi() => QuotesWindow.Toggle();
    public void ToggleConfigUi() => ConfigWindow.Toggle();
    public void ToggleMainUi() => MainWindow.Toggle();
}
