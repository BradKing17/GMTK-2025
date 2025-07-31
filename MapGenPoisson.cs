using Godot;
using System;
using System.Collections.Generic;

public partial class MapGenPoisson : Node2D
{


    public class Point
    {
        public Point() { }

        private Vector2 position;
        private int numOfConnections;

        public Vector2 GetPosition() { return position; }
        public void SetPosition(Vector2 newPos) { position = newPos; }
        public int GetNumConnections() { return numOfConnections; }
    }


    [Export]
    public Polygon2D mapShape;



    [Export]
    private float poisson_radius = 20;

    [Export]
    PackedScene pointMarker = ResourceLoader.Load<PackedScene>("res://Marker.tscn");

    public override void _Ready()
    {
        List<Point> points = new List<Point>();
        points = GeneratePoints();
        GenerateStreets(points);
    }

    List<Point> GeneratePoints()
    {
        var poissonScript = GD.Load<GDScript>("res://addons/PoissonDiscSampling/poisson_disc_sampling.gd");
        var poissonScriptNode = (GodotObject)poissonScript.New();
        Vector2[] newVectors = (Vector2[])poissonScriptNode.Call("generate_points_for_polygon", mapShape.Polygon, poisson_radius, 10); ;

        List<Point> pointsFilled = new List<Point>();

        foreach (var vector in newVectors)
        {

            Point newPoint = new Point();
            newPoint.SetPosition(vector);
            pointsFilled.Add(newPoint);


            Node2D pointInstance = (Node2D)pointMarker.Instantiate();
            pointInstance.Position = newPoint.GetPosition();
            AddChild(pointInstance);
        }
        return pointsFilled;
    }
    void GenerateStreets(List<Point> pointsPassed)
    {
        GD.Print("Generating Streets with: " + pointsPassed.Count + " Points");
        foreach (var point in pointsPassed)
        {

            foreach (Point newConnection in pointsPassed)
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
            /*            while (newConnection == point || point.GetPosition().DistanceTo(newConnection.GetPosition()) > 60)
                        {
                            newConnection = pointsPassed[GD.RandRange(1, pointsPassed.Count) - 1];
                        }*/


        }
    }
}
