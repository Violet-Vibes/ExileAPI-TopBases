using ExileCore.Shared.Attributes;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;
using SharpDX;

namespace TopBases;

public class TopBasesSettings : ISettings
{
    //Mandatory setting to allow enabling/disabling your plugin
    public ToggleNode Enable { get; set; } = new ToggleNode(false);
    public ToggleNode MultiThreading { get; set; } = new ToggleNode(false);
    [Menu("If you don't want to draw items for a specific element, set the color transparency to 0", 100)]
    public EmptyNode Description { get; set; }
    [Menu("Color for the player inventory", parentIndex = 100)]
    public ColorNode InventoryColor { get; set; } = new ColorNode(Color.White);
    [Menu("Color for Rog/Gwennen inventory", parentIndex = 100)]
    public ColorNode ServerColor { get; set; } = new ColorNode(Color.White);
    [Menu("Border thickness", parentIndex = 100)]
    public RangeNode<int> BorderThickness { get; set; } = new RangeNode<int>(1, 1, 5);



    //Put all your settings here if you can.
    //There's a bunch of ready-made setting nodes,
    //nested menu support and even custom callbacks are supported.
    //If you want to override DrawSettings instead, you better have a very good reason.
}