using System;
using System.Numerics;
using System.Threading.Tasks;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;
using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Utility.Raii;
namespace FFXIVQuotesClient.Windows;

public class QuotesWindow : Window, IDisposable
{
    private readonly Configuration configuration;
    private Task? searchQuotesTask;
    
    // We give this window a constant ID using ###.
    // This allows for labels to be dynamic, like "{FPS Counter}fps###XYZ counter window",
    // and the window ID will always be "###XYZ counter window" for ImGui
    public QuotesWindow(Plugin plugin) : base("Retrieve Quotes")
    {
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
        using var table = ImRaii.Table("Quotes", 3, ImGuiTableFlags.Sortable | ImGuiTableFlags.RowBg | ImGuiTableFlags.ScrollY | ImGuiTableFlags.SizingFixedFit,
                                       new Vector2(0, 300));
        
        ImGui.TableSetupColumn("Quote");
        ImGui.TableSetupColumn("Author");
        ImGui.TableSetupColumn("Date");
        ImGui.TableSetupScrollFreeze(0, 1);
        ImGui.TableHeadersRow();
        //TODO: sort table here
        for (var i = 0; i < 50; i++) //TODO: actually iterate some quotes
        {
            using var text2 = ImRaii.PushColor(ImGuiCol.Text, new Vector4(1, 1, 1, 1));
            ImGui.TableNextColumn();
            ImGui.TextUnformatted("this is a quote");
            ImGui.TableNextColumn();
            ImGui.TextUnformatted("author");
            ImGui.TableNextColumn();
            ImGui.TextUnformatted("2025-09-17");
        }
    }
}
