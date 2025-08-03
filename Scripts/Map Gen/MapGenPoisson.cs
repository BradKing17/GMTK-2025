using Godot;
using System.Collections.Generic;

/// <summary>
/// Refactored map generation using cleaner architecture
/// Maintains same interface for existing scene compatibility
/// </summary>
public partial class MapGenPoisson : Node2D
{
    //Point Shape Variables 
    [Export] public Polygon2D mapShape;
    [Export] private float poisson_radius = 20;
    [Export] private float maxConnectionDistance = 50;
    [Export] Vector2 pointScale = new(0.35f, 0.35f);
    [Export] private float pointRadius = 20;

    //Point Weighting Variables
    [Export] private float house = 40.0f; // Most common
    [Export] private float postBox = 15.0f; // Common
    [Export] private float parkBench = 12.0f; // Fairly common
    [Export] private float waterFountain = 10.0f; // Moderate
    [Export] private float postOffice = 8.0f; // Less common
    [Export] private float postDepot = 5.0f; // Rare
    [Export] private float granniesHouse = 2.0f; // Very rare

    // Revelation Settings
    [Export] private int initialNodesCount = 4;
    [Export] private float delayBetweenNodes = 10.0f;

    // Refactored components
    private PointFactory pointFactory;
    private ConnectionManager connectionManager;
    private RevealationManager revealationManager;
    private PointManager pointManager;

    public override void _Ready()
    {
        InitializeManagers();
        GenerateMap();
        StartProgressiveReveal();
    }

    private void InitializeManagers()
    {
        pointManager = new PointManager();
        AddChild(pointManager);
        
        // Create point factory with custom weights
        var weights = CreateWeightsConfig();
        pointFactory = new PointFactory(pointManager);
        pointFactory.SetWeights(weights);
        
        connectionManager = new ConnectionManager(maxConnectionDistance);
        revealationManager = new RevealationManager();
        
        AddChild(revealationManager);
    }

    private PointTypeWeightsConfig CreateWeightsConfig()
    {
        return new PointTypeWeightsConfig
        {
            House = house,
            PostBox = postBox,
            ParkBench = parkBench,
            WaterFountain = waterFountain,
            PostOffice = postOffice,
            PostDepot = postDepot,
            GranniesHouse = granniesHouse
        };
    }

    private void GenerateMap()
    {
        var positions = GeneratePositions();
        var points = pointFactory.CreateAllPoints(positions, pointScale);
        
        // Add points to scene tree
        foreach (var point in points)
        {
            AddChild(point);
        }
        
        var connections = connectionManager.GenerateConnections(points);
        
        // Add connection lines to scene tree
        foreach (var line in connections)
        {
            AddChild(line);
        }
        
        revealationManager.Initialize(points, connections);
    }

    private void StartProgressiveReveal()
    {
        revealationManager.StartReveal(initialNodesCount, delayBetweenNodes);
    }

    private Vector2[] GeneratePositions()
    {
        var poissonScript = GD.Load<GDScript>(MapGenConstants.FilePaths.POISSON_SAMPLING_SCRIPT);
        var poissonScriptNode = (GodotObject)poissonScript.New();
        return (Vector2[])poissonScriptNode.Call("generate_points_for_polygon", mapShape.Polygon, poisson_radius, 10);
    }
}
