using Godot;
using System;
using System.Collections.Generic;

public partial class DepotManager : Node2D
{
    public Globals globals;
    // public List<Postie> postieList = new List<Postie>();
    private int NumOfWagons = 0;
    private int NumOfVans = 0;

    private Postie selectedPostie = null;

    public Points highlightedPoint = null;

    public override void _Ready()
    {
        globals = GetNode<Globals>(GetTree().Root.GetChild(0).GetPath());
    }
    public void StartLoop(List<Points> newLoop)
    {
        selectedPostie.loop = newLoop;
    }

    public void BirthPostie()
    {
        // dpeot
    }
}
