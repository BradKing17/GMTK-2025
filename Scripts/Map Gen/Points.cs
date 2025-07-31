using Godot;
using System;

public partial class Points : Node2D
{

    public int GetNumConnections() { return numOfConnections; }
    public Vector2 scale = new(1,1);
    [Export] protected Area2D area;
    [Export] protected CollisionShape2D collider;

    private Tween tween;
    private Vector2 position;
    private int numOfConnections;

    public override void _EnterTree()
    {
        area ??= this.GetChild<Area2D>(0);
        area.MouseEntered += HandleMouseEntered;
        area.MouseExited += HandleMouseExited;
    }

    private void HandleMouseEntered()
    {
        var tweener = GetTree().CreateTween();
        tweener.TweenProperty(collider, "scale", scale + new Vector2(.35f, .35f), 0.25f)
				.SetTrans(Tween.TransitionType.Back)
				.SetEase(Tween.EaseType.Out);
    }
    private void HandleMouseExited()
    {
        var tweener = GetTree().CreateTween();

        tweener.TweenProperty(collider, "scale", scale, 0.25f)
					.SetTrans(Tween.TransitionType.Back)
					.SetEase(Tween.EaseType.In);
    }
}
