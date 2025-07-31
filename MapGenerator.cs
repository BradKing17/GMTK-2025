using Godot;
using System;
using System.Collections.Generic;

public partial class MapGenerator : Node
{

    class Point
    {
        public Point() { }

        private Vector2 position;
        private int numOfConnections;

        public Vector2 GetPosition() { return position; }
        public void SetPosition(Vector2 newPos) { position = newPos; }
        public int GetNumConnections() { return numOfConnections; }
    }


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
            
            newPoint.SetPosition(new Vector2(GD.RandRange(0, mapSizeX),(GD.RandRange(0, mapSizeY))));
            
            pointsFilled.Add(newPoint);

            Node2D pointInstance = (Node2D)pointMarker.Instantiate();
            pointInstance.Position = newPoint.GetPosition();
         //   GD.Print(pointInstance.Position);
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

            var newConnection = pointsPassed[GD.RandRange(1, pointsPassed.Count) - 1];

            while (newConnection == point || point.GetPosition().DistanceTo(newConnection.GetPosition()) > 200)
            {
                newConnection = pointsPassed[GD.RandRange(1, pointsPassed.Count) - 1];
            }

            Line2D newLine = new Line2D();
            newLine.AddPoint(point.GetPosition());
            newLine.AddPoint(newConnection.GetPosition());
            newLine.Width = 2;
            newLine.DefaultColor = new Color(0.8f, 0.8f, 0.8f);
            AddChild(newLine);
        }
    }
}

