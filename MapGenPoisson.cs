using Godot;
using System;
using System.Collections.Generic;

public partial class MapGenPoisson : Node2D
{


    // public class Point
    // {
    //     public Point() { }

    //     private Vector2 position;
    //     private int numOfConnections;

    //     public Vector2 GetPosition() { return position; }
    //     public void SetPosition(Vector2 newPos) { position = newPos; }
    //     public int GetNumConnections() { return numOfConnections; }
    // }


    [Export]
    public Polygon2D mapShape;



    [Export]
    private float poisson_radius = 20;

    private PackedScene PackedPoint = GD.Load<PackedScene>("res://Assets/Objects/Point.tscn");

    [Export] Vector2 pointScale = new (0.35f,0.35f);

    public override void _Ready()
    {
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
            newPoint.SetPosition(vector);
            pointsFilled.Add(newPoint);

            newPoint.Position = newPoint.GetPosition();

            newPoint.Scale = pointScale;
            AddChild(newPoint);
        }

        return pointsFilled;
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
