using Godot;
using System;

public partial class Checkpoint : Detector
{
	[Export] public CameraHint _cameraHint;

	Level _level;

    public override void _Ready()
    {
        base._Ready();

        _level = GetOwner<Level>();
    }


    private void OnDetected()
	{
        _level.LastCheckpoint = this;
    }
}
