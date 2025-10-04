using System;
using System.Numerics;
using System.Threading.Tasks;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;
using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Utility.Raii;
namespace FFXIVQuotesClient.Windows;

public class CreateQuoteWindow : Window, IDisposable
{
    private readonly Configuration configuration;
    private string? quoteText;
    private string? authorText;
    private Plugin plugin;
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
        if (ImGui.InputText("##Quote", ref quoteInput, 512, ImGuiInputTextFlags.None))
        {
            quoteText = quoteInput;
        }
        ImGui.TextUnformatted("Author:");
        if (ImGui.InputText("##Author", ref authorInput, 64, ImGuiInputTextFlags.None))
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
