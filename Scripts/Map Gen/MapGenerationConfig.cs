using Godot;

/// <summary>
/// Centralized configuration for map generation parameters
/// </summary>
[System.Serializable]
public partial class MapGenerationConfig : Resource
{
    [Export] public float PoissonRadius { get; set; } = 20f;
    [Export] public float MaxConnectionDistance { get; set; } = 50f;
    [Export] public Vector2 PointScale { get; set; } = new(0.35f, 0.35f);
    [Export] public float PointRadius { get; set; } = 20f;
    
    [Export] public int InitialNodesCount { get; set; } = 4;
    [Export] public float DelayBetweenNodes { get; set; } = 10.0f;
    
    [Export] public PointTypeWeightsConfig PointWeights { get; set; } = new();
}

[System.Serializable]
public partial class PointTypeWeightsConfig : Resource
{
    [Export] public float House { get; set; } = 40.0f;
    [Export] public float PostBox { get; set; } = 15.0f;
    [Export] public float ParkBench { get; set; } = 12.0f;
    [Export] public float WaterFountain { get; set; } = 10.0f;
    [Export] public float PostOffice { get; set; } = 8.0f;
    [Export] public float PostDepot { get; set; } = 5.0f;
    [Export] public float GranniesHouse { get; set; } = 2.0f;
}
