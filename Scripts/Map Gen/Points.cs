using Godot;
using System;
using System.Linq;

public partial class Points : Node2D
{
    public int GetNumConnections() { return numOfConnections; }
    public int postWaitingForDelivery = 0;
    public bool maxPostReached = false;
    public float radius = 20; 
    [Export] protected Area2D area;
    [Export] protected CollisionShape2D collider;

    private Tween tween;
    private Vector2 position;
    private int numOfConnections;
    private string pointType = "Base";

    public string GetPointType() { return pointType; }
    public void SetPointType(string type) { pointType = type; }

    
    public override void _EnterTree()
    {
        area ??= this.GetChild<Area2D>(0);
        area.MouseEntered += HandleMouseEntered;
        area.MouseExited += HandleMouseExited;

        collider.Shape = new CircleShape2D()
        {
            Radius = radius,
            CustomSolverBias = 0
        };
    }

    private void HandleMouseEntered()
    {
        var tweener = GetTree().CreateTween();
        tweener.TweenProperty(collider.Shape, "radius", radius + 20, 0.25f)
				.SetTrans(Tween.TransitionType.Back)
				.SetEase(Tween.EaseType.Out);
    }
    private void HandleMouseExited()
    {
        var tweener = GetTree().CreateTween();
        tweener.TweenProperty(collider.Shape, "radius", radius, 0.25f)
					.SetTrans(Tween.TransitionType.Back)
					.SetEase(Tween.EaseType.In);
    }

    public new Vector2 GetPosition() { return position; }
    public new void SetPosition(Vector2 newPos) { position = newPos; }
}
