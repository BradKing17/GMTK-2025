using Godot;
using System;

public partial class PostieStatus : Control
{
    public Postie soverignPostie;
    [Export] public Button NameLabel;
    [Export] public RichTextLabel StatusLabel;
    [Export] public OptionButton VehicleOptionsButton;
    [Export] public RichTextLabel FatigueLabel;
    [Export] public Button closeButton;
    Vector2 mouseNodeDiff = new();

    [Export] private Sprite2D[] vehicleSprites = [];
    [Export] private AnimationPlayer vehicleAnimator;

    private bool grabbedWindow = false;
    public override void _Ready()
    {
        base._Ready();
        VehicleOptionsButton.ItemSelected += updateVehicle;
        closeButton.Pressed += closeWindow;

        NameLabel.ButtonDown += AttachWindow;
    }

    private void updateVehicle(long index)
    {
        soverignPostie.vehicle = Postie.Vehicle.SetVehicle((Postie.Vehicle.Transport)index);
        switch (soverignPostie.vehicle.transport)
        {
            case Postie.Vehicle.Transport.Van:
            {
                vehicleSprites[0].Visible = true;
                vehicleSprites[1].Visible = false;
                vehicleAnimator.Play("Driving");
                // vehicleSprites[2].Visible = false; no walking sprite yet
                return;
            }
            case Postie.Vehicle.Transport.Trolley:
            {
                vehicleSprites[0].Visible = false;
                vehicleSprites[1].Visible = true;
                vehicleAnimator.Play("TrolleyDriving");

                // vehicleSprites[2].Visible = false; no walking sprite yet
                return;
            }
            case Postie.Vehicle.Transport.Walking:
            {
                vehicleSprites[0].Visible = false;
                vehicleSprites[1].Visible = false;
                // vehicleSprites[2].Visible = true; no walking sprite yet
                return;
            }
        }
    }

    private void closeWindow()
    {
        soverignPostie.StatusScreen = null;
        this.QueueFree();
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
