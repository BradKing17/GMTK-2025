using Godot;

/// <summary>
/// Constants used throughout map generation
/// </summary>
public static class MapGenConstants
{
    public const float POSITION_TOLERANCE = 1.0f;
    public const int MIN_NEIGHBORS_FOR_VIABILITY = 2;
    public const float DEFAULT_LINE_WIDTH = 2f;
    
    public static readonly Color DEFAULT_LINE_COLOR = new(0.8f, 0.8f, 0.8f);
    
    public static class FilePaths
    {
        public const string POISSON_SAMPLING_SCRIPT = "res://addons/PoissonDiscSampling/poisson_disc_sampling.gd";
        public const string POINT_SCENE = "res://Assets/Objects/Point.tscn";
        public const string NAMES_JSON = "res://Scripts/Map Gen/Names/List.json";
    }
    
    public static class JsonKeys
    {
        public static readonly string[] NAME_CATEGORIES = { "Prefixes", "Suffixes" };
    }
}
