using Godot;
using System;

public partial class PostDepot : Points
{
     public override void _Ready()
    {
        base._Ready();
        mainColor = debugIcon.Color = Colors.Orange;
        SetSprite("res://Assets/Images/MapSprites/postDepot.png");
    }
}
