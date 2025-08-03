using Godot;
using System.Collections.Generic;

/// <summary>
/// Factory for creating different types of points with proper configuration
/// </summary>
public class PointFactory
{
    private readonly PointManager pointManager;
    private readonly PointTypeWeights weights;
    
    public PointFactory(PointManager pointManager)
    {
        this.pointManager = pointManager;
        this.weights = new PointTypeWeights();
    }
    
    public List<Points> CreateAllPoints(Vector2[] positions, Vector2 scale)
    {
        var allPoints = new List<Points>();
        var weightedTypes = weights.GetWeightedTypeList();
        
        for (int i = 0; i < positions.Length; i++)
        {
            var pointType = DeterminePointType(i, weightedTypes);
            var point = CreatePoint(pointType, positions[i], scale);
            
            allPoints.Add(point);
            pointManager.RegisterPoint(point);
        }
        
        return allPoints;
    }
    
    private string DeterminePointType(int index, List<string> weightedTypes)
    {
        // First point is always Post Office
        if (index == 0) return "PostOffice";
        
        return weightedTypes[GD.RandRange(0, weightedTypes.Count - 1)];
    }
    
    private Points CreatePoint(string type, Vector2 position, Vector2 scale)
    {
        Points point = type switch
        {
            "GranniesHouse" => new GranniesHouse(),
            "PostOffice" => new PostOffice(),
            "ParkBench" => new ParkBench(),
            "PostBox" => new PostBox(),
            "PostDepot" => new PostDepot(),
            "WaterFountain" => new WaterFountain(),
            _ => new House(),
        };
        
        ConfigurePoint(point, position, scale);
        return point;
    }
    
    private void ConfigurePoint(Points point, Vector2 position, Vector2 scale)
    {
        point.Name = Utitily.RandomName.returnJsonNames(
            "res://Scripts/Map Gen/Names/List.json", 
            ["Prefixes", "Suffixes"]);
        point.SetPosition(position);
        point.Position = point.GetPosition();
        point.Scale = scale;
        point.manager = pointManager;
        point.Visible = false;
    }
}

/// <summary>
/// Manages point type weights and selection
/// </summary>
public class PointTypeWeights
{
    private readonly Dictionary<string, float> weights = new()
    {
        { "House", 40.0f },
        { "PostBox", 15.0f },
        { "ParkBench", 12.0f },
        { "WaterFountain", 10.0f },
        { "PostDepot", 5.0f },
        { "GranniesHouse", 2.0f }
    };
    
    public List<string> GetWeightedTypeList()
    {
        var weightedTypes = new List<string>();
        
        foreach (var kvp in weights)
        {
            int weight = Mathf.RoundToInt(kvp.Value);
            for (int i = 0; i < weight; i++)
            {
                weightedTypes.Add(kvp.Key);
            }
        }
        
        return weightedTypes;
    }
}
