using Godot;
using System;

public partial class Status : Node
{
    protected enum Feeling {
    };
    [Export] Feeling feeling;

    protected enum Vehicle { 
		Walking, Trolley, Car, Hobbling
    };
    [Export] Vehicle vehicle;
    [Export] public RichTextLabel StatusContent;
    [Export] public RichTextLabel VehicleContent;
    [Export] public RichTextLabel FatigueContent;
}
