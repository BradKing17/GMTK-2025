using Godot;

public partial class ParkBench : Points
{
    public override void _Ready()
    {
        base._Ready();
        mainColor = debugIcon.Color = Colors.Brown;
    }
}
