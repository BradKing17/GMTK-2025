using Godot;
using System.Collections.Generic;
using System.Drawing;

public partial class MapGenerator : Node
{

    [Export]
    private int mapSizeX = 0;
    [Export]
    private int mapSizeY = 0;

    [Export]
    private int numOfPoints = 0;

    [Export]
    private int maxConnections = 1;

    [Export]
    private float minPointGenDistance = 50;

    [Export]
    private float maxConnectionDistance = 60;
    private PackedScene PackedPoint = GD.Load<PackedScene>("res://Assets/Objects/Point.tscn");

    private PointManager pointManager;
    [Export] private float pointRadius = 20;
    public override void _Ready()
    {
        pointManager = new PointManager();
        AddChild(pointManager);
        GenerateMap();
    }

    void GenerateMap()
    {
        GD.Print("Running Generation");
        List<Points> points = GeneratePoints();

        GenerateStreets(points);
    }

    List<Points> GeneratePoints()
    {
        List<Points> pointsFilled = new List<Points>();
        for (int i = 0; i < numOfPoints; i++)
        {
            Points newPoint = PackedPoint.Instantiate<Points>();

            Vector2 newPos = new Vector2(GD.RandRange(0, mapSizeX), (GD.RandRange(0, mapSizeY)));
            foreach(Points point in pointsFilled)
            {
                if(newPos.DistanceTo(point.GetPosition()) < minPointGenDistance)
                {
                    newPos = new Vector2(GD.RandRange(0, mapSizeX), (GD.RandRange(0, mapSizeY)));
                }
            }

            newPoint.SetPosition(newPos);            
            newPoint.Position = newPoint.GetPosition();

            Label timerLabel = new()
            {
                Text = "0",
                Position = new Vector2(0, -20)
            };
            newPoint.AddChild(timerLabel);

            newPoint.InitializeTimer(this, timerLabel); // Start the timer for this point and pass the label
            pointsFilled.Add(newPoint);
            pointManager.RegisterPoint(newPoint); // Register the point with the PointManager

            GD.Print(newPoint.GetPosition());
            newPoint.radius = this.pointRadius;
            this.AddChild(newPoint);

        }

        return pointsFilled;
    }

    void GenerateStreets(List<Points> pointsPassed)
    {
        GD.Print("Generating Streets");
        foreach (var point in pointsPassed)
        {
           

           //var newConnection = pointsPassed[GD.RandRange(1, pointsPassed.Count) - 1];

           foreach(Points newConnection in pointsPassed)
            {
                if(point.GetPosition().DistanceTo(newConnection.GetPosition()) < maxConnectionDistance && point != newConnection)
                {
                    Line2D newLine = new Line2D();
                    newLine.AddPoint(point.GetPosition());
                    newLine.AddPoint(newConnection.GetPosition());
                    newLine.Width = 2;
                    newLine.DefaultColor = new Godot.Color(0.8f, 0.8f, 0.8f);
                    AddChild(newLine);
                }
            }
        }
    }
}

