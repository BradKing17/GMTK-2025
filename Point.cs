using Godot;

public class Point
{
    public Point() { }

    private Vector2 position;
    private int numOfConnections;

    public int postWaitingForDelivery = 0;
    public bool maxPostReached = false;

    private Timer postTimer;

    public void InitializeTimer(Node parent)
    {
        postTimer = new Timer();
        postTimer.WaitTime = 10.0f;
        postTimer.OneShot = false;
        postTimer.Timeout += OnPostTimerTimeout;
        parent.AddChild(postTimer);
        postTimer.Start();
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

    public Vector2 GetPosition() { return position; }
    public void SetPosition(Vector2 newPos) { position = newPos; }
    public int GetNumConnections() { return numOfConnections; }
}
