using Godot;
using System;

public partial class PostOffice : Points
{ 
    public override void _Ready()
    {
        base._Ready();
        mainColor = debugIcon.Color = Colors.Yellow;
    }
}
