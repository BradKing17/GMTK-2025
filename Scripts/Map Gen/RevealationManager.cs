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
        
        // Cull isolated nodes with only one connection before starting reveals
        CullIsolatedNodes();
        
        SetupTimer();
        
        // Add lines to scene tree
        foreach (var line in allLines)
        {
            GetParent().AddChild(line);
        }
    }

    private void CullIsolatedNodes()
    {
        bool nodesCulled;
        
        // Keep culling until no more isolated nodes are found
        do
        {
            nodesCulled = false;
            var isolatedNodes = allPoints.Where(p => p.GetNeighbours().Count <= 1).ToList();
            
            foreach (var isolatedNode in isolatedNodes)
            {
                // Remove this node from all its neighbors' neighbor lists
                foreach (var neighbor in isolatedNode.GetNeighbours().ToList())
                {
                    neighbor.RemoveNeighbour(isolatedNode);
                }
                
                // Remove associated lines
                RemoveLinesConnectedToNode(isolatedNode);
                
                // Remove from points list
                allPoints.Remove(isolatedNode);
                
                // Queue for deletion from scene tree
                if (isolatedNode.GetParent() != null)
                {
                    isolatedNode.QueueFree();
                }
                
                nodesCulled = true;
            }
            
        } while (nodesCulled); // Continue until no more nodes are culled
        
        GD.Print($"Culling complete. Remaining nodes: {allPoints.Count}");
    }

    private void RemoveLinesConnectedToNode(Points nodeToRemove)
    {
        var linesToRemove = allLines.Where(line => 
            LineConnectsToNode(line, nodeToRemove)).ToList();
        
        foreach (var line in linesToRemove)
        {
            allLines.Remove(line);
            
            // Queue for deletion from scene tree if already added
            if (line.GetParent() != null)
            {
                line.QueueFree();
            }
        }
    }
    
    public void StartReveal(int initialNodesCount, float delayBetweenNodes)
    {
        revealTimer.WaitTime = delayBetweenNodes;
        
        // Reveal initial nodes
        for (int i = 0; i < initialNodesCount && i < allPoints.Count; i++)
        {
            var nodeToReveal = FindNextNodeToReveal();
            if (nodeToReveal != null)
            {
                RevealNode(nodeToReveal);
                RevealConnectedLines(nodeToReveal);
            }
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
        
        // Try to reveal nodes that maintain proper connectivity
        var nodesRevealed = TryRevealWithProperConnectivity();
        
        if (!nodesRevealed && visiblePoints.Count < allPoints.Count)
        {
            // Handle isolated clusters
            if (visiblePoints.Count == visibleCountBefore && visiblePoints.Count > 0)
            {
                ForceRevealIsolatedNode();
            }
        }
        
        if (visiblePoints.Count < allPoints.Count)
        {
            revealTimer.Start();
        }
    }

    private bool TryRevealWithProperConnectivity()
    {
        // Try to find a node that will have at least 2 connections when revealed
        var nodeWithGoodConnectivity = FindNodeWithMinimumConnections(2);
        if (nodeWithGoodConnectivity != null)
        {
            RevealNode(nodeWithGoodConnectivity);
            RevealConnectedLines(nodeWithGoodConnectivity);
            return true;
        }
        
        // If no such node exists, reveal a sequence that forms a complete path
        return RevealPathSequence();
    }

    private Points FindNodeWithMinimumConnections(int minConnections)
    {
        if (visiblePoints.Count == 0)
        {
            return allPoints.FirstOrDefault();
        }
        
        return allPoints.FirstOrDefault(node => 
            !node.Visible && 
            CountVisibleConnections(node) >= minConnections);
    }

    private int CountVisibleConnections(Points node)
    {
        return node.GetNeighbours().Count(neighbor => neighbor.Visible);
    }

    private bool RevealPathSequence()
    {
        // Find a path of connected nodes that will form a complete loop
        var pathToReveal = FindPathToCompleteLoop();
        
        if (pathToReveal != null && pathToReveal.Count > 0)
        {
            foreach (var node in pathToReveal)
            {
                RevealNode(node);
                RevealConnectedLines(node);
            }
            return true;
        }
        
        // Fallback: reveal single node if no path found
        var nextNode = FindNextNodeToReveal();
        if (nextNode != null)
        {
            RevealNode(nextNode);
            RevealConnectedLines(nextNode);
            return true;
        }
        
        return false;
    }

    private List<Points> FindPathToCompleteLoop()
    {
        // Find a node with only one visible connection (potential start of path)
        var startNodes = visiblePoints.Where(node => 
            CountVisibleConnections(node) == 1).ToList();
        
        foreach (var startNode in startNodes)
        {
            var path = FindPathFromNode(startNode);
            if (path != null && path.Count > 0)
            {
                return path;
            }
        }
        
        // If no single-connection nodes, try from any edge node
        foreach (var visibleNode in visiblePoints)
        {
            var path = FindPathFromNode(visibleNode);
            if (path != null && path.Count > 0)
            {
                return path;
            }
        }
        
        return new List<Points>();
    }

    private List<Points> FindPathFromNode(Points startNode)
    {
        var path = new List<Points>();
        var visited = new HashSet<Points>(visiblePoints);
        var current = startNode;
        
        // Follow unvisited neighbors until we find a complete one-way loop path
        while (true)
        {
            var unvisitedNeighbors = current.GetNeighbours()
                .Where(neighbor => !visited.Contains(neighbor))
                .ToList();
            
            if (unvisitedNeighbors.Count == 0)
            {
                break;
            }
            
            // Find the best neighbor that maintains one-way loop property
            var nextNode = FindBestNeighborForOneWayLoop(unvisitedNeighbors, visited, startNode, path);
            
            if (nextNode == null)
            {
                break; // No valid path forward that maintains one-way property
            }
            
            path.Add(nextNode);
            visited.Add(nextNode);
            current = nextNode;
            
            // Check if this completes a one-way loop back to the start or visible network
            if (CanReturnToStartWithoutBacktracking(nextNode, visited, startNode))
            {
                break;
            }
            
            // Limit path length to prevent infinite loops
            if (path.Count > 8) // Increased limit for longer loops
            {
                break;
            }
        }
        
        // Only return the path if it forms a valid one-way loop
        if (path.Count > 0 && ValidateOneWayLoopPath(path, startNode, visited))
        {
            return path;
        }
        
        return new List<Points>();
    }

    private Points FindBestNeighborForOneWayLoop(List<Points> candidates, HashSet<Points> visited, Points startNode, List<Points> currentPath)
    {
        // Score each candidate based on how well it supports one-way loop formation
        var scoredCandidates = candidates.Select(candidate => new
        {
            Node = candidate,
            Score = CalculateOneWayLoopScore(candidate, visited, startNode, currentPath)
        }).Where(c => c.Score > 0) // Only consider valid candidates
          .OrderByDescending(c => c.Score);
        
        return scoredCandidates.FirstOrDefault()?.Node;
    }

    private float CalculateOneWayLoopScore(Points candidate, HashSet<Points> visited, Points startNode, List<Points> currentPath)
    {
        float score = 0;
        
        // Simulate adding this candidate to the path
        var tempVisited = new HashSet<Points>(visited) { candidate };
        
        // Check if this candidate has a path back to start or visible network
        if (!HasPathBackToStart(candidate, tempVisited, startNode))
        {
            return 0; // Invalid - no way back
        }
        
        // Prefer nodes with more total connections (more routing options)
        score += candidate.GetNeighbours().Count * 2;
        
        // Prefer nodes that connect back to visible network (complete loops)
        var connectsToVisible = candidate.GetNeighbours().Any(n => visiblePoints.Contains(n) && n != startNode);
        if (connectsToVisible)
        {
            score += 10;
        }
        
        // Prefer shorter paths back to start
        var pathLengthToStart = EstimatePathLengthToStart(candidate, tempVisited, startNode);
        if (pathLengthToStart > 0)
        {
            score += 5.0f / pathLengthToStart; // Shorter paths get higher scores
        }
        
        // Avoid creating branches that would require backtracking
        var wouldCreateDeadEnd = WouldCreateDeadEnd(candidate, tempVisited);
        if (wouldCreateDeadEnd)
        {
            score -= 5;
        }
        
        return score;
    }

    private bool HasPathBackToStart(Points from, HashSet<Points> visited, Points startNode)
    {
        // Use BFS to check if there's a path back to start without using visited nodes
        var queue = new Queue<Points>();
        var pathVisited = new HashSet<Points>();
        
        queue.Enqueue(from);
        pathVisited.Add(from);
        
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            
            // Check if we can reach the start node or any visible node
            if (current.GetNeighbours().Any(n => visiblePoints.Contains(n)))
            {
                return true;
            }
            
            // Continue searching through unvisited neighbors
            foreach (var neighbor in current.GetNeighbours())
            {
                if (!visited.Contains(neighbor) && !pathVisited.Contains(neighbor))
                {
                    queue.Enqueue(neighbor);
                    pathVisited.Add(neighbor);
                }
            }
            
            // Limit search depth
            if (pathVisited.Count > 10)
            {
                break;
            }
        }
        
        return false;
    }

    private int EstimatePathLengthToStart(Points from, HashSet<Points> visited, Points startNode)
    {
        // Simple BFS to estimate shortest path length
        var queue = new Queue<(Points node, int distance)>();
        var pathVisited = new HashSet<Points>();
        
        queue.Enqueue((from, 0));
        pathVisited.Add(from);
        
        while (queue.Count > 0)
        {
            var (current, distance) = queue.Dequeue();
            
            if (current.GetNeighbours().Any(n => visiblePoints.Contains(n)))
            {
                return distance + 1;
            }
            
            if (distance > 5) continue; // Limit search depth
            
            foreach (var neighbor in current.GetNeighbours())
            {
                if (!visited.Contains(neighbor) && !pathVisited.Contains(neighbor))
                {
                    queue.Enqueue((neighbor, distance + 1));
                    pathVisited.Add(neighbor);
                }
            }
        }
        
        return -1; // No path found
    }

    private bool WouldCreateDeadEnd(Points candidate, HashSet<Points> visited)
    {
        // Check if adding this candidate would create a dead end
        var unvisitedNeighbors = candidate.GetNeighbours()
            .Where(n => !visited.Contains(n))
            .ToList();
        
        // If it has multiple unvisited neighbors, it's less likely to be a dead end
        return unvisitedNeighbors.Count <= 1;
    }

    private bool CanReturnToStartWithoutBacktracking(Points currentNode, HashSet<Points> visited, Points startNode)
    {
        // Check if from this point we can return to start or visible network
        var hasPathToVisible = currentNode.GetNeighbours()
            .Any(neighbor => visiblePoints.Contains(neighbor) && neighbor != startNode);
        
        if (hasPathToVisible)
        {
            return true;
        }
        
        // Use the path validation to check for proper loop formation
        return CountVisibleConnectionsAfterReveal(currentNode, visited) >= 2;
    }

    private bool ValidateOneWayLoopPath(List<Points> path, Points startNode, HashSet<Points> visited)
    {
        if (path.Count == 0) return false;
        
        // Check that the end of the path connects back to visible network
        var endNode = path.Last();
        var hasReturnConnection = endNode.GetNeighbours()
            .Any(neighbor => visiblePoints.Contains(neighbor));
        
        if (!hasReturnConnection)
        {
            return false;
        }
        
        // Verify that each node in the path has exactly 2 connections within the visible network
        // (after the path is revealed)
        foreach (var node in path)
        {
            var connectionCount = CountVisibleConnectionsAfterReveal(node, visited);
            if (connectionCount < 1 || connectionCount > 2)
            {
                return false; // Invalid loop structure
            }
        }
        
        return true;
    }

    private int CountVisibleConnectionsAfterReveal(Points node, HashSet<Points> wouldBeVisible)
    {
        return node.GetNeighbours().Count(neighbor => wouldBeVisible.Contains(neighbor));
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
    }    private void RevealNode(Points node)
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
