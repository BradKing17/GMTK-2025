using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;

public partial class Postie : Node2D
{
    public List<Points> loop = new List<Points>();
    public bool isLooping = false;
    public Path2D path = new Path2D();
    public PathFollow2D pathFollow = new PathFollow2D();
    public class Vehicle
    {
        // get enum name by toString() after entry;
        public enum Transport { Walking, Trolley, Van };
        public Transport transport { get; set; } = Transport.Walking;
        public float speedMultiplier;
        public float fatigueMultiplier;
        public int AdditionalMaxPostage;


        public static Vehicle SetVehicle(Vehicle.Transport transport)
        {
            switch (transport)
            {
                case Transport.Van:
                {
                    return Van();
                };
                case Transport.Trolley:
                {
                    return Trolley();
                };
                case Transport.Walking:
                {
                    return Walking();
                };
            }
            return null;
        }
        private static Vehicle Van()
        {
            return new Vehicle()
            {
                transport = Transport.Van,
                speedMultiplier = 1.25f,
                fatigueMultiplier = .5f,
                AdditionalMaxPostage = 50
            };
        }
        private static Vehicle Trolley()
        {
            return new Vehicle()
            {
                transport = Transport.Trolley,
                speedMultiplier = 1.1f,
                fatigueMultiplier = 1.1f,
                AdditionalMaxPostage = 25
            };
        }
        private static Vehicle Walking()
        {
            return new Vehicle()
            {
                transport = Transport.Walking,
                speedMultiplier = 1f,
                fatigueMultiplier = 1f,
                AdditionalMaxPostage = 0
            };
        }
    }

    public class Status
    {
        public enum Feeling { Caffinated, Woke, Happy, Normal, Weak, Sleepy, Fatal, Shot };
        public Feeling feeling { get; set; } = Feeling.Normal;
        public bool Active = false;
        public float fatiguePercentage = 0f;
        public float speed = 10.0f;
        public int arms = 2;
        public int currentPost = 0;
        public int level = 1;

        public Status SetStatus(Feeling feeling)
        {
            switch (feeling)
            {
                case Feeling.Caffinated:
                {
                    return Caffinated();
                };
                case Feeling.Woke:
                {
                    return Woke();  
                };
                case Feeling.Happy:
                {
                    return Happy();  
                };
                case Feeling.Normal:
                {
                    return Normal();  
                };
                case Feeling.Weak:
                {
                    return Weak();  
                };
                case Feeling.Sleepy:
                {
                    return Sleepy();  
                };
                case Feeling.Fatal:
                {
                    return Fatal();  
                };
                case Feeling.Shot:
                {
                    return Shot();  
                };
            }
            return null;
        }
        private Status Caffinated()
        {
            return new Status()
            {
                feeling = Feeling.Caffinated,
                speed = 15f * (1 + (level *.0f))
            };
        }
        private Status Woke()
        {
            return new Status()
            {
                feeling = Feeling.Woke,
                speed = 12.5f * (1 + (level *.0f))
            };
        }
        private Status Happy()
        {
            return new Status()
            {
                feeling = Feeling.Happy,
                speed = 11.5f * (1 + (level *.0f))
            };
        }
        private Status Normal()
        {
            return new Status()
            {
                feeling = Feeling.Normal,
                speed = 10f * (1 + (level *.0f))
            };
        }
        private Status Weak()
        {
            return new Status()
            {
                feeling = Feeling.Weak,
                speed = .8f * (1 + (level *.0f))
            };
        }
        private Status Sleepy()
        {
            return new Status()
            {
                feeling = Feeling.Sleepy,
                speed = .6f * (1 + (level *.0f))
            };
        }
        private Status Fatal()
        {
            return new Status()
            {
                feeling = Feeling.Fatal,
                speed = .3f * (1 + (level *.0f))
            };
        }
        private Status Shot()
        {
            return new Status()
            {
                feeling = Feeling.Shot,
                speed = 0f
            };
        }
        public static bool CheckFatigued(float fatiguePercentage)
        {
            if(fatiguePercentage >= 100)
            {
                return true;
            }
            return false;
        }
        public void addPost(int AdditionalPost)
        {
            currentPost += AdditionalPost;
        }
    }
    [Export] public float clickableRadius = 20;
    private string name;
    private bool mouseInside = false;
    private Area2D area;
    private CollisionShape2D collider;
    public Vehicle vehicle = Vehicle.SetVehicle(Vehicle.Transport.Walking);
    public Status status = new();    
    private PackedScene PackedStatusScreen = GD.Load<PackedScene>("res://Assets/Objects/UI/StatusScreen.tscn");
    public PostieStatus StatusScreen;
    [Export] CanvasLayer canvasLayer;
    public Globals globals;
    public ColorRect debugIcon;

    public override void _Ready()
    {
        globals = GetNode<Globals>(GetTree().Root.GetChild(0).GetPath());
        debugIcon = new ColorRect()
        {
            Size = new Vector2(20, 20),
            Position = new Vector2(-10, -10),
            MouseFilter = Control.MouseFilterEnum.Ignore,
            Color = Colors.Pink // debugpink
        };

        canvasLayer = globals.canvasLayer;
        name = Name = Utitily.RandomName.returnJsonNames("res://Scripts/Main/Jsons/PostieNames.json", ["Forenames", "Surnames"]);
        area = new Area2D();
        collider = new CollisionShape2D();
        AddChild(area);
        area.AddChild(collider);
        area.MouseEntered += HandleMouseEntered;
        area.MouseExited += HandleMouseExited;
        collider.DebugColor = Godot.Colors.Red;
        collider.Shape = new CircleShape2D()
        {
            Radius = clickableRadius,
            CustomSolverBias = 0,
        };
        vehicle = Vehicle.SetVehicle(Vehicle.Transport.Walking);
        status = status.SetStatus(Status.Feeling.Normal);
    }

    public void PointHit(Points currentPoint)
    {
        currentPoint.HitPoint(this);
    }

    private void HandleMouseEntered()
    {
        mouseInside = true;
        var tweener = GetTree().CreateTween();
        tweener.TweenProperty(area.GetChild<CollisionShape2D>(0).Shape, "radius", clickableRadius + 20, 0.25f)
				.SetTrans(Tween.TransitionType.Back)
				.SetEase(Tween.EaseType.Out);
    }
    private void HandleMouseExited()
    {
        mouseInside = false;
        var tweener = GetTree().CreateTween();
        tweener.TweenProperty(area.GetChild<CollisionShape2D>(0).Shape, "radius", clickableRadius, 0.25f)
					.SetTrans(Tween.TransitionType.Back)
					.SetEase(Tween.EaseType.In);
    }
    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if(@event.IsActionPressed("Left MB") && mouseInside)
        {
            GD.Print("clicked on Postie: " + Name);
            GD.Print("Vehicle: " + vehicle.transport, ", spd mult: ", vehicle.speedMultiplier, ", ftg mult:", vehicle.fatigueMultiplier);
            GD.Print("Status: ", "feeling: ", status.feeling, ", active: ", status.Active, ", post: ", status.currentPost, ", ftg%: ", status.fatiguePercentage, ", spd: ", status.speed, ", lvl: ", status.level);
                    GD.Print("SP before: ", globals.selectedPostie);
            globals.selectedPostie = this;
                    GD.Print("SP after: ", globals.selectedPostie);
            createStatusScreen();
        }
    }

    private void createStatusScreen()
    {
        if(StatusScreen != null){ return; };
        StatusScreen = PackedStatusScreen.Instantiate<PostieStatus>();
        StatusScreen.soverignPostie = this;
        StatusScreen.NameLabel.Text = name;
        StatusScreen.StatusLabel.Text = status.feeling.ToString();
        StatusScreen.VehicleOptionsButton.Selected = (int)vehicle.transport;
        StatusScreen.FatigueLabel.Text = status.fatiguePercentage.ToString();
        canvasLayer.AddChild(StatusScreen);
    }

    public void AssignedLoop()
    {
        GD.Print("ARray top SP: ", globals.selectedPostie);
        path = new Path2D();
        globals.PointsLayer.AddChild(path);
        path.Curve = new Curve2D();
        foreach (Points point in loop)
        {
            GD.Print(point.GetPosition());
            path.Curve.AddPoint(point.GetPosition());
        }
        foreach (var child in pathFollow.GetChildren())
        {
            if (child is ColorRect) { return; }
            child.QueueFree();
        }
        pathFollow = new();
        path.AddChild(pathFollow);
        isLooping = true;
        pathFollow.Loop = true;
        pathFollow.AddChild(debugIcon);
        GD.Print("ARray bot SP: ", globals.selectedPostie);
    }

    public override void _Process(double delta)
    {
        if(isLooping == true)
        {
            pathFollow.Progress += (float)(delta * 10.0 * status.speed);
        }
        base._Process(delta);
    }
}
