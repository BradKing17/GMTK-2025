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
    [Export] private Single house = 40.0f; // Most common
    [Export] private Single postBox = 15.0f; // Common
    [Export] private Single parkBench = 12.0f; // Fairly common
    [Export] private Single waterFountain = 10.0f; // Moderate
    [Export] private Single postOffice = 8.0f; // Less common
    [Export] private Single postDepot = 5.0f; // Rare
    [Export] private Single granniesHouse = 2.0f; // Very rare

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

        var json = new Json { Data = Json.ParseString(Godot.FileAccess.GetFileAsString("res://Scripts/Map Gen/Names/List.json")) };
        Godot.Collections.Dictionary<string, string[]> nodeDict = new((Dictionary)json.Data);
        RandomNumberGenerator nameRNG = new();
        
        List<Points> pointsFilled = new();
        
        // Define weights for different point types
        var pointTypes = new Godot.Collections.Dictionary<string, float>
        {
            { "House", house },
            { "PostBox", postBox },
            { "ParkBench", parkBench },
            { "WaterFountain", waterFountain },
            { "PostOffice", postOffice },
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
            Points newPoint = PackedPoint.Instantiate<Points>();
            newPoint.Name = nodeDict["Prefixes"][nameRNG.RandiRange(0, nodeDict["Prefixes"].Length-1)] + " " + nodeDict["Suffixes"][nameRNG.RandiRange(0, nodeDict["Suffixes"].Length-1)];

            AddChild(newPoint);

            // Add visual representation for the point
            var square = new ColorRect();
            square.Size = new Vector2(20, 20);
            square.Position = new Vector2(-10, -10); 
            newPoint.AddChild(square);

            // Ensure first point is always a PostOffice, then use weighted random selection
            string selectedType = isFirstPoint ? "PostOffice" : weightedPointTypes[GD.RandRange(0, weightedPointTypes.Count - 1)];
            isFirstPoint = false;

            Color pointColor = selectedType switch
            {
                "House" => new Color(0, 1, 0), // Green
                "GranniesHouse" => new Color(0, 0.5f, 1), // Light Blue
                "PostOffice" => new Color(1, 1, 0), // Yellow
                "ParkBench" => new Color(0.5f, 0.3f, 0), // Brown
                "PostBox" => new Color(1, 0, 0), // Red
                "PostDepot" => new Color(1, 0.5f, 0), // Orange
                "WaterFountain" => new Color(0, 0, 1), // Blue
                _ => new Color(1, 1, 1) // White default
            };
            square.Color = pointColor;

            newPoint.SetPosition(vector);
            newPoint.SetPointType(selectedType); 

            switch (selectedType)
            {
                case "House":
                    Label timerLabel = new()
                    {
                        Text = "0",
                        Position = new Vector2(0, -60),
                        LabelSettings = new LabelSettings()
                        {
                            FontSize = 40,

                        }

                    };
                    newPoint.AddChild(timerLabel);

                    House houseComponent = new House();
                    newPoint.AddChild(houseComponent);
                    houseComponent.InitializeTimer(this, timerLabel);
                    break;

                case "GranniesHouse":
                    // Add GranniesHouse-specific behavior here
                    break;

                case "PostOffice":
                    // Add PostOffice-specific behavior here
                    break;

                case "ParkBench":
                    // Add ParkBench-specific behavior here
                    break;

                case "PostBox":
                    // Add PostBox-specific behavior here
                    break;

                case "PostDepot":
                    // Add PostDepot-specific behavior here
                    break;

                case "WaterFountain":
                    // Add WaterFountain-specific behavior here
                    break;
            }

            pointsFilled.Add(newPoint);
            pointManager.RegisterPoint(newPoint); // Register the point with the PointManager
            GD.Print($"Generated {selectedType} at {newPoint.GetPosition()}");
            newPoint.Position = newPoint.GetPosition();
            newPoint.Scale = pointScale;
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
                if (point.GetPosition().DistanceTo(newConnection.GetPosition()) < maxConnectionDistance && point != newConnection)
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
