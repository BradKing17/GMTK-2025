using Godot;
using System;
using System.Collections.Generic;

public partial class Globals : Node
{
    [Export] public PostOffice postOffice;
    [Export] public DepotManager depotManager;
    [Export] public PointManager pointManager;
    [Export] public Canvas canvasLayer;
    public List<Postie> totalPosties = new();

    [Export] public Node2D MapLayer;
}
