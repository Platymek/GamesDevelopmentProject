using Godot;
using System;

// TODO: 

// add properties to attack stats file:
// - add cancellable boolean

// add ability to launch:
// - remove ammo

// add ability to fire a shell:
// - rotate in direction of aim immediately

// add reload:
// - add ammo (using reload function)

// all actions:
// - add input buffer

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
	[Export] private float _launchSpeedMultiplier = 2;

	private string _state;

	[Export] private string State
	{
		get => _state;

		set
		{
            RefreshPrivileges();

            switch (value)
			{
				case "idle":

                    // if not on floor, change state to falling instead
                    if (!IsOnFloor())
					{
						State = "falling";
					}
					else
					{
						_canReload = true;
                    }

					break;


				case "launch":

					var moveVector = StickAngle;

                    _canDecelerate = false;

                    if (Moving)
					{
						Jump(_launchHeight);
						Push(_horizontalMaxSpeed * _launchSpeedMultiplier);
					}
					else
                    {
                        Jump(_launchHeight * 1.5f);
						Halt();
                    }
					
					break;

				case "reload":

                    // snap angle in direction of stick
                    Angle = StickAngle;

					_canMove = false;
					_canDecelerate = false;
                    _canReload = false;

                    Halt();

                    break;

				case "fire":

                    // snap angle in direction of stick
                    Angle = StickAngle;
                    _canMove = false;
                    _canFall = false;
					_canDecelerate = false;

                    Halt();
					HaltFall();
                    Push(-_knockBackStrength);

                    break;
			}

			// play state animation if it exists
            if (_animationPlayer != null)
            {
                if (_animationPlayer.HasAnimation(value))
                {
                    _animationPlayer.Play(value);
                }
            }

            _state = value;
		}
	}

	private Node3D _turret;
	private AnimationPlayer _animationPlayer;

	private Label3D _velocityLabel;
	private Label3D _stateLabel;

    private bool _accelerating = false;

    // privileges
    private bool _canMove;
    private bool _canDecelerate;
	private bool _canTurn;
	private bool _canFall;

    private bool _canReload;

    // Node Functions //

    // get movement vector rotated relative to camera
    private float StickAngle
	{
		get
		{
			Vector2 move = Input.GetVector(
				"Down", "Up",
				"Right", "Left");

			Vector2 rotatedMove = move.Rotated(
				GetViewport().GetCamera3D().GlobalRotation.Y);

			return rotatedMove.Angle();
		}
	}

	private bool Moving
	{
		get
		{
			return Input.GetVector(
				"Down", "Up",
				"Right", "Left").Length() > 0;
		}
	}

	private float Angle
	{
		set
		{
			Rotation = new Vector3(
				Rotation.X, value, Rotation.Z);
		}

		get
		{
			return Rotation.Y;
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

        // for debug
        _velocityLabel = GetNode<Label3D>
            ("Debug/VelocityLabel");
        _stateLabel = GetNode<Label3D>
            ("Debug/StateLabel");

        _accelerating = false;
		_state = "idle";

		_canReload = true;

        RefreshPrivileges();
    }

	// Called every frame. 'delta' is the elapsed time
	// since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);
		
		// move based on velocity from previous frame
		MoveAndSlide();
		
		
		// Horizontal Movement //

		_accelerating = false;

        // get horizontal velocity
        var currentVelocity = new Vector2(Velocity.X, Velocity.Z);

        // get current speed
        float currentSpeed = currentVelocity.Length();

        // get angle of stick
        float turretRotation = StickAngle;

        // if the stick is being moved
        if (Moving && _canMove && currentSpeed < _horizontalMaxSpeed)
		{
			
			// rotate turret in direction of stick
			_turret.Rotation = new Vector3(
				0, turretRotation - Angle, 0);

			Accelerate(turretRotation, delta);
        }

        if (Moving && _canTurn)
        {
            TurnTowards(StickAngle, delta);
        }

		// refresh current speed
        currentSpeed = currentVelocity.Length();

        // if going faster than max speed or not accelerating, decelerate
        if ((currentSpeed > _horizontalMaxSpeed || !_accelerating) && _canDecelerate)
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

		if (!IsOnFloor() && _canFall)
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
		if (State == "falling" && IsOnFloor())
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
		else if (Input.IsActionJustPressed("Reload") && _canReload)
		{
			State = "reload";
		}

        // Debug //

        _velocityLabel.Text = "Velocity:"
                              + new Vector2(Velocity.X, Velocity.Z)
                                  .Length();


        _stateLabel.Text = $"State: {State}";
    }
	
	
	// Other Functions //

	private void Accelerate(float angle, double delta)
	{
		// accelerate forwards regardless of stick angle
		Velocity += Vector3.Forward.Rotated(Vector3.Up, Angle)
					* (float)delta * (IsOnFloor() 
						? _horizontalAcceleration
						: _horizontalAirAcceleration);

		_accelerating = true;
	}

	private void TurnTowards(float targetAngle, double delta)
	{
		float currentAngle = Angle;
		
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
			Angle = targetAngle;

			return;
		}

		// check direction of turning and turn in tha direction
		float newAngle = currentAngle 
						   + (angleDifference < 0
							   ? -rotationSpeed
							   : rotationSpeed);
		
		// apply rotation
		Angle = newAngle;
	}

	private void Jump(float magnitude)
    {
        // calculate vertical velocity using desired height
        // u = -sqrt(2as)
        var verticalVelocity
            = MathF.Sqrt(
                2 * _fallingAcceleration * magnitude);

        // apply velocity
        Velocity = new Vector3(-Velocity.X,
            verticalVelocity, Velocity.Y);
    }

    // push directly forwards
    private void Push(float force)
	{
        // launch player in direction of tank
        var velocity
            = (Vector2.Up * force)
                .Rotated(Angle);

        // apply velocity
        Velocity = new Vector3(-velocity.X,
            Velocity.Y, velocity.Y);
    }

	// halts all horizontal velocity
	private void Halt()
	{
		Velocity = new Vector3(0, Velocity.Y, 0);
	}

	private void HaltFall()
    {
        Velocity = new Vector3(Velocity.X, 0, Velocity.Z);
    }

	private void RefreshPrivileges()
    {
        _canMove = true;
        _canDecelerate = true;
        _canTurn = true;
		_canFall = true;
    }
}
