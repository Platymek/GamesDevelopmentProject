using Godot;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

public partial class Player : Actor
{
	// Properties //
	
	[Export] private float _fallingDeceleration = 16;
	
	[Export] private float _knockBackStrength = 2;
	[Export] private int _maxAmmoCount = 2;
	[Export] private float _launchHeight = 4;
	[Export] private float _launchSpeedMultiplier = 2;

	[Export] private Level _level;

    [Export(PropertyHint.Range, "0,1")] private float AimAssistStrength = 0.2f;


    private string _state;

	[Export] private string State
	{
		get => _state;

		set
		{
			RefreshPrivileges();
			Invincible = false;

			switch (value)
			{
				case "idle":

					// if not on floor, change state to falling instead
					if (!IsOnFloor())
					{
						State = "falling";
						_canAccelerate = false;
						CanDecelerate = false;
						return;
					}
					else
					{
						_canReload = true;
					}

					break;


				case "launch":

					CanDecelerate = false;
					_explosionEmitter.Emit();

					// high jump if not moving the stick
					if (Moving)
					{
						Jump(_launchHeight);
						Speed = HorizontalMaxSpeed * _launchSpeedMultiplier;
					}
					else
					{
						Jump(_launchHeight * 2f);
						Halt();
					}

					break;


				case "reload":

					_canMove = false;
					CanDecelerate = false;
					_canReload = false;

					if (Moving)
					{
						// snap angle in direction of stick
						Angle = _aimAngle;
					}

					if (Velocity.Y <= 0)
					{
						HaltFall();
					}

					break;


				case "fire":

                    // snap angle in direction of stick
                    Angle = AimAssistAngle;

					_canMove = false;
					_canFall = false;
					CanDecelerate = false;
					_canAccelerate = false;
					_canTurn = false;

                    // fire a shell
                    _shellEmitter.Emit();

					// snap angle back to aim angle
                    Angle = _aimAngle;

					// halt all momentum and knockback player
                    Halt();
					HaltFall();
					Speed = -_knockBackStrength;

                    break;


				case "reload_quick":

					if (!Input.IsActionPressed("reload"))
					{
						State = "idle";
						return;
					}

                    CanDecelerate = false;

                    break;


				case "hurt":

                    Invincible = true;
                    _canMove = false;
                    _canFall = false;
                    CanDecelerate = false;

                    Halt();
                    HaltFall();
                    Speed = -_knockBackStrength;

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
	private Vector3 _respawnPoint;


    [ExportGroup("Nodes")]

    [Export] private Node3D _turret;
	[Export] private AnimationPlayer _animationPlayer;
	[Export] private Emitter _shellEmitter;
	[Export] private Emitter _explosionEmitter;
	[Export] private Timer _inputBufferTimer;
	[Export] private Detector _aimAssistDetector;
    [Export] private Node3D _aimReticle;

	[Export] private Node3D _jumpPointer;
	[Export] private float _jumpPointerAngle = -60;

    [Export] private Label3D _velocityLabel;
	[Export] private Label3D _stateLabel;
	[Export] private Label3D _ammoLabel;
	[Export] private Label3D _bufferLabel;

	// privileges
	 private bool _canMove;
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
				}, quarPi * 0.25f);
		}
	}

	private float AimAssistAngle
	{
		get
		{
			float aimAssistAngle = _aimAngle;
			List<float> anglesToEnemies = new();
			Vector2 myPosition2D = new(-Position.Z, -Position.X);

            foreach (Node3D a in _aimAssistDetector.DetectedActors)
            {
                Vector2 aPosition2D = new(-a.Position.Z, -a.Position.X);

                float angle = myPosition2D.AngleToPoint(aPosition2D);
				anglesToEnemies.Add(angle);
			}

			if (anglesToEnemies.Count > 0)
			{
                aimAssistAngle = SnapAngle(aimAssistAngle, anglesToEnemies.ToArray(),
					AimAssistStrength * Mathf.Pi);

            }

            return aimAssistAngle;
		}
	}

	private bool Moving =>
		Input.GetVector(
			"Down", "Up",
			"Right", "Left").Length() > 0;


	// Node Functions //
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();

		// initialise properties
		_aimAngle = Angle;
		_state = "idle";
		_canReload = true;
		Ammo = _ammoLimit;

		RefreshPrivileges();
	}

	// Called every frame. 'delta' is the elapsed time
	// since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);

		// if the stick is being moved, the shooting aims in the direction
		// that the player is pointing the stick. Otherwise, directly forward
		if (Moving)
		{
			_aimAngle = StickAngle;
        }

        // point floor pointer and turret
        _turret.Rotation = Vector3.Up * (_aimAngle - Angle);

        // point aiming reticle
        _aimReticle.Rotation = Vector3.Up * Mathf.LerpAngle(
			_aimReticle.Rotation.Y, AimAssistAngle - Angle, 
			(float)delta * 20f);

		// hide static aim reticle if auto aim reticle detected
		_aimReticle.GetNode<Node3D>("Target").Visible =
			!_aimReticle.GetNode<RayCast3D>("AimReticle").IsColliding();

		// if moving, move jump pointer arrow to show that the player
		// will be jumping forward

		Vector3 jpRotation = _jumpPointer.Rotation;

        jpRotation.X = Mathf.LerpAngle(
            _jumpPointer.Rotation.X, 
			Moving
				? Mathf.DegToRad(_jumpPointerAngle)
				: 0,
            (float)delta * 20f);

		_jumpPointer.Rotation = jpRotation;


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

        // if the stick is being moved
        if (_canMove 
			&& Moving 
			&& Speed < HorizontalMaxSpeed)
		{
			Accelerate(delta);
		}

		// turn towards stick angle
		if (_canTurn)
		{
			TurnTowards(_aimAngle, delta);
		}
		

		// Vertical Movement //

		if (!IsOnFloor() && _canFall)
		{
			Fall(delta);
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

				if (Speed < HorizontalMaxSpeed
					|| SnapAngle(_aimAngle, new[] { Angle },
						0.3f) != Angle
					|| !Moving)
				{
					_canAccelerate = true;
					CanDecelerate = true;
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

		_velocityLabel.Text = $"Velocity:{Speed}";
		
		_stateLabel.Text = $"State: {State}";
		
		_ammoLabel.Text = $"Ammo: {Ammo}";
		
		_bufferLabel.Text = $"Buffer: {_inputBuffer}";
	}
	
	
	// Signals //

	private void OnInputBufferTimeout()
	{
		_inputBuffer = null;
	}

	private void OnSetRespawnTimerTimeout()
	{
		if (IsOnFloor())
		{
			_respawnPoint = Position;
		}
	}
	
	
	// Other Functions //

	private void RefreshPrivileges()
	{
		_canMove = true;
		CanDecelerate = true;
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

	// send the player back to the last place they were standing
	public void Respawn(Vector3 respawnPoint)
	{
		Position = respawnPoint;

		Halt();
	}

	public override void Kill()
	{
		_level.Lose();
	}

    protected override void OnHurt(int damage)
    {
        base.OnHurt(damage);

		State = "hurt";
    }
}
