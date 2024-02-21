using Godot;
using System;

public partial class DropShadow : RayCast3D
{
	private Sprite3D _shadow;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();

		_shadow = GetChild<Sprite3D>(0);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);

		_shadow.Visible = IsColliding();
		_shadow.GlobalPosition = GetCollisionPoint() + Vector3.Up * 0.2f;
	}
}
