using Godot;
using System;

// TODO: 

// add properties to attack stats file:
// - add cancellable boolean

// add ability to launch:
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

// all actions:
// - add input buffer
// - add cancellable frames

public partial class Player : CharacterBody3D
{
	// Properties //

	[Export] private float _horizontalMaxSpeed = 4;
	[Export] private float _horizontalAcceleration = 16;
	[Export] private float _horizontalAirAcceleration = 4;
	[Export] private float _horizontalDeceleration = 16;
	[Export] private float _horizontalAirDeceleration = 4;
	
	[Export] private float _fallingMaxSpeed = 4;
	[Export] private float _fallingAcceleration = 16;
	[Export] private float _fallingDeceleration = 16;
	
	[Export] private float _rotationSpeed = 1;
	[Export] private float _knockBackStrength = 2;
	[Export] private int _maxAmmoCount = 2;
	[Export] private float _launchHeight = 4;

	private string _state;

	[Export] private string State
	{
		get => _state;

		set
		{
			if (_animationPlayer.HasAnimation(value))
			{
				_animationPlayer.Play(value);
			}

			switch (value)
			{
				case "launch":

					var moveVector = MoveVector;
					
					if (moveVector.Length() > 0)
					{
						Jump(_launchHeight);
						
						// launch player in direction of tank
						var horizontalVelocity 
							= (Vector2.Up * _horizontalMaxSpeed * 3)
								.Rotated(Rotation.Y);
						
						GD.Print(horizontalVelocity.Length());
						
						// calculate vertical velocity using desired height
						// u = -sqrt(2as)
						var verticalVelocity
							= MathF.Sqrt(
								2 * _fallingAcceleration * _launchHeight);
						
						// apply velocity
						Velocity = new Vector3(-horizontalVelocity.X, 
							verticalVelocity, horizontalVelocity.Y);
						
					}
					else
					{
						// calculate vertical velocity using desired height
						// which is double launch height
						// u = -sqrt(2a 2s)
						var verticalVelocity
							= MathF.Sqrt(
								4 * _fallingAcceleration * _launchHeight);
						
						// apply velocity
						Velocity = new Vector3(0, 
							verticalVelocity, 0);
					}
					
					break;
			}
			
			_state = value;
		}
	}

	private Node3D _turret;
	private AnimationPlayer _animationPlayer;
	private Label3D _velocityLabel;

	private bool _accelerating = false;
	
	
	// Node Functions //
	
	// get movement vector rotated relative to camera
	private Vector2 MoveVector
	{
		get
		{
			Vector2 move = Input.GetVector(
				"Down", "Up",
				"Right", "Left");

			Vector2 rotatedMove = move.Rotated(
				GetViewport().GetCamera3D().GlobalRotation.Y);

			return rotatedMove;
		}
	}
	
	
	// Node Functions //
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();

		_turret = GetNode<Node3D>
			("Model/TankBase/TankTurret");
		_animationPlayer = GetNode<AnimationPlayer>
			("AnimationPlayer");
		_velocityLabel = GetNode<Label3D>
			("VelocityLabel");

		_accelerating = false;
		_state = "idle";
	}

	// Called every frame. 'delta' is the elapsed time
	// since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);
		
		// move based on velocity from previous frame
		MoveAndSlide();
		
		
		// Horizontal Movement //

		Vector2 move = MoveVector;
		_accelerating = false;
		
		// if the stick is being moved
		if (move != Vector2.Zero)
		{
			// get angle of stick
			float turretRotation = MoveVector.Angle();
			
			// rotate turret in direction of stick
			_turret.Rotation = new Vector3(
				0, turretRotation - Rotation.Y, 0);

			Accelerate(turretRotation, delta);
		}
	
		// get horizontal velocity
		var currentVelocity = new Vector2(Velocity.X, Velocity.Z);
		
		// get current speed
		float currentSpeed = currentVelocity.Length();

		// if going faster than max speed or not accelerating, decelerate
		if (currentSpeed > _horizontalMaxSpeed || !_accelerating)
		{
			// decelerate
			float newSpeed = currentSpeed 
							 - _horizontalDeceleration
							 * (float)delta * (IsOnFloor() 
								 ? _horizontalDeceleration
								 : _horizontalAirDeceleration);
			
			// if deceleration makes the value below max speed and the player
			// is accelerating, cap to max speed
			if (newSpeed < _horizontalMaxSpeed && _accelerating)
			{
				newSpeed = _horizontalMaxSpeed;
			}
			
			// if deceleration reduced the new speed below 0, set to 0
			if (newSpeed < 0)
			{
				newSpeed = 0;
			}
				
			// normalise velocity and multiply by the new speed to
			// retain angle
			var newVelocity = currentVelocity.Normalized() 
								  * newSpeed;

			Velocity = new Vector3(newVelocity.X, Velocity.Y, 
				newVelocity.Y);
		}
		
		
		// Vertical Movement //

		if (!IsOnFloor())
		{
			float fallingSpeed = -Velocity.Y;

			fallingSpeed += _fallingAcceleration * (float)delta;

			if (fallingSpeed > _fallingMaxSpeed)
			{
				fallingSpeed = _fallingMaxSpeed;
			}
			
			Velocity = new Vector3(Velocity.X, -fallingSpeed, Velocity.Z);
		}

		if (IsOnFloorOnly())
		{
			Velocity = new Vector3(Velocity.X, 0, Velocity.Z);
		}
		
		
		// States //

		// swap state back to idle once landed
		if (State == "falling" && IsOnFloorOnly())
		{
			State = "idle";
		}
		
		if (Input.IsActionJustPressed("Launch"))
		{
			State = "launch";
		}
		else if (Input.IsActionJustPressed("Fire"))
		{
			State = "fire";
		}
		else if (Input.IsActionJustPressed("Reload"))
		{
			State = "reload";
		}
		
		// Debug //
			
		_velocityLabel.Text = "Velocity:\n" 
		                      +  new Vector2(Velocity.X, Velocity.Z)
			                      .Length();
	}
	
	
	// Other Functions //

	private void Accelerate(float angle, double delta)
	{
		// accelerate forwards regardless of stick angle
		Velocity += Vector3.Forward.Rotated(Vector3.Up, Rotation.Y)
					* (float)delta * (IsOnFloor() 
						? _horizontalAcceleration
						: _horizontalAirAcceleration);

		_accelerating = true;

		TurnTowards(angle, delta);
	}

	private void TurnTowards(float targetAngle, double delta)
	{
		float currentAngle = Rotation.Y;
		
		// get angle difference from current angle to new angle
		float angleDifference = Mathf.AngleDifference(
			currentAngle, targetAngle);
		
		// if the difference is 0, no rotation is needed
		if (angleDifference == 0) return;

		float rotationSpeed = Mathf.Tau * _rotationSpeed * (float)delta;
		
		// if the difference in angle is smaller than the rotation speed
		// snap to target angle and return
		if (MathF.Abs(angleDifference) < rotationSpeed)
		{
			// apply rotation
			Rotation = new Vector3(
				Rotation.X,
				targetAngle,
				Rotation.Z);

			return;
		}

		// check direction of turning and turn in tha direction
		float newAngle = currentAngle 
						   + (angleDifference < 0
							   ? -rotationSpeed
							   : rotationSpeed);
		
		// apply rotation
		Rotation = new Vector3(
			Rotation.X,
			newAngle,
			Rotation.Z);
	}

	private void Jump(float magnitude)
	{
		Velocity += Vector3.Up * magnitude;
	}
}
