using Godot;
using System;

public partial class PostOffice : Points
{
    private bool mouseInside;
    private PackedScene PackedDepotScreen = GD.Load<PackedScene>("res://Assets/Objects/UI/DepotScreen.tscn");
    public DepotUI DepotScreen;

    public override void _Ready()
    {
        radius = 50;
        base._Ready();
        collider.DebugColor = Godot.Colors.PaleVioletRed;

        globals.postOffice = this;

        mainColor = debugIcon.Color = Colors.Yellow;
        SetSprite("res://Assets/Images/MapSprites/postOffice.png");
    }

    protected override void HandleMouseEntered()
    {
        base.HandleMouseEntered();
        
        GD.Print("mouse inside: ", this.Name);
        mouseInside = true;
    }
    protected override void HandleMouseExited()
    {
        base.HandleMouseExited();
        mouseInside = false;
    }
    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);
        if (@event.IsActionPressed("Left MB") && mouseInside)
        {
            createStatusScreen();
        }
    }

    private void createStatusScreen()
    {
        if(PackedDepotScreen == null){ return; };
        DepotScreen = PackedDepotScreen.Instantiate<DepotUI>();
        GD.Print(globals);
        GD.Print("canvas layer: ", globals.canvasLayer);
        globals.canvasLayer.AddChild(DepotScreen);
    }

}
