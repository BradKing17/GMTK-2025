using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Points : Node2D
{
    public int GetNumConnections() { return numOfConnections; }
    public int postWaitingForDelivery = 0;
    public bool maxPostReached = false;
    public float radius = 20;
    public Color mainColor = new Color(1f, 1f, 1f);
    public bool isSelected = false;
    private Tween tween;
    private Vector2 position;
    private int numOfConnections;
    private List<Points> neighbourPoints;

    private string pointType = "Base";
    public PointManager manager = null;
    public string GetPointType() { return pointType; }
    public void SetPointType(string type) { pointType = type; }

    private Area2D area;
    protected CollisionShape2D collider;
    protected ColorRect debugIcon;
    public override void _Ready()
    {
        neighbourPoints = new List<Points>();
        area = new Area2D();
        collider = new CollisionShape2D();
        AddChild(area);
        area.AddChild(collider);
        area.MouseEntered += HandleMouseEntered;
        area.MouseExited += HandleMouseExited;

        collider.Shape = new CircleShape2D()
        {
            Radius = radius,
            CustomSolverBias = 0
        };

        debugIcon = new ColorRect()
        {
            Size = new Vector2(20, 20),
            Position = new Vector2(-10, -10),
            MouseFilter = Control.MouseFilterEnum.Ignore,
            Color = Colors.Pink // debugpink
        };
        AddChild(debugIcon);
    }

    protected virtual void HandleMouseEntered()
    {
        var tweener = GetTree().CreateTween();
        tweener.TweenProperty(collider.Shape, "radius", radius + 20, 0.25f)
				.SetTrans(Tween.TransitionType.Back)
				.SetEase(Tween.EaseType.Out);
        manager.SetHighlightedPoint(this);
    }
    protected virtual void HandleMouseExited()
    {

        var tweener = GetTree().CreateTween();
        tweener.TweenProperty(collider.Shape, "radius", radius, 0.25f)
					.SetTrans(Tween.TransitionType.Back)
					.SetEase(Tween.EaseType.In);
        manager.SetHighlightedPoint(null);
    }

    public virtual void HitPoint(Postie postie)
    {

    }

    public new Vector2 GetPosition() { return position; }
    public new void SetPosition(Vector2 newPos) { position = newPos; }

    public List<Points> GetNeighbours() { return neighbourPoints; }
    public void AddNeighbour(Points newPoint) { neighbourPoints.Add(newPoint); }
    public void RemoveNeighbour(Points newPoint) { neighbourPoints.Remove(newPoint); }
}
