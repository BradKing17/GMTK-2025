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
}
