using Godot;
using System;

public partial class Postie : Node2D
{
    enum Transport
    {
        ON_FOOT,
        WAGON,
        VAN
    }


    //  Stats

    private string postieName = "John Postman";

    private int level = 1;

    private float maxFatigue = 100.0f;

    private float maxPost = 100.0f;

    private Transport transport = Transport.ON_FOOT;



    //  Current State

    private float speed = 10.0f;

    private float currentPost = 0.0f;

    private float currentFatigue = 0.0f;

    public override void _Ready()
    {
        switch (transport)
            {
            case Transport.ON_FOOT:
                speed = 10.0f;
                maxPost = 50.0f;
                break;
            case Transport.WAGON:
                speed = 15.0f;
                maxPost = 150.0f;
                break;
            case Transport.VAN:
                speed = 30.0f;
                maxPost = 300.0f;
                break;

        }

    }

    public void PointHit(House house)
    {
        house.DecreasePost(10);
    }
}
