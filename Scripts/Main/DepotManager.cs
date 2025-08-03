using Godot;
using System;
using System.Collections.Generic;

public partial class DepotManager : Node2D
{
    public List<Postie> postieList = new List<Postie>();
    private int NumOfWagons = 0;
    private int NumOfVans = 0;

    private Postie selectedPostie = null;

    //[Export]
    //public PointManager pointManager;

    public Points highlightedPoint = null;



    public override void _Ready()
    {
       // pointManager.depotManager = this;
    }
    public void StartLoop(List<Points> newLoop)
    {
        selectedPostie.loop = newLoop;
    }
}
