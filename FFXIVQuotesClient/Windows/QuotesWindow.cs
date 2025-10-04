using System;
using System.Collections.Generic;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;
using Dalamud.Interface.Utility.Raii;
using FFXIVQuotesClient.Http.Responses;

namespace FFXIVQuotesClient.Windows;

public class QuotesWindow : Window, IDisposable
{
    private readonly Configuration configuration;
    private int maxQuotesValue = 5;
    private Plugin plugin;
    public List<QuoteItem> RetrievedQuotes = [];
    public QuotesWindow(Plugin plugin) : base("Retrieve Quotes")
    {
        this.plugin = plugin;
        Flags = ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
                ImGuiWindowFlags.NoScrollWithMouse;

        Size = new Vector2(1280, 480);
        SizeCondition = ImGuiCond.Always;

        configuration = plugin.Configuration;
    }

    public void Dispose() {}

    public override void PreDraw()
    {
        Flags &= ~ImGuiWindowFlags.NoMove;
    }
    
    public override void Draw()
    {
        int maxQuotesInput = maxQuotesValue;
        
        ImGui.TextUnformatted("Quotes to Retrieve:");
        if (ImGui.InputInt("##MaxCount", ref maxQuotesInput))
        {
            maxQuotesValue = maxQuotesInput;
        }
        if (ImGui.Button("Retrieve"))
        {
            Plugin.Log.Information("Retrieving quotes");
            plugin.InvokeOnQuoteRetrieved(1, maxQuotesValue);
        }
        ImGui.TextUnformatted("Results:");
        using var table = ImRaii.Table("Quotes", 3, ImGuiTableFlags.Sortable | ImGuiTableFlags.RowBg | ImGuiTableFlags.ScrollY | ImGuiTableFlags.SizingFixedFit,
                                       new Vector2(0, 300));
        
        ImGui.TableSetupColumn("Quote");
        ImGui.TableSetupColumn("Author");
        ImGui.TableSetupColumn("Date");
        ImGui.TableSetupScrollFreeze(0, 1);
        ImGui.TableHeadersRow();
        //TODO: sort table here
        foreach(var quote in RetrievedQuotes) //TODO: actually iterate some quotes
        {
            using var text2 = ImRaii.PushColor(ImGuiCol.Text, new Vector4(1, 1, 1, 1));
            ImGui.TableNextColumn();
            ImGui.TextUnformatted(quote.QuoteText);
            ImGui.TableNextColumn();
            ImGui.TextUnformatted(quote.Author);
            ImGui.TableNextColumn();
            ImGui.TextUnformatted(quote.Date);
        }
    }
}
