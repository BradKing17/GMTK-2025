using Godot;
using System;
using System.Collections.Generic;

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
    PackedScene pointMarker = ResourceLoader.Load<PackedScene>("res://Marker.tscn");

    [Export]
    private float minPointGenDistance = 50;

    [Export]
    private float maxConnectionDistance = 60;



    public override void _Ready()
    {
        GenerateMap();
    }

    void GenerateMap()
    {
        GD.Print("Running Generation");
        List<Point> points = new List<Point>();

        points = GeneratePoints();

        GenerateStreets(points);
    }

    List<Point> GeneratePoints()
    {
        List<Point> pointsFilled = new List<Point>();
        for (int i = 0; i < numOfPoints; i++)
        {
            Point newPoint = new Point();
            Vector2 newPos = new Vector2(GD.RandRange(0, mapSizeX), (GD.RandRange(0, mapSizeY)));
            foreach(Point point in pointsFilled)
            {
                if(newPos.DistanceTo(point.GetPosition()) < minPointGenDistance)
                {
                    newPos = new Vector2(GD.RandRange(0, mapSizeX), (GD.RandRange(0, mapSizeY)));
                }
            }
            newPoint.SetPosition(newPos);
            newPoint.InitializeTimer(this); // Start the timer for this point
            pointsFilled.Add(newPoint);

            Node2D pointInstance = (Node2D)pointMarker.Instantiate();
            pointInstance.Position = newPoint.GetPosition();
            GD.Print(newPoint.GetPosition());
            this.AddChild(pointInstance);
        }

        return pointsFilled;
    }

    void GenerateStreets(List<Point> pointsPassed)
    {
        GD.Print(pointsPassed.Count);
        foreach (var point in pointsPassed)
        {
            GD.Print("Generating Streets");

           //var newConnection = pointsPassed[GD.RandRange(1, pointsPassed.Count) - 1];

           foreach(Point newConnection in pointsPassed)
            {
                if(point.GetPosition().DistanceTo(newConnection.GetPosition()) < maxConnectionDistance && point != newConnection)
                {
                    Line2D newLine = new Line2D();
                    newLine.AddPoint(point.GetPosition());
                    newLine.AddPoint(newConnection.GetPosition());
                    newLine.Width = 2;
                    newLine.DefaultColor = new Color(0.8f, 0.8f, 0.8f);
                    AddChild(newLine);
                }
            }
/*            while (newConnection == point || point.GetPosition().DistanceTo(newConnection.GetPosition()) > 60)
            {
                newConnection = pointsPassed[GD.RandRange(1, pointsPassed.Count) - 1];
            }*/


        }
    }
}

