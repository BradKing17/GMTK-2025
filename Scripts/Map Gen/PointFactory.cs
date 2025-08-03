using Godot;
using System.Collections.Generic;

/// <summary>
/// Factory for creating different types of points with proper configuration
/// </summary>
public class PointFactory
{
    private readonly PointManager pointManager;
    private PointTypeWeightsConfig weights;
    
    public PointFactory(PointManager pointManager)
    {
        this.pointManager = pointManager;
        this.weights = new PointTypeWeightsConfig(); // Default weights
    }

    public void SetWeights(PointTypeWeightsConfig customWeights)
    {
        this.weights = customWeights;
    }
    
    public List<Points> CreateAllPoints(Vector2[] positions, Vector2 scale)
    {
        var allPoints = new List<Points>();
        var weightedTypes = GetWeightedTypeList();
        
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
            MapGenConstants.FilePaths.NAMES_JSON, 
            MapGenConstants.JsonKeys.NAME_CATEGORIES);
        point.SetPosition(position);
        point.Position = point.GetPosition();
        point.Scale = scale;
        point.manager = pointManager;
        point.Visible = false;
    }

    private List<string> GetWeightedTypeList()
    {
        var weightedTypes = new List<string>();
        var weightDict = new Dictionary<string, float>
        {
            { "House", weights.House },
            { "PostBox", weights.PostBox },
            { "ParkBench", weights.ParkBench },
            { "WaterFountain", weights.WaterFountain },
            { "PostDepot", weights.PostDepot },
            { "GranniesHouse", weights.GranniesHouse }
        };
        
        foreach (var kvp in weightDict)
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
