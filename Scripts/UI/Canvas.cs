using Godot;
using System;
using System.Collections.Generic;

public partial class Canvas : CanvasLayer
{
    public Globals globals;
    // public List<Postie> canvasPostie;
    public List<DepotUI> canvasDepo = new();
    public override void _Ready()
    {
        globals = GetNode<Globals>(GetTree().Root.GetChild(0).GetPath());
    }

    public void updatePostieCount()
    {
        foreach (var depotUI in canvasDepo)
        {
            GD.Print(depotUI);
            depotUI.postieTotal.Text = globals.totalPosties.Count.ToString();
        }
    }
}
