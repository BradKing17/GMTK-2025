using Godot;
using System;
using System.Linq;

public partial class Points : Node2D
{
    public int GetNumConnections() { return numOfConnections; }
    public int postWaitingForDelivery = 0;
    public bool maxPostReached = false;
    public float radius = 20;

    private Tween tween;
    private Vector2 position;
    private int numOfConnections;
    private string pointType = "Base";
    public string GetPointType() { return pointType; }
    public void SetPointType(string type) { pointType = type; }
    public override void _Ready()
    {
        var area = new Area2D();
        var collider = new CollisionShape2D();
        AddChild(area);
        area.AddChild(collider);
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
        tweener.TweenProperty(GetChild<Area2D>(0).GetChild<CollisionShape2D>(0).Shape, "radius", radius + 20, 0.25f)
				.SetTrans(Tween.TransitionType.Back)
				.SetEase(Tween.EaseType.Out);
    }
    private void HandleMouseExited()
    {
        var tweener = GetTree().CreateTween();
        tweener.TweenProperty(GetChild<Area2D>(0).GetChild<CollisionShape2D>(0).Shape, "radius", radius, 0.25f)
					.SetTrans(Tween.TransitionType.Back)
					.SetEase(Tween.EaseType.In);
    }

    public new Vector2 GetPosition() { return position; }
    public new void SetPosition(Vector2 newPos) { position = newPos; }
}
