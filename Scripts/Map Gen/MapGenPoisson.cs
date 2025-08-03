using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
public partial class MapGenPoisson : Node2D
{
    //Point Shape Variables 
    [Export] public Polygon2D mapShape;
    [Export] private float poisson_radius = 20;
    [Export] private float maxConnectionDistance = 50;
    [Export] Vector2 pointScale = new (0.35f,0.35f);
    [Export] private float pointRadius = 20;

    //Point Weighting Variables
    [Export] private float house = 40.0f; // Most common
    [Export] private float postBox = 15.0f; // Common
    [Export] private float parkBench = 12.0f; // Fairly common
    [Export] private float waterFountain = 10.0f; // Moderate
    [Export] private float postOffice = 8.0f; // Less common
    [Export] private float postDepot = 5.0f; // Rare
    [Export] private float granniesHouse = 2.0f; // Very rare
    private PackedScene PackedPoint = GD.Load<PackedScene>("res://Assets/Objects/Point.tscn");
    private PointManager pointManager;
    
    public override void _Ready()
    {
        pointManager = new PointManager();
        AddChild(pointManager);
        List<Points> points = new(GeneratePoints());

        GenerateStreets(points);
    }

    List<Points> GeneratePoints()
    {
        var poissonScript = GD.Load<GDScript>("res://addons/PoissonDiscSampling/poisson_disc_sampling.gd");
        var poissonScriptNode = (GodotObject)poissonScript.New();
        Vector2[] newVectors = (Vector2[])poissonScriptNode.Call("generate_points_for_polygon", mapShape.Polygon, poisson_radius, 10);
        
        List<Points> pointsFilled = new();
        
        // Define weights for different point types
        var pointTypes = new Godot.Collections.Dictionary<string, float>
        {
            { "House", house },
            { "PostBox", postBox },
            { "ParkBench", parkBench },
            { "WaterFountain", waterFountain },
            // { "PostOffice", postOffice },
            { "PostDepot", postDepot },
            { "GranniesHouse", granniesHouse }
        };
        
        var weightedPointTypes = new List<string>();
        foreach (var kvp in pointTypes)
        {
            int weight = Mathf.RoundToInt(kvp.Value);
            for (int i = 0; i < weight; i++)
            {
                weightedPointTypes.Add(kvp.Key);
            }
        }

        bool isFirstPoint = true;
        foreach (var vector in newVectors)
        {
            string selectedType = isFirstPoint ? "PostOffice" : weightedPointTypes[GD.RandRange(0, weightedPointTypes.Count - 1)];
            isFirstPoint = false;

            Points newPoint = selectedType switch // didnt realise you could write switches like this
            {
                "GranniesHouse" => new GranniesHouse(),
                "PostOffice" => new PostOffice(),
                "ParkBench" => new ParkBench(),
                "PostBox" => new PostBox(),
                "PostDepot" => new PostDepot(),
                "WaterFountain" => new WaterFountain(),
                _ => new House(),
            };

            // probably shouldnt be opening the json every time we make a new node but idc!!
            newPoint.Name = Utitily.RandomName.returnJsonNames("res://Scripts/Map Gen/Names/List.json", ["Prefixes", "Suffixes"]); 
            AddChild(newPoint);
            newPoint.SetPosition(vector);
            pointsFilled.Add(newPoint);
            pointManager.RegisterPoint(newPoint); // Register the point with the PointManager
            newPoint.Position = newPoint.GetPosition();
            newPoint.Scale = pointScale;
            newPoint.manager = pointManager;
        }

        return pointsFilled;
    }


    void GenerateStreets(List<Points> pointsPassed)
    {
        List<Points> deadEnds = new();

        foreach (Points point in pointsPassed)
        {
            foreach (Points newConnection in pointsPassed)
            {
                if (point.GetPosition().DistanceTo(newConnection.GetPosition()) < maxConnectionDistance && point != newConnection && !point.GetNeighbours().Contains(newConnection))
                {
                    point.AddNeighbour(newConnection);
                    newConnection.AddNeighbour(point);
                }
            }
        }
        foreach(Points point in pointsPassed)
        {
            if (point.GetNeighbours().Count < 2)
            {
                deadEnds.Add(point);
            }
        }
        foreach(Points deadPoint in deadEnds)
        {
            foreach(Points point in pointsPassed)
            {
                if (point.GetNeighbours().Contains(deadPoint))
                {
                    point.RemoveNeighbour(deadPoint); 
                }
            }
            pointManager.DeregisterPoint(deadPoint);
            pointsPassed.Remove(deadPoint);
            deadPoint.QueueFree();
        }
        foreach (Points point in pointsPassed)
        {
           // GD.Print("Num of connections: " + point.GetNeighbours().Count);

            for (int i = 0; i < point.GetNeighbours().Count; i++)
            {
                    Line2D newLine = new Line2D();
                    newLine.AddPoint(point.GetPosition());
                    newLine.AddPoint(point.GetNeighbours()[i].GetPosition());
                    newLine.Width = 2;
                    newLine.DefaultColor = new Color(0.8f, 0.8f, 0.8f);
                    AddChild(newLine);
             }
        }
    }
}
