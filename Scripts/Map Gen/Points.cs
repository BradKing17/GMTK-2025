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
    private Timer postTimer;
    private Label timerLabel;

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

    public void InitializeTimer(Node parent, Label label)
    {
        timerLabel = label;
        postTimer = new Timer();
        postTimer.WaitTime = 1.0f;
        postTimer.OneShot = false;
        postTimer.Timeout += OnPostTimerTimeout;
        parent.AddChild(postTimer);
        postTimer.Start();
        UpdateLabel();
    }

    private void OnPostTimerTimeout()
    {
        if (postWaitingForDelivery < 30)
        {
            postWaitingForDelivery++;
            if (postWaitingForDelivery == 30)
            {
                maxPostReached = true;
            }
        }
        else
        {
            maxPostReached = true;
        }
        
        UpdateLabel();

    }
    private void UpdateLabel()
    {
        if (timerLabel != null)
        {
            timerLabel.Text = $"{postWaitingForDelivery}";
            if (postWaitingForDelivery == 30)
                timerLabel.AddThemeColorOverride("font_color", new Color(1, 0, 0)); // Red
            else
                timerLabel.AddThemeColorOverride("font_color", new Color(1, 1, 1)); // White or default
        }
    }

    public void DecreasePost(int amount)
    {
        postWaitingForDelivery -= amount;

        if (postWaitingForDelivery < 30)
        {
            maxPostReached = false;
        }
        if (postWaitingForDelivery < 0)
        {
            postWaitingForDelivery = 0;
        }
    }
}
