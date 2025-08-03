using Godot;
using System;

public partial class WaterFountain : Points
{
    public override void _Ready()
    {
        base._Ready();
        mainColor = debugIcon.Color = Colors.Blue;
    }
}
