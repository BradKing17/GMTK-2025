using Godot;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Extension methods for common map generation operations
/// </summary>
public static class MapGenExtensions
{
    /// <summary>
    /// Checks if a line connects to a specific point within tolerance
    /// </summary>
    public static bool ConnectsToPoint(this Line2D line, Vector2 point, float tolerance = MapGenConstants.POSITION_TOLERANCE)
    {
        return (line.Points[0] - point).Length() < tolerance ||
               (line.Points[1] - point).Length() < tolerance;
    }
    
    /// <summary>
    /// Checks if a line connects to any point in a collection
    /// </summary>
    public static bool ConnectsToAnyPoint(this Line2D line, IEnumerable<Points> points, float tolerance = MapGenConstants.POSITION_TOLERANCE)
    {
        return points.Any(point => line.ConnectsToPoint(point.GetPosition(), tolerance));
    }
    
    /// <summary>
    /// Gets all hidden lines from a collection
    /// </summary>
    public static IEnumerable<Line2D> GetHiddenLines(this IEnumerable<Line2D> lines)
    {
        return lines.Where(line => !line.Visible);
    }
    
    /// <summary>
    /// Gets all visible points from a collection
    /// </summary>
    public static IEnumerable<Points> GetVisiblePoints(this IEnumerable<Points> points)
    {
        return points.Where(point => point.Visible);
    }
    
    /// <summary>
    /// Gets all hidden points from a collection
    /// </summary>
    public static IEnumerable<Points> GetHiddenPoints(this IEnumerable<Points> points)
    {
        return points.Where(point => !point.Visible);
    }
}
