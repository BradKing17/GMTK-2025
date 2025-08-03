using Godot;
using System;

public partial class MainMenu : Control
{
    [Export] public Button playButton = null;

    public Button quitButton = null;

    public override void _Ready()
    {
        base._Ready();
        playButton.Pressed += Play;

    }

    public void Play()
    {
        GetTree().ChangeSceneToFile("res://ProcGenTest.tscn");
    }

}
