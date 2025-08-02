using Godot;
using System;
using System.Collections.Generic;

public partial class DepotManager : Node2D
{
    private List<Postie> postieList = new List<Postie>();
    private int NumOfWagons = 0;
    private int NumOfVans = 0;

    private Postie selectedPostie = null;

    [Export]
    private PointManager pointManager = null;
    private Script pointManagerScript = null;

    public Points highlightedPoint = null;

    public override void _Ready()
    {
        pointManagerScript = (Script)pointManager.GetScript();
        GD.Print(pointManagerScript);
    }
    void StartLoop(Points startPoint)
    {

    }

}
