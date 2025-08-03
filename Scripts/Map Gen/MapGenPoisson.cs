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
    private Vector2[] vectorsToGenerate;
    private List<Points> allPoints = new(); // All points created in advance
    private List<Points> visiblePoints = new(); // Points that are currently visible
    private List<Line2D> allLines = new(); // All lines created in advance
    private int currentPointIndex = 0;
    private Timer nodeGenerationTimer;
    
    public override void _Ready()
    {
        pointManager = new PointManager();
        AddChild(pointManager);
        
        // Generate all positions and create all points in advance
        GenerateAllPointsAndConnections();
        
        // Set up timer for progressive node revelation
        nodeGenerationTimer = new Timer();
        nodeGenerationTimer.WaitTime = 10.0f; // 10 second delay
        nodeGenerationTimer.Timeout += OnNodeRevealTimeout;
        AddChild(nodeGenerationTimer);
        
        // Reveal the first node (Post Office) immediately
        RevealNextNode();
        
        // Start the timer for subsequent nodes
        if (visiblePoints.Count < allPoints.Count)
        {
            nodeGenerationTimer.Start();
        }
    }

    void GenerateAllPointsAndConnections()
    {
        // Generate all positions using Poisson disc sampling
        var poissonScript = GD.Load<GDScript>("res://addons/PoissonDiscSampling/poisson_disc_sampling.gd");
        var poissonScriptNode = (GodotObject)poissonScript.New();
        vectorsToGenerate = (Vector2[])poissonScriptNode.Call("generate_points_for_polygon", mapShape.Polygon, poisson_radius, 10);
        
        // Create all points in advance but don't make them visible
        CreateAllPoints();
        
        // Generate all connections and lines
        GenerateAllConnections();
        
        // Clean up dead ends
        CleanUpDeadEnds();
    }

    void CreateAllPoints()
    {
        // Define weights for different point types
        var pointTypes = new Godot.Collections.Dictionary<string, float>
        {
            { "House", house },
            { "PostBox", postBox },
            { "ParkBench", parkBench },
            { "WaterFountain", waterFountain },
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

        for (int i = 0; i < vectorsToGenerate.Length; i++)
        {
            Vector2 vector = vectorsToGenerate[i];
            bool isFirstPoint = i == 0;
            string selectedType = isFirstPoint ? "PostOffice" : weightedPointTypes[GD.RandRange(0, weightedPointTypes.Count - 1)];

            Points newPoint = selectedType switch
            {
                "GranniesHouse" => new GranniesHouse(),
                "PostOffice" => new PostOffice(),
                "ParkBench" => new ParkBench(),
                "PostBox" => new PostBox(),
                "PostDepot" => new PostDepot(),
                "WaterFountain" => new WaterFountain(),
                _ => new House(),
            };

            newPoint.Name = Utitily.RandomName.returnJsonNames("res://Scripts/Map Gen/Names/List.json", ["Prefixes", "Suffixes"]);
            newPoint.SetPosition(vector);
            newPoint.Position = newPoint.GetPosition();
            newPoint.Scale = pointScale;
            newPoint.manager = pointManager;
            newPoint.Visible = false; // Hide initially
            
            AddChild(newPoint);
            allPoints.Add(newPoint);
            pointManager.RegisterPoint(newPoint);
        }
    }

    void GenerateAllConnections()
    {
        // Generate connections between all points
        foreach (Points point in allPoints)
        {
            foreach (Points otherPoint in allPoints)
            {
                if (point != otherPoint && 
                    point.GetPosition().DistanceTo(otherPoint.GetPosition()) < maxConnectionDistance && 
                    !point.GetNeighbours().Contains(otherPoint))
                {
                    point.AddNeighbour(otherPoint);
                    otherPoint.AddNeighbour(point);
                }
            }
        }

        // Create all visual lines but hide them initially
        foreach (Points point in allPoints)
        {
            for (int i = 0; i < point.GetNeighbours().Count; i++)
            {
                // Only create line if this point's ID is smaller to avoid duplicates
                if (point.GetInstanceId() < point.GetNeighbours()[i].GetInstanceId())
                {
                    Line2D newLine = new Line2D();
                    newLine.AddPoint(point.GetPosition());
                    newLine.AddPoint(point.GetNeighbours()[i].GetPosition());
                    newLine.Width = 2;
                    newLine.DefaultColor = new Color(0.8f, 0.8f, 0.8f);
                    newLine.Visible = false; // Hide initially
                    AddChild(newLine);
                    allLines.Add(newLine);
                }
            }
        }
    }

    void CleanUpDeadEnds()
    {
        List<Points> deadEnds = new();
        
        foreach(Points point in allPoints)
        {
            if (point.GetNeighbours().Count < 2)
            {
                deadEnds.Add(point);
            }
        }
        
        foreach(Points deadPoint in deadEnds)
        {
            foreach(Points point in allPoints)
            {
                if (point.GetNeighbours().Contains(deadPoint))
                {
                    point.RemoveNeighbour(deadPoint); 
                }
            }
            pointManager.DeregisterPoint(deadPoint);
            allPoints.Remove(deadPoint);
            deadPoint.QueueFree();
        }
        
        // Remove lines connected to dead end points
        var linesToRemove = new List<Line2D>();
        foreach (Line2D line in allLines)
        {
            bool lineConnectsToDeadEnd = false;
            foreach (Points deadPoint in deadEnds)
            {
                if ((line.Points[0] - deadPoint.GetPosition()).Length() < 1.0f || 
                    (line.Points[1] - deadPoint.GetPosition()).Length() < 1.0f)
                {
                    lineConnectsToDeadEnd = true;
                    break;
                }
            }
            if (lineConnectsToDeadEnd)
            {
                linesToRemove.Add(line);
            }
        }
        
        foreach (Line2D line in linesToRemove)
        {
            allLines.Remove(line);
            line.QueueFree();
        }
    }

    void OnNodeRevealTimeout()
    {
        int visibleCountBefore = visiblePoints.Count;
        RevealNextNode();
        
        // Continue timer if there are more nodes to reveal and we successfully revealed one
        if (visiblePoints.Count < allPoints.Count)
        {
            // If we didn't reveal a node this time, but there are still hidden nodes,
            // it means all remaining nodes are isolated (no connections to visible network)
            if (visiblePoints.Count == visibleCountBefore && visiblePoints.Count > 0)
            {
                // Force reveal the next available node to potentially connect isolated clusters
                for (int i = 0; i < allPoints.Count; i++)
                {
                    if (!allPoints[i].Visible)
                    {
                        allPoints[i].Visible = true;
                        visiblePoints.Add(allPoints[i]);
                        
                        // Reveal any lines connecting to this newly visible node
                        foreach (Line2D line in allLines)
                        {
                            if (!line.Visible)
                            {
                                bool connectsToNewNode = (line.Points[0] - allPoints[i].GetPosition()).Length() < 1.0f ||
                                                       (line.Points[1] - allPoints[i].GetPosition()).Length() < 1.0f;
                                
                                if (connectsToNewNode)
                                {
                                    bool otherEndVisible = false;
                                    foreach (Points visiblePoint in visiblePoints)
                                    {
                                        if (visiblePoint != allPoints[i] &&
                                            ((line.Points[0] - visiblePoint.GetPosition()).Length() < 1.0f ||
                                             (line.Points[1] - visiblePoint.GetPosition()).Length() < 1.0f))
                                        {
                                            otherEndVisible = true;
                                            break;
                                        }
                                    }
                                    
                                    if (otherEndVisible)
                                    {
                                        line.Visible = true;
                                    }
                                }
                            }
                        }
                        break;
                    }
                }
            }
            
            nodeGenerationTimer.Start();
        }
    }

    void RevealNextNode()
    {
        if (currentPointIndex >= allPoints.Count)
            return;

        Points nodeToReveal = null;
        
        // If this is the first node, reveal it (Post Office)
        if (visiblePoints.Count == 0)
        {
            nodeToReveal = allPoints[0]; // First node is always Post Office
        }
        else
        {
            // Find the next node that has at least one connection to a visible node
            for (int i = 0; i < allPoints.Count; i++)
            {
                Points candidateNode = allPoints[i];
                
                // Skip if already visible
                if (candidateNode.Visible)
                    continue;
                
                // Check if this node connects to any visible node
                bool hasVisibleConnection = false;
                foreach (Points neighbor in candidateNode.GetNeighbours())
                {
                    if (neighbor.Visible)
                    {
                        hasVisibleConnection = true;
                        break;
                    }
                }
                
                if (hasVisibleConnection)
                {
                    nodeToReveal = candidateNode;
                    break;
                }
            }
        }
        
        // If no suitable node found, try again later
        if (nodeToReveal == null)
        {
            return;
        }
        
        // Reveal the selected node
        nodeToReveal.Visible = true;
        visiblePoints.Add(nodeToReveal);

        // Reveal lines that connect this new node to visible points
        foreach (Line2D line in allLines)
        {
            if (!line.Visible)
            {
                bool connectsToNewNode = false;
                bool connectsToVisibleNode = false;
                
                // Check if line connects to the newly revealed node
                if ((line.Points[0] - nodeToReveal.GetPosition()).Length() < 1.0f ||
                    (line.Points[1] - nodeToReveal.GetPosition()).Length() < 1.0f)
                {
                    connectsToNewNode = true;
                }
                
                // Check if the other end connects to a visible node
                if (connectsToNewNode)
                {
                    foreach (Points visiblePoint in visiblePoints)
                    {
                        if (visiblePoint != nodeToReveal &&
                            ((line.Points[0] - visiblePoint.GetPosition()).Length() < 1.0f ||
                             (line.Points[1] - visiblePoint.GetPosition()).Length() < 1.0f))
                        {
                            connectsToVisibleNode = true;
                            break;
                        }
                    }
                }
                
                if (connectsToNewNode && connectsToVisibleNode)
                {
                    line.Visible = true;
                }
            }
        }

        currentPointIndex++;
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
