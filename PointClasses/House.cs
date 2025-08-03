using Godot;
using System;

public partial class House : Points
{

    public int postWaitingForDelivery = 0;
    public bool maxPostReached = false;

    private Timer postTimer;
    private Label timerLabel;
    public override void _Ready()
    {
        base._Ready();
        mainColor = debugIcon.Color = Colors.Green;

        timerLabel = new Label()
        {
            Text = "0",
            Position = new Vector2(0, -60),
            LabelSettings = new LabelSettings()
            {
                FontSize = 40,
            }
        };
        AddChild(timerLabel);
        InitializeTimer();
    }
    public void InitializeTimer()
    {
        postTimer = new Timer();
        postTimer.WaitTime = 1.0f;
        postTimer.OneShot = false;
        postTimer.Timeout += OnPostTimerTimeout;
        
        AddChild(postTimer);
        postTimer.Start();
        UpdateLabel();
    }

    private void OnPostTimerTimeout()
    {
        if (postWaitingForDelivery < 30)
        {
            postWaitingForDelivery++;
            if (postWaitingForDelivery == 30)
            {
                maxPostReached = true;
            }
        }
        else
        {
            maxPostReached = true;
        }
        UpdateLabel();

    }
    private void UpdateLabel()
    {
        if (timerLabel != null)
        {
            timerLabel.Text = $"{postWaitingForDelivery}";
            
            if (postWaitingForDelivery == 30)
                timerLabel.LabelSettings.FontColor = new Color(1, 0, 0); // Red
            else
                timerLabel.LabelSettings.FontColor = new Color(1, 1, 1); // White
        }
    }

    public void DecreasePost(int amount)
    {
        postWaitingForDelivery -= amount;
        if (postWaitingForDelivery < 30)
        {
            maxPostReached = false;
        }
        if (postWaitingForDelivery < 0)
        {
            postWaitingForDelivery = 0;
        }
    }
}
