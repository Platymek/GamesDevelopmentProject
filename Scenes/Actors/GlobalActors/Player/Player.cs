using Godot;
using System;

public partial class Player : Actor
{
	// Properties //

	[Export] private float _horizontalMaxSpeed = 4;
	[Export] private float _horizontalAcceleration = 16;
	[Export] private float _horizontalAirAcceleration = 4;
	[Export] private float _horizontalDeceleration = 16;
	[Export] private float _horizontalAirDeceleration = 4;
	
	[Export] private float _fallingMaxSpeed = 4;
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
						_canDecelerate = false;
						_canAccelerate = false;
						return;
					}
					else
					{
						_canReload = true;
					}

					// rotate turret in direction of stick
					_turret.Rotation = Vector3.Up * (_aimAngle - Angle);

					break;


				case "launch":

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

					_canMove = false;
					_canDecelerate = false;
					_canReload = false;

					if (Moving)
					{
						// snap angle in direction of stick
						Angle = _aimAngle;
					}

					if (Velocity.Y < 0)
					{
						HaltFall();
					}

					break;

				case "fire":

					// snap angle in direction of stick
					if (Moving)
                    {
                        Angle = _aimAngle;
                    }

					_canMove = false;
					_canFall = false;
					_canDecelerate = false;

					// fire a shell
					_shellEmitter.Emit();

					Halt();
					HaltFall();
					Push(-_knockBackStrength);

					break;

				case "reload_quick":

					if (!Input.IsActionPressed("reload"))
					{
						State = "idle";
						return;
					}

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

	private int _ammo;
	private int _ammoLimit = 2;

	public int Ammo
	{
		get => _ammo;

		set
		{
			// keep ammo within bounds
			_ammo = value < 0
				? 0
				: value > _ammoLimit
				? _ammoLimit
				: value;
		}
	}

	private string _inputBuffer;

	private Node3D _turret;
	private AnimationPlayer _animationPlayer;
	private Emitter _shellEmitter;
	private Node3D _pointer;
	private Timer _inputBufferTimer;

	private Label3D _velocityLabel;
	private Label3D _stateLabel;
	private Label3D _ammoLabel;
	private Label3D _bufferLabel;

	private bool _accelerating = false;

	// privileges
	private bool _canMove;
	private bool _canDecelerate;
	private bool _canAccelerate;
	private bool _canTurn;
	private bool _canFall;
	private bool _canReload;

	// Node Functions //

	// get movement vector rotated relative to camera
	private float _aimAngle;

	private float StickAngle
	{
		get
		{
			Vector2 move = Input.GetVector(
				"Down", "Up",
				"Right", "Left");

			Vector2 rotatedMove = move.Rotated(
				GetViewport().GetCamera3D().GlobalRotation.Y);

			float halfPi = Mathf.Pi / 2;
			float quarPi = Mathf.Pi / 4;

			// snap angle in 4 cardinal directions as well as where the character is pointing
			return SnapAngle(rotatedMove.Angle(), 
				new float[] { 
					0, // N
					quarPi,	// NE
					halfPi, // E
					halfPi + quarPi, // SE
					halfPi * 2, // S
					halfPi * 2 + quarPi, // SW
					halfPi * 3, // W
					halfPi * 3 + quarPi, // NW
				}, 0.2f);
		}
	}

	private bool Moving =>
		Input.GetVector(
			"Down", "Up",
			"Right", "Left").Length() > 0;

	private float Angle
	{
		set =>
			Rotation = new Vector3(
				Rotation.X, value, Rotation.Z);

		get => Rotation.Y;
	}

	private float Speed => new Vector2(Velocity.X, Velocity.Z).Length();


	// Node Functions //
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();

		_turret = GetNode<Node3D>
			("Model/TankBase/TankTurret");
		_animationPlayer = GetNode<AnimationPlayer>
			("AnimationPlayer");
		_shellEmitter = GetNode<Emitter>
			("ShellEmitter");
		_pointer = GetNode<Node3D>
			("Pointer");
		_inputBufferTimer = GetNode<Timer>
			("InputBuffer");

		_aimAngle = Angle;
		_accelerating = false;
		_state = "idle";
		_canReload = true;
		Ammo = _ammoLimit;

		RefreshPrivileges();

		// for debug
		_velocityLabel = GetNode<Label3D>
			("Debug/VelocityLabel");
		_stateLabel = GetNode<Label3D>
			("Debug/StateLabel");
		_ammoLabel = GetNode<Label3D>
			("Debug/AmmoLabel");
		_bufferLabel = GetNode<Label3D>
			("Debug/BufferLabel");
	}

	// Called every frame. 'delta' is the elapsed time
	// since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);

		if (Moving)
		{
			_aimAngle = StickAngle;
		}

		_pointer.Rotation = Vector3.Up * (_aimAngle - Angle);
		
		
		// Input Buffer //

		string[] actions = new[] { "reload", "launch", "fire" };

		foreach (string action in actions)
		{
			if (Input.IsActionJustPressed(action))
			{
				_inputBuffer = action;
				_inputBufferTimer.Start();
				break;
			}
		}
		

		// Horizontal Movement //

		_accelerating = false;

		// get horizontal velocity
		var currentVelocity = new Vector2(Velocity.X, Velocity.Z);

		// get current speed
		float currentSpeed = currentVelocity.Length();

		// if the stick is being moved
		if (Moving && _canMove && currentSpeed < _horizontalMaxSpeed)
		{
			// rotate turret in direction of stick
			_turret.Rotation = Vector3.Up * (_aimAngle - Angle);

			Accelerate(_aimAngle, delta);
		}

		// turn towards stick angle
		if (Moving && _canTurn)
		{
			TurnTowards(_aimAngle, delta);
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

			fallingSpeed += FallingAcceleration * (float)delta;

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

		switch (State)
		{
			case "falling":

				if (IsOnFloor())
				{
					State = "idle";
				}

				if (Speed < _horizontalMaxSpeed
					|| SnapAngle(_aimAngle, new[] { Angle },
						0.3f) != Angle
					|| !Moving)
				{
					_canAccelerate = true;
					_canDecelerate = true;
				}

				break;

			case "reload":
			case "reload_quick":

				if (Moving)
				{
					// snap angle in direction of stick
					Angle = _aimAngle;
				}

				break;
		}

		if (_inputBuffer != null)
		{
			bool bufferUsed = false;
			
			if (_inputBuffer == "launch")
			{
				if (HasAndUseAmmo())
				{
					State = "launch";
					bufferUsed = true;
				}
			}
			else if (_inputBuffer == "fire")
			{
				if (HasAndUseAmmo())
				{
					State = "fire";
					bufferUsed = true;
				}
			}
			else if (_inputBuffer == "reload" && _canReload)
			{
				State = "reload";
				bufferUsed = true;
			}

			if (bufferUsed)
			{
				_inputBuffer = null;
			}
		}


		// Debug //

		_velocityLabel.Text = "Velocity:"
							  + new Vector2(Velocity.X, Velocity.Z)
								  .Length();


		_stateLabel.Text = $"State: {State}";

		_ammoLabel.Text = $"Ammo: {Ammo}";
		
		_bufferLabel.Text = $"Buffer: {_inputBuffer}";
	}
	
	
	// Signals //

	private void OnInputBufferTimeout()
	{
		_inputBuffer = null;
	}
	
	
	// Other Functions //

	private void Accelerate(float angle, double delta)
	{
		if (!IsOnFloor())
        {
            // accelerate forwards regardless of stick angle
            Velocity += Vector3.Forward.Rotated(Vector3.Up, Angle)
                        * (float)delta * _horizontalAirAcceleration;
        }
		else
        {
			// accelerate forwards regardless of stick angle
			Push(Speed + (float)delta * _horizontalAcceleration);
        }

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

	// reload a shell
	private void Reload()
	{
		Ammo++;
	}

	// check have any ammo and remove accordingly
	private bool HasAndUseAmmo()
	{
		if (Ammo > 0)
		{
			Ammo--;
			return true;
		}

		return false;
	}

	// snap an angle if it is near to any angle in a list of angles
	private float SnapAngle(float angle, float[] snapAngles, float tolerance = 0.1f)
	{
		// this foreach loop means that angles listed first have more priority
		foreach (float snapAngle in snapAngles)
		{
			float angleDifference = Mathf.Abs(Mathf.AngleDifference(snapAngle, angle));

			if (angleDifference < tolerance)
			{
				return snapAngle;
			}
		}

		return angle;
	}
}