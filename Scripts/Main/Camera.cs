using Godot;

public partial class Camera : Camera2D
{
    Vector2 ogCamPos = new();
    bool mbDown = false;
    bool dragging = false;
    Vector2 oGmBPos = new();

    public override void _Ready()
    {
        ogCamPos = this.GlobalPosition;
    }
    public override void _Input(InputEvent @event)
    {
        if (!dragging) { return; }
        if (@event is InputEventMouseMotion eventMouseMotion && mbDown)
        {
            dragging = true;
            var diff = ogCamPos - eventMouseMotion.GlobalPosition + oGmBPos;
            this.GlobalPosition = diff;
        }
    }
    public override void _UnhandledInput(InputEvent @event)
    {
        base._Input(@event);
        dragging = false;
        var x = Mathf.Clamp(Zoom.X, 1, 30);
        var y = Mathf.Clamp(Zoom.Y, 1, 30);
        if(@event.IsActionPressed("Left MB"))
        {
            mbDown = true;
            oGmBPos = this.GetViewport().GetMousePosition();
        }
        else if(@event.IsActionReleased("Left MB"))
        {
            dragging = false;
            mbDown = false;
            oGmBPos = new Vector2();
            ogCamPos = this.GlobalPosition;
        }
        if (@event is InputEventMouseMotion eventMouseMotion && mbDown)
        {
            dragging = true;
            var diff = ogCamPos - eventMouseMotion.GlobalPosition + oGmBPos;
            this.GlobalPosition = diff;
        }

        if(@event.IsAction("Zoom In"))
        {
            GD.Print("zoom% ", Zoom.X);
            Zoom = new Vector2(x+.25f, y+.25f);
        }
        else if(@event.IsAction("Zoom Out"))
        {
            GD.Print("zoom% ", Zoom.X);
            Zoom = new Vector2(x-.25f, y-.25f);
        }
    }
}
