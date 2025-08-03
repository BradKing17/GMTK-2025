using Godot;
using System.Collections.Generic;

/// <summary>
/// Main map generation coordinator - handles the overall process
/// </summary>
public partial class MapGenerator : Node2D
{
    [Export] public Polygon2D mapShape;
    [Export] private float poissonRadius = 20;
    [Export] private float maxConnectionDistance = 50;
    [Export] private Vector2 pointScale = new(0.35f, 0.35f);
    
    private PointFactory pointFactory;
    private ConnectionManager connectionManager;
    private RevealationManager revealationManager;
    private PointManager pointManager;
    
    public override void _Ready()
    {
        InitializeManagers();
        GenerateMap();
        StartProggressiveReveal();
    }
    
    private void InitializeManagers()
    {
        pointManager = new PointManager();
        AddChild(pointManager);
        
        pointFactory = new PointFactory(pointManager);
        connectionManager = new ConnectionManager(maxConnectionDistance);
        revealationManager = new RevealationManager();
        
        AddChild(revealationManager);
    }
    
    private void GenerateMap()
    {
        var positions = GeneratePositions();
        var points = pointFactory.CreateAllPoints(positions, pointScale);
        var connections = connectionManager.GenerateConnections(points);
        
        revealationManager.Initialize(points, connections);
    }
    
    private void StartProggressiveReveal()
    {
        revealationManager.StartReveal(initialNodesCount: 4, delayBetweenNodes: 10.0f);
    }
    
    private Vector2[] GeneratePositions()
    {
        var poissonScript = GD.Load<GDScript>("res://addons/PoissonDiscSampling/poisson_disc_sampling.gd");
        var poissonScriptNode = (GodotObject)poissonScript.New();
        return (Vector2[])poissonScriptNode.Call("generate_points_for_polygon", mapShape.Polygon, poissonRadius, 10);
    }
}
