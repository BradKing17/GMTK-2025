using Godot;
using System.Collections.Generic;
using System.Drawing;

public partial class PointManager : Node
{
    private List<Points> points = [];
    private Timer gameOverTimer;
    private float gameOverTimeLeft = 60f;
    private bool timerActive = false;
    private Points highlightedPoint = null;
    private Points selectedPoint = null;

    private List<Points> currentRoute = new List<Points>();
    public void SetSelectedPoint(Points point) { selectedPoint = point; }
    public void SetHighlightedPoint(Points point) { highlightedPoint = point; }

    public Globals globals;

    public override void _Ready()
    {
        globals = GetNode<Globals>(GetTree().Root.GetChild(0).GetPath());
        gameOverTimer = new Timer();
        gameOverTimer.WaitTime = 1.0f;
        gameOverTimer.OneShot = false;
        gameOverTimer.Timeout += OnGameOverTimerTimeout;
        AddChild(gameOverTimer);
    }

    public void RegisterPoint(Points point)
    {
        points.Add(point);
        point.manager = this;
    }

    public void DeregisterPoint(Points point)
    {
        points.Remove(point);
    }

    public void AddPointNeighbour(Points point, Points neighbour)
    {
        point.AddNeighbour(neighbour);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        base._Input(@event);
        if (@event.IsActionPressed("Left MB"))
        {
            if(highlightedPoint != null) // If a node is hovered over
            {

                //If there is no currently selected point
                if(selectedPoint == null) 
                {
                    foreach (Points neighbour in highlightedPoint.GetNeighbours())
                    {
                        neighbour.GetChildOrNull<ColorRect>(1).Color = new Godot.Color(1f, 1f, 1f);
                    }
                    highlightedPoint.isSelected = true;
                    selectedPoint = highlightedPoint;
                    GD.Print("SELECTED");
                }

                //if there is a selected point, remove the current selection, then add the new point as selected
                else if(selectedPoint != null)
                {
                    if(selectedPoint.GetNeighbours().Contains(highlightedPoint))
                    {
                        Line2D newLine = new Line2D();
                        newLine.AddPoint(selectedPoint.GetPosition());
                        newLine.AddPoint(highlightedPoint.GetPosition());
                        newLine.Width = 2;
                        newLine.DefaultColor = new Godot.Color(1f, 0f, 0f);
                        AddChild(newLine);
                    }
                    foreach (Points neighbour in selectedPoint.GetNeighbours())
                    {  
                        neighbour.GetChildOrNull<ColorRect>(1).Color = neighbour.mainColor;
                    }

                    selectedPoint.GetChildOrNull<ColorRect>(1).Color = selectedPoint.mainColor;
                    selectedPoint.isSelected = false;

                    foreach (Points neighbour in highlightedPoint.GetNeighbours())
                    {
                        
                        neighbour.GetChildOrNull<ColorRect>(1).Color = new Godot.Color(1f, 1f, 1f);
                    }
                    highlightedPoint.isSelected = true;
                    selectedPoint = highlightedPoint;

                }

                if (currentRoute.Contains(highlightedPoint))
                {
                    selectedPoint.GetChildOrNull<ColorRect>(1).Color = new Godot.Color(1f, 1f, 1f);
                    currentRoute.Add(selectedPoint);
                    GD.Print("Loop complete");
                    EndLoop(currentRoute);
                }
                else
                {
                    selectedPoint.GetChildOrNull<ColorRect>(1).Color = new Godot.Color(1f, 1f, 1f);
                    currentRoute.Add(selectedPoint);
                    GD.Print("Added" + selectedPoint);
                }
            }
            else
            {
                if (selectedPoint != null)
                {
                    foreach (Points neighbour in selectedPoint.GetNeighbours())
                    {
                        neighbour.GetChildOrNull<ColorRect>(1).Color = neighbour.mainColor;
                    }
                    selectedPoint.GetChildOrNull<ColorRect>(1).Color = selectedPoint.mainColor;
                    selectedPoint.isSelected = false;
                }
            }
            

        }
    }

    public void EndLoop(List<Points> newLoop)
    {
        globals.depotManager.StartLoop(newLoop);
        currentRoute.Clear();
    }
    public override void _Process(double delta)
    {
        bool anyMaxPostReached = false;
        foreach (var point in points)
        {
            // Check if this point has a House component
            foreach (Node child in point.GetChildren())
            {
                if (child is House house && house.maxPostReached)
                {
                    anyMaxPostReached = true;
                    break;
                }
            }
            if (anyMaxPostReached) break;
        }

        if (anyMaxPostReached)
        {
            if (!timerActive)
            {
                gameOverTimeLeft = 60f;
                gameOverTimer.Start();
                timerActive = true;
            }
        }
        else
        {
            if (timerActive)
            {
                gameOverTimer.Stop();
                gameOverTimeLeft = 60f;
                timerActive = false;
            }
        }
    }

    private void OnGameOverTimerTimeout()
    {
        gameOverTimeLeft--;
        GD.Print($"Timer ticked! Time left: {gameOverTimeLeft}");
        if (gameOverTimeLeft <= 0)
        {
            gameOverTimer.Stop();
            ShowGameOverScreen();
        }
    }

    private void ShowGameOverScreen()
    {
        GD.Print("Game Over! GET FUCKED POSTIE! You took too long to deliver the post! You are fired! You are a disgrace to the post office! Eat SHIT!");
    }
}
