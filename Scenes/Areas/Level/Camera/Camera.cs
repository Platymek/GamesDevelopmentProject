using Godot;
using System;

public partial class Camera : Node3D
{
	[Export] private Node3D _target;

	[Export] private bool _trackX;
    [Export] private bool _trackY;
    [Export] private bool _trackZ;

	[Export] public int X;
	[Export] public int Y;
	[Export] public int Z;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		base._Ready();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Position = new Vector3(
			_trackX ? _target.Position.X : X,
			_trackY ? _target.Position.Y : Y,
			_trackZ ? _target.Position.Z : Z);
    }
	
	
	// Other Functions //

}
