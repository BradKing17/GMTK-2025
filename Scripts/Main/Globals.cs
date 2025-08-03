using Godot;
using System;
using System.Collections.Generic;

public partial class Globals : Node
{
    [Export] public DepotManager depotManager;
    [Export] public PointManager pointManager;
    [Export] public CanvasLayer canvasLayer;
    [Export] public List<Postie> totalPosties = new();
    
}
