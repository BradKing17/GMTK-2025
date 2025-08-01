using Godot;
using System;

public partial class House : Points
{

    public int postWaitingForDelivery = 0;
    public bool maxPostReached = false;

    private Timer postTimer;
    private Label timerLabel;

    public void InitializeTimer(Node parent, Label label)
    {
        timerLabel = label;
        postTimer = new Timer();
        postTimer.WaitTime = 1.0f;
        postTimer.OneShot = false;
        postTimer.Timeout += OnPostTimerTimeout;
        parent.AddChild(postTimer);
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
