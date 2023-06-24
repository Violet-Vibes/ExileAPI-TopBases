using System.Collections.Concurrent;
using ExileCore;
using ExileCore.PoEMemory.Elements.InventoryElements;
using ExileCore.Shared.Cache;
using SharpDX;
using System.Collections.Generic;
using ExileCore.Shared.Enums;
using System.Linq;
using ImGuiNET;

namespace TopBases;

public class TopBases : BaseSettingsPlugin<TopBasesSettings>
{
    private CachedValue<bool> ingameUIisVisible;

    private static readonly ConcurrentDictionary<NormalInventoryItem, RectangleF> topBases =
            new ConcurrentDictionary<NormalInventoryItem, RectangleF>();

    public override void OnLoad()
    {
        CanUseMultiThreading = true;
    }
    public override bool Initialise()
    {
        var _ingameUI = GameController.IngameState.IngameUi;
        ingameUIisVisible = new TimeCache<bool>(() =>
            {
                return _ingameUI.SyndicatePanel.IsVisibleLocal
                    || _ingameUI.TreePanel.IsVisibleLocal
                    || _ingameUI.Atlas.IsVisibleLocal;
            }, 250);
        return true;
    }

    public override Job Tick()
    {
        if (Settings.MultiThreading)
            return GameController.MultiThreadManager.AddJob(TickLogic, Name);

        TickLogic();
        return null;
    }

    private void TickLogic()
    {
        var _playerInventory = GameController.Game.IngameState.IngameUi.InventoryPanel;
        var _serverInventory = GameController.Game.IngameState.IngameUi.HaggleWindow;

        var _topBasesList = new List<NormalInventoryItem>();

        if (_playerInventory.IsVisibleLocal)
        {
            foreach (var item in _playerInventory[InventoryIndex.PlayerInventory].VisibleInventoryItems)
            {
                var BIT = GameController.Files.BaseItemTypes.Translate(item.Item.Path);
                var _tags = BIT.Tags;
                if (BIT == null) continue;
                if (_tags.Contains("top_tier_base_item_type"))
                    _topBasesList.Add(item);
            }
        }

        if (_serverInventory.IsVisibleLocal)
        {
            foreach (var item in _serverInventory.InventoryItems)
            {
                var BIT = GameController.Files.BaseItemTypes.Translate(item.Item.Path);
                var _tags = BIT.Tags;
                if (BIT == null) continue;
                if (_tags.Contains("top_tier_base_item_type"))
                    _topBasesList.Add(item);
            }
        }

        foreach (var key in topBases.Keys.Where(x => !_topBasesList.Contains(x)))
            topBases.TryRemove(key, out _);

        foreach (var item in _topBasesList)
        {
            var _rectangle = item?.GetClientRect();
            if (_rectangle == null) continue;
            topBases.AddOrUpdate(item, (RectangleF)_rectangle, (key, oldValue) => (RectangleF)_rectangle);
        }
    }

    public override void Render()
    {
        if (ingameUIisVisible.Value) return;
        var inventoryPanel = GameController.Game.IngameState.IngameUi.InventoryPanel;
        var serverInventory = GameController.Game.IngameState.IngameUi.HaggleWindow;
        foreach (var item in topBases.Keys)
        {
            var color = inventoryPanel.IsVisibleLocal && inventoryPanel[InventoryIndex.PlayerInventory].VisibleInventoryItems.Contains(item) ? Settings.InventoryColor.Value :
                        serverInventory.IsVisibleLocal && serverInventory.InventoryItems.Contains(item) ? Settings.ServerColor.Value :
                        Color.White;
            Graphics.DrawFrame(topBases[item], color, Settings.BorderThickness);
        }
    }

    public override void DrawSettings()
    {
        base.DrawSettings();
        ImGui.Separator();
    }


}