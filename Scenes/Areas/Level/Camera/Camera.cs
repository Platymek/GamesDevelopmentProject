using Godot;
using System;

public partial class Camera : Node3D
{
	[Export] private Node3D _target;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (_target != null)
		{
			Position = new Vector3(
				_target.Position.X, 0, 0);
		}
	}
	
	
	// Other Functions //

}
