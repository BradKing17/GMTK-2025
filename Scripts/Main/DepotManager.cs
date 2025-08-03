using Godot;
using System;
using System.Collections.Generic;

public partial class DepotManager : Node2D
{
    public Globals globals;
    private int NumOfWagons = 0;
    private int NumOfVans = 0;

    [Export] public Postie selectedPostie = null;

    public Points highlightedPoint = null;

    public override void _Ready()
    {
        globals = GetNode<Globals>(GetTree().Root.GetChild(0).GetPath());
    }
    public void StartLoop(List<Points> newLoop)
    {
        // selectedPostie.loop = newLoop;
        // selectedPostie.AssignedLoop();
    }

    public void BirthPostie()
    {
        // Ask Brad if this layering is correct;
        Postie postie = new();
        globals.totalPosties.Add(postie);
        globals.MapLayer.AddChild(postie);
        postie.Position = globals.postOffice.Position;
    }
}
