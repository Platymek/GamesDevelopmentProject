using Godot;
using System;

// TODO: 

// add properties to stats resource file:
// - add max horizontal speed
// - add max falling speed
// - add fall acceleration
// - add car rotation speed
// - add shot knock back strength (which will also be used for reload)
// - add ammo count
// - add launch height

// add properties to attack stats file:
// - add cancellable boolean

// add ability to move:
// - rotate tank in direction of turret aim

// add ability to launch:
// - add animation
// - add launching physics
// - add upwards launch
// - remove ammo

// add ability to fire a shell:
// - add animation
// - rotate in direction of aim immediately

// add reload:
// - add animation
// - rotate in direction of reload immediately
// - add sliding forwards
// - add ammo (using reload function)
// - double strength of gravity

public partial class Player : CharacterBody3D
{
	// Properties //

	private Node3D _turret;
	
	// get movement vector rotated relative to camera
	private Vector2 MoveVector
	{
		get
		{
			Vector2 move = Input.GetVector(
				"Down", "Up",
				"Right", "Left");

			GD.Print(GetViewport().GetCamera3D().GlobalRotation.Y);

			Vector2 rotatedMove = move.Rotated(
				GetViewport().GetCamera3D().GlobalRotation.Y
				- Rotation.Y);

			return rotatedMove;
		}
	}
	
	
	// Node Functions //
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();

		_turret = GetNode<Node3D>("Model/TankBase/TankTurret");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);

		Vector2 move = MoveVector;
		
		// if the stick is being moved
		if (move != Vector2.Zero)
		{
			// get angle of stick
			float turretRotation = MoveVector.Angle();
			
			// rotate turret in direction of stick
			_turret.Rotation = new Vector3(
				0, turretRotation, 0);
			
			// move forwards regardless of stick angle
			Velocity += Vector3.Forward.Rotated(
				Vector3.Up, Rotation.Y);
			
			// move stick in direction of 
		}

		MoveAndSlide();
	}
	
	
	// Other Functions //

	
}
