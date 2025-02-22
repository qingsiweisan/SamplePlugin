using System;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;

namespace MarketBoardAutoBuyer
{
    public sealed class Plugin : IDalamudPlugin
    {
        public string Name => "MarketBoard AutoBuyer";

        [PluginService] internal static IPluginLog Log { get; private set; } = null!;
        [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;
        [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
        [PluginService] internal static IGameInventory GameInventory { get; private set; } = null!;



        private readonly WindowSystem windowSystem;
        private readonly MarketBoardUI marketBoardUI;

        public Plugin()
        {
            this.windowSystem = new WindowSystem("MarketBoardAutoBuyer");
            this.marketBoardUI = new MarketBoardUI();
            this.windowSystem.AddWindow(this.marketBoardUI);

            PluginInterface.UiBuilder.OpenMainUi += ToggleMainUI;
            PluginInterface.UiBuilder.Draw += DrawUI;

            marketBoardUI.IsOpen = true;

            Log.Information("MarketBoard AutoBuyer 插件已加载");

        }

        public void Dispose()
        {
            PluginInterface.UiBuilder.OpenMainUi -= ToggleMainUI;
            PluginInterface.UiBuilder.Draw -= DrawUI;
            this.windowSystem.RemoveAllWindows();
            Log.Information("MarketBoard AutoBuyer 插件已卸载");
        }

        private void DrawUI() => this.windowSystem.Draw();

        public void ToggleMainUI() => marketBoardUI.IsOpen = !marketBoardUI.IsOpen;
    }
}
