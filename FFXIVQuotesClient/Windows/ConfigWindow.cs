using System;
using System.Linq;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;

namespace FFXIVQuotesClient.Windows;

public class ConfigWindow : Window, IDisposable
{
    private readonly Plugin plugin;
    private readonly Configuration configuration;
    private string? baseUrl;
    private string? apiToken;
    public ConfigWindow(Plugin plugin) : base("Settings for quotes.elmu.dev###ConfigWindow")
    {
        this.plugin = plugin;
        Flags = ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
                ImGuiWindowFlags.NoScrollWithMouse;

        Size = new Vector2(300, 180);
        SizeCondition = ImGuiCond.Always;

        configuration = plugin.Configuration;
        baseUrl = !string.IsNullOrEmpty(configuration.BaseUrl) ? configuration.BaseUrl : baseUrl;
        apiToken = !string.IsNullOrEmpty(configuration.ApiToken) ? configuration.ApiToken : apiToken;
    }

    public void Dispose() { }

    public override void PreDraw()
    {
        Flags &= ~ImGuiWindowFlags.NoMove;
    }

    public override void Draw()
    {
        var baseUrlInput = baseUrl ?? "";
        var tokenInput = apiToken ?? "";
        ImGui.TextUnformatted("Api Base Url:");
        if (ImGui.InputText("##BaseUrl", ref baseUrlInput, 256, ImGuiInputTextFlags.None))
        {
            baseUrl = baseUrlInput;
        }
        ImGui.TextUnformatted("Api Token:");
        if (ImGui.InputText("##ApiToken", ref tokenInput, 256, ImGuiInputTextFlags.Password))
        {
            apiToken = tokenInput;
        }
        if (ImGui.Button("Save"))
        {
            plugin.InvokeOnSettingsSaved(baseUrl!, apiToken!);
        }
    }
}
