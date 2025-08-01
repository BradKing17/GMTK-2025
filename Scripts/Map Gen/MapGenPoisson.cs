using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
public partial class MapGenPoisson : Node2D
{
    [Export]
    public Polygon2D mapShape;

    [Export]
    private float poisson_radius = 20;

    [Export]
    private float maxConnectionDistance = 50;

    [Export] 
    Vector2 pointScale = new (0.35f,0.35f);

    private PackedScene PackedPoint = GD.Load<PackedScene>("res://Assets/Objects/Point.tscn");

    private PointManager pointManager;
    [Export] private float pointRadius = 20;

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
        Vector2[] newVectors = (Vector2[])poissonScriptNode.Call("generate_points_for_polygon", mapShape.Polygon, poisson_radius, 10);

        var json = new Json { Data = Json.ParseString(Godot.FileAccess.GetFileAsString("res://Scripts/Map Gen/Names/List.json")) };
        Godot.Collections.Dictionary<string, string[]> nodeDict = new((Dictionary)json.Data);
        RandomNumberGenerator nameRNG = new();
        
        List<Points> pointsFilled = new();
        
        // Define weights for different point types (you can adjust these)
        string[] pointTypes = { "House", "PostOffice", "ParkBench", "PostBox", "PostDepot", "WaterFountain", "GranniesHouse" };

        foreach (var vector in newVectors)
        {
            Points newPoint = PackedPoint.Instantiate<Points>();
            
            // Add visual representation for the point
            var square = new ColorRect();
            square.Size = new Vector2(20, 20);
            square.Position = new Vector2(-10, -10); // Center it
            newPoint.AddChild(square);
            
            // Randomly select a point type
            string selectedType = pointTypes[GD.RandRange(0, pointTypes.Length - 1)];
            
            // Set color based on point type
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
            newPoint.SetPointType(selectedType); // Set the point type

            // Add type-specific behavior based on the selected type
            switch (selectedType)
            {
                case "House":
                    // Add timer and label for House types
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
                    
                    // Create a House component and attach it to the point
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
            newPoint.Name = nodeDict["Prefixes"][nameRNG.RandiRange(0, nodeDict["Prefixes"].Length-1)] + " " + nodeDict["Suffixes"][nameRNG.RandiRange(0, nodeDict["Suffixes"].Length-1)];
            GD.Print($"Generated {selectedType} at {newPoint.GetPosition()}");
            newPoint.Position = newPoint.GetPosition();
            newPoint.Scale = pointScale;

            AddChild(newPoint);
        }

        return pointsFilled;
    }

    // private void AddTimer(Points newPoint)
    // {
    //     Label timerLabel = new()
    //     {
    //         Text = "0",
    //         Position = new Vector2(0, -40)
    //     };
    //     newPoint.radius = pointRadius;
    //     newPoint.AddChild(timerLabel);

    //     newPoint.InitializeTimer(this, timerLabel); // Start the timer for this point and pass the label
    // }

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
