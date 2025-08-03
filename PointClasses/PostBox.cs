using Godot;
using System;

public partial class PostBox : Points
{
     public override void _Ready()
    {
        base._Ready();
        mainColor = debugIcon.Color = Colors.Red;
    }
}
