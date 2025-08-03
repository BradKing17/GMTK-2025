using Godot;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Manages connections between points and visual line representation
/// </summary>
public class ConnectionManager
{
    private readonly float maxConnectionDistance;
    
    public ConnectionManager(float maxConnectionDistance)
    {
        this.maxConnectionDistance = maxConnectionDistance;
    }
    
    public List<Line2D> GenerateConnections(List<Points> points)
    {
        CreateNeighborConnections(points);
        RemoveDeadEnds(points);
        return CreateVisualLines(points);
    }
    
    private void CreateNeighborConnections(List<Points> points)
    {
        foreach (var point in points)
        {
            foreach (var otherPoint in points)
            {
                if (ShouldConnect(point, otherPoint))
                {
                    point.AddNeighbour(otherPoint);
                    otherPoint.AddNeighbour(point);
                }
            }
        }
    }
    
    private bool ShouldConnect(Points point1, Points point2)
    {
        return point1 != point2 && 
               point1.GetPosition().DistanceTo(point2.GetPosition()) < maxConnectionDistance && 
               !point1.GetNeighbours().Contains(point2);
    }
    
    private void RemoveDeadEnds(List<Points> points)
    {
        var deadEnds = points.Where(p => p.GetNeighbours().Count < 2).ToList();
        
        foreach (var deadPoint in deadEnds)
        {
            RemovePointConnections(deadPoint, points);
            points.Remove(deadPoint);
            deadPoint.QueueFree();
        }
    }
    
    private void RemovePointConnections(Points deadPoint, List<Points> allPoints)
    {
        foreach (var point in allPoints)
        {
            if (point.GetNeighbours().Contains(deadPoint))
            {
                point.RemoveNeighbour(deadPoint);
            }
        }
    }
    
    private List<Line2D> CreateVisualLines(List<Points> points)
    {
        var lines = new List<Line2D>();
        
        foreach (var point in points)
        {
            foreach (var neighbor in point.GetNeighbours())
            {
                // Only create line if this point's ID is smaller to avoid duplicates
                if (point.GetInstanceId() < neighbor.GetInstanceId())
                {
                    var line = CreateLine(point.GetPosition(), neighbor.GetPosition());
                    lines.Add(line);
                }
            }
        }
        
        return lines;
    }
    
    private Line2D CreateLine(Vector2 start, Vector2 end)
    {
        var line = new Line2D();
        line.AddPoint(start);
        line.AddPoint(end);
        line.Width = 2;
        line.DefaultColor = new Color(0.8f, 0.8f, 0.8f);
        line.Visible = false;
        return line;
    }
}
