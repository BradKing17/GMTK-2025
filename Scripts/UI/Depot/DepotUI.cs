using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class DepotUI : Control
{
    public Globals globals;
    [Export] public Button NameLabel;
    [Export] public Button closeButton;
    [Export] public Button plusPosties;
    [Export] public Button minusPosties;
    Vector2 mouseNodeDiff = new();
    private bool grabbedWindow = false;

    // [Export] public List<Postie> activePostes;
    // [Export] public List<Postie> inActivePostes;
    public override void _Ready()
    {
        base._Ready();
        globals = GetNode<Globals>(GetTree().Root.GetChild(0).GetPath());

        closeButton.Pressed += closeWindow;
        NameLabel.ButtonDown += AttachWindow;
    }

    private void closeWindow()
    {
        this.QueueFree();
    }
    private void addPosties()
    {
        
    }
    private void cullPosties()
    {
        Postie postieToPop = globals.totalPosties.Last();
        Postie target = GetNode<Postie>(postieToPop.GetPath());
        globals.totalPosties.Remove(postieToPop);
        target.QueueFree();
    }

    private void AttachWindow()
    {
        var oGmBPos = this.GetViewport().GetMousePosition();
        var currentpos = this.GlobalPosition;
        mouseNodeDiff = currentpos - oGmBPos;
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (@event is InputEventMouseMotion eventMouseMotion && NameLabel.ButtonPressed)
        {
            var diff = eventMouseMotion.GlobalPosition + mouseNodeDiff;
            this.GlobalPosition = diff;
        }
    }
}



