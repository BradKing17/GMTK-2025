using Godot;
using System;
using System.Collections.Generic;

public partial class MapGenPoisson : Node2D
{
    [Export]
    public Polygon2D mapShape;

    [Export]
    private float poisson_radius = 20;

    private PackedScene PackedPoint = GD.Load<PackedScene>("res://Assets/Objects/Point.tscn");

    private PointManager pointManager;

    public override void _Ready()
    {

        pointManager = new PointManager();
        AddChild(pointManager);
        List<Points> points = new List<Points>();
        points = GeneratePoints();
        GenerateStreets(points);
    }

    List<Points> GeneratePoints()
    {
        var poissonScript = GD.Load<GDScript>("res://addons/PoissonDiscSampling/poisson_disc_sampling.gd");
        var poissonScriptNode = (GodotObject)poissonScript.New();
        Vector2[] newVectors = (Vector2[])poissonScriptNode.Call("generate_points_for_polygon", mapShape.Polygon, poisson_radius, 10); ;

        List<Points> pointsFilled = new();

        foreach (var vector in newVectors)
        {

            Points newPoint = PackedPoint.Instantiate<Points>();

            AddTimer(newPoint);

            newPoint.SetPosition(vector);


            pointsFilled.Add(newPoint);
            pointManager.RegisterPoint(newPoint); // Register the point with the PointManager

            newPoint.Position = newPoint.GetPosition();

            AddChild(newPoint);
        }

        return pointsFilled;
    }

    private void AddTimer(Points newPoint)
    {
        Label timerLabel = new()
        {
            Text = "0",
            Position = new Vector2(0, -40)
        };
        newPoint.AddChild(timerLabel);

        newPoint.InitializeTimer(this, timerLabel); // Start the timer for this point and pass the label
    }

    void GenerateStreets(List<Points> pointsPassed)
    {
        GD.Print("Generating Streets with: " + pointsPassed.Count + " Points");
        foreach (var point in pointsPassed)
        {

            foreach (Points newConnection in pointsPassed)
            {
                if (point.GetPosition().DistanceTo(newConnection.GetPosition()) < 50 && point != newConnection)
                {
                    Line2D newLine = new Line2D();
                    newLine.AddPoint(point.GetPosition());
                    newLine.AddPoint(newConnection.GetPosition());
                    newLine.Width = 2;
                    newLine.DefaultColor = new Color(0.8f, 0.8f, 0.8f);
                    AddChild(newLine);
                }
            }
        }
    }
}
