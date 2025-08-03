using Godot;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Manages the progressive revelation of map nodes and connections
/// </summary>
public partial class RevealationManager : Node
{
    private List<Points> allPoints;
    private List<Points> visiblePoints;
    private List<Line2D> allLines;
    private Timer revealTimer;
    
    public void Initialize(List<Points> points, List<Line2D> lines)
    {
        allPoints = points;
        allLines = lines;
        visiblePoints = new List<Points>();
        
        SetupTimer();
        
        // Add lines to scene tree
        foreach (var line in allLines)
        {
            GetParent().AddChild(line);
        }
    }
    
    public void StartReveal(int initialNodesCount, float delayBetweenNodes)
    {
        revealTimer.WaitTime = delayBetweenNodes;
        
        // Reveal initial nodes
        for (int i = 0; i < initialNodesCount && i < allPoints.Count; i++)
        {
            RevealNextNode();
        }
        
        // Start timer for remaining nodes
        if (visiblePoints.Count < allPoints.Count)
        {
            revealTimer.Start();
        }
    }
    
    private void SetupTimer()
    {
        revealTimer = new Timer();
        revealTimer.Timeout += OnRevealTimeout;
        AddChild(revealTimer);
    }
    
    private void OnRevealTimeout()
    {
        int visibleCountBefore = visiblePoints.Count;
        RevealNextNode();
        
        if (visiblePoints.Count < allPoints.Count)
        {
            // Handle isolated clusters
            if (visiblePoints.Count == visibleCountBefore && visiblePoints.Count > 0)
            {
                ForceRevealIsolatedNode();
            }
            
            revealTimer.Start();
        }
    }
    
    private void RevealNextNode()
    {
        var nodeToReveal = FindNextNodeToReveal();
        if (nodeToReveal == null) return;
        
        RevealNode(nodeToReveal);
        RevealConnectedLines(nodeToReveal);
    }
    
    private Points FindNextNodeToReveal()
    {
        // First node (Post Office)
        if (visiblePoints.Count == 0)
        {
            return allPoints.FirstOrDefault();
        }
        
        // Find node connected to visible network
        return allPoints.FirstOrDefault(node => 
            !node.Visible && 
            node.GetNeighbours().Any(neighbor => neighbor.Visible));
    }
    
    private void RevealNode(Points node)
    {
        node.Visible = true;
        visiblePoints.Add(node);
    }
    
    private void RevealConnectedLines(Points newNode)
    {
        foreach (var line in allLines.Where(l => !l.Visible))
        {
            if (LineConnectsToNode(line, newNode) && LineConnectsToVisibleNode(line, newNode))
            {
                line.Visible = true;
            }
        }
    }
    
    private bool LineConnectsToNode(Line2D line, Points node)
    {
        return (line.Points[0] - node.GetPosition()).Length() < 1.0f ||
               (line.Points[1] - node.GetPosition()).Length() < 1.0f;
    }
    
    private bool LineConnectsToVisibleNode(Line2D line, Points excludeNode)
    {
        return visiblePoints.Any(visiblePoint =>
            visiblePoint != excludeNode &&
            ((line.Points[0] - visiblePoint.GetPosition()).Length() < 1.0f ||
             (line.Points[1] - visiblePoint.GetPosition()).Length() < 1.0f));
    }
    
    private void ForceRevealIsolatedNode()
    {
        var isolatedNode = allPoints.FirstOrDefault(node => !node.Visible);
        if (isolatedNode == null) return;
        
        RevealNode(isolatedNode);
        
        // Reveal lines for this isolated node
        foreach (var line in allLines.Where(l => !l.Visible && LineConnectsToNode(l, isolatedNode)))
        {
            if (LineConnectsToVisibleNode(line, isolatedNode))
            {
                line.Visible = true;
            }
        }
    }
}
