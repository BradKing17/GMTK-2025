using Godot;
using System;

public partial class House : Points
{

    public int postWaitingForDelivery = 0;
    public bool maxPostReached = false;

    private Timer postTimer;
    private Label timerLabel;
    private bool timerStarted = false;
    
    public override void _Ready()
    {
        base._Ready();
        mainColor = debugIcon.Color = Colors.Green;
        SetSprite("res://Assets/Images/MapSprites/House.png");

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

    public override void _Notification(int what)
    {
        base._Notification(what);
        if (what == NotificationVisibilityChanged && IsVisibleInTree() && !timerStarted)
        {
            StartTimer();
        }
    }
    public void InitializeTimer()
    {
        postTimer = new Timer();
        postTimer.WaitTime = 1.0f;
        postTimer.OneShot = false;
        postTimer.Timeout += OnPostTimerTimeout;
        
        AddChild(postTimer);
        UpdateLabel();
    }

    public void StartTimer()
    {
        if (postTimer != null && !timerStarted)
        {
            postTimer.Start();
            timerStarted = true;
        }
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
