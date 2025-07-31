using Godot;
using System;
using System.Collections.Generic;
using System.Drawing;

public partial class MapGenerator : Node
{

    // class Point
    // {
    //     public Point() { }

    //     private Vector2 position;
    //     private int numOfConnections;

    //     public Vector2 GetPosition() { return position; }
    //     public void SetPosition(Vector2 newPos) { position = newPos; }
    //     public int GetNumConnections() { return numOfConnections; }
    // }


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
    private PackedScene PackedPoint;


    public override void _Ready()
    {
        PackedPoint = GD.Load<PackedScene>("res://Assets/Objects/Point.tscn");
        GD.Print(PackedPoint);
        // Points copy = seed.Instantiate<Points>();

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
            pointsFilled.Add(newPoint);
            newPoint.Position = newPoint.GetPosition();

            GD.Print(newPoint.GetPosition());
            newPoint.Scale = new(.35f,.35f);
            this.AddChild(newPoint);

        }

        return pointsFilled;
    }

    void GenerateStreets(List<Points> pointsPassed)
    {
        GD.Print(pointsPassed.Count);
        foreach (var point in pointsPassed)
        {
            GD.Print("Generating Streets");

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
/*            while (newConnection == point || point.GetPosition().DistanceTo(newConnection.GetPosition()) > 60)
            {
                newConnection = pointsPassed[GD.RandRange(1, pointsPassed.Count) - 1];
            }*/


        }
    }
}

