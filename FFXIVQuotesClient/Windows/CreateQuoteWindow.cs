using System;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;
namespace FFXIVQuotesClient.Windows;

public class CreateQuoteWindow : Window, IDisposable
{
    private readonly Configuration configuration;
    private string? quoteText;
    private string? authorText;
    private readonly Plugin plugin;
    public CreateQuoteWindow(Plugin plugin) : base("Create Quotes")
    {
        this.plugin = plugin;
        Flags = ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
                ImGuiWindowFlags.NoScrollWithMouse;

        Size = new Vector2(720, 360);
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
        var quoteInput = quoteText ?? "";
        var authorInput = authorText ?? "";
        
        ImGui.TextUnformatted("Quote:");
        if (ImGui.InputText("##Quote", ref quoteInput))
        {
            quoteText = quoteInput;
        }
        ImGui.TextUnformatted("Author:");
        if (ImGui.InputText("##Author", ref authorInput, 64))
        {
            authorText = authorInput;
        }
        if (ImGui.Button("Create Quote"))
        {
            Plugin.Log.Information($"\"{quoteText}\" -{authorText}");
            plugin.InvokeOnQuoteCreated(authorText!, quoteText!);
        }
    }
}
