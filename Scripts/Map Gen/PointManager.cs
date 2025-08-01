using Godot;
using System.Collections.Generic;

public partial class PointManager : Node
{
    private List<Points> points = [];
    private Timer gameOverTimer;
    private float gameOverTimeLeft = 60f;
    private bool timerActive = false;

    public override void _Ready()
    {
        gameOverTimer = new Timer();
        gameOverTimer.WaitTime = 1.0f;
        gameOverTimer.OneShot = false;
        gameOverTimer.Timeout += OnGameOverTimerTimeout;
        AddChild(gameOverTimer);
    }

    public void RegisterPoint(Points point)
    {
        points.Add(point);
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
