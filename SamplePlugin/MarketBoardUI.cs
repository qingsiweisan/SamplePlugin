using System;
using System.Threading.Tasks;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using ImGuiNET;
using System.Linq;
using System.Collections.Generic;
using Dalamud.IoC;
using Dalamud.Game.Inventory;
using FFXIVClientStructs.FFXIV.Client.Game;
using System.Linq;
using System.Threading.Tasks;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;

namespace MarketBoardAutoBuyer
{
    internal class MarketBoardUI : Window
    {
        [PluginService] internal static IGameInventory? GameInventory { get; private set; } = null!;

        public MarketBoardUI() : base("MarketBoard AutoBuyer")
        {
            this.SizeConstraints = new WindowSizeConstraints
            {
                MinimumSize = new System.Numerics.Vector2(300, 100),
                MaximumSize = new System.Numerics.Vector2(600, 300)
            };
            this.IsOpen = true;
        }

        public override void Draw()
        {
            ImGui.Text("自动在交易板搜索【陈旧的狞豹革地图】并购买");

            if (ImGui.Button("搜索 & 购买"))
            {
                Task.Run(async () =>
                {
                    await SimulateMarketBoardSearch();
                });
            }

            ImGui.Separator();
            ImGui.Text("自动将物品放入陆行鸟背包");

            if (ImGui.Button("放入陆行鸟背包"))
            {
                if (GameInventory == null)
                {
                    Plugin.Log.Information("GameInventory 尚未初始化，稍后再试！");
                    return;
                }

                Task.Run(async () =>
                {
                    await MoveItemToChocoboBag();
                });
            }
        }

        private async Task SimulateMarketBoardSearch()
        {
            Plugin.Log.Information("开始交易板搜索...");

            Plugin.Log.Information("输入小键盘 080...");
            KeyPressSimulator.PressKey(VirtualKey.NUMPAD0);
            await Task.Delay(100);
            KeyPressSimulator.PressKey(VirtualKey.NUMPAD8);
            await Task.Delay(100);
            KeyPressSimulator.PressKey(VirtualKey.NUMPAD0);
            await Task.Delay(300);

            Plugin.Log.Information("输入 `陈旧的狞豹革地图` ...");
            await KeyPressSimulator.TypeChineseText("陈旧的狞豹革地图");
            await Task.Delay(500);

            KeyPressSimulator.PressKey(VirtualKey.RETURN);
            await Task.Delay(1000);
            Plugin.Log.Information("市场板搜索完成！");

            Plugin.Log.Information("模拟小键盘 0 进行购买...");
            for (int i = 0; i < 4; i++)
            {
                await Task.Delay(250);
                KeyPressSimulator.PressKey(VirtualKey.NUMPAD0);
            }

            Plugin.Log.Information("购买完成！");
        }

        private static unsafe Task MoveItemToChocoboBag()
        {
            var module = RaptureGearsetModule.Instance();
            var manager = InventoryManager.Instance();

            var targetItemId = 43557;  // 陈旧的狞豹革地图的 ID
            var counter = 0;

            // 定义背包类型数组并进行类型转换
            var BagInventories = new FFXIVClientStructs.FFXIV.Client.Game.InventoryType[]
            {
        (FFXIVClientStructs.FFXIV.Client.Game.InventoryType)GameInventoryType.Inventory1,
        (FFXIVClientStructs.FFXIV.Client.Game.InventoryType)GameInventoryType.Inventory2,
        (FFXIVClientStructs.FFXIV.Client.Game.InventoryType)GameInventoryType.Inventory3,
        (FFXIVClientStructs.FFXIV.Client.Game.InventoryType)GameInventoryType.Inventory4
            };

            // 先遍历背包（Inventory1, Inventory2, Inventory3, Inventory4）
            foreach (var inventoryType in BagInventories)
            {
                var container = manager->GetInventoryContainer(inventoryType);
                if (container == null) continue;

                for (var i = 0; i < container->Size; i++)
                {
                    var slot = container->GetInventorySlot(i);
                    if (slot == null || slot->ItemId == 0) continue;

                    // 如果物品是 "陈旧的狞豹革地图"
                    if (slot->ItemId == targetItemId)
                    {
                        // 获取陆行鸟背包的空槽
                        var targetSaddleBag = FFXIVClientStructs.FFXIV.Client.Game.InventoryType.SaddleBag1; // 陆行鸟背包，选择第一面
                        var targetContainer = manager->GetInventoryContainer(targetSaddleBag);

                        if (targetContainer != null)
                        {
                            // 找到目标容器并直接移动物品到陆行鸟背包
                            manager->MoveItemSlot(inventoryType, (ushort)i, targetSaddleBag, (ushort)i, 1);
                            counter++;
                            Plugin.Log.Information($"物品 {slot->ItemId} 已成功移动到陆行鸟背包。");
                        }
                        else
                        {
                            Plugin.Log.Information("陆行鸟背包没有空槽！");
                        }
                        break;
                    }
                }
            }

            // 如果物品被成功移动
            if (counter > 0)
            {
                Plugin.Log.Information("物品已成功移动到陆行鸟背包！");
            }
            else
            {
                Plugin.Log.Information("未找到指定物品！");
            }
            return Task.CompletedTask;

        }
    }

    public enum VirtualKey : ushort
    {
        RETURN = 0x0D,
        NUMPAD0 = 0x60,
        NUMPAD8 = 0x68,
        F10 = 0x79,
        ESCAPE = 0x1B
    }
}
