using Godot;
using System;

public partial class GranniesHouse : Points
{
    public override void HitPoint(Postie postie)
    {
        postie.status.SetStatus(Postie.Status.Feeling.Happy);
        postie.status.fatiguePercentage = postie.status.fatiguePercentage - 10.0f;
    }

}

