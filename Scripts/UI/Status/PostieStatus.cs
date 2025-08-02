using Godot;
using System;

public partial class PostieStatus : Control
{
    Postie postieData;
    [Export] public RichTextLabel NameLabel;
    [Export] public RichTextLabel StatusLabel;
    [Export] public OptionButton VehicleOptionsButton;
    [Export] public RichTextLabel FatigueLabel;

    public override void _Ready()
    {
        base._Ready();
    }
}
