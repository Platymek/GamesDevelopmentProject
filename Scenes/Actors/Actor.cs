using Godot;
using System;

public partial class Actor : CharacterBody3D
{
	[Export] protected float FallingAcceleration;
	[Export] protected int MaxHealth;
	[Export] protected float RotationSpeed = 1;
	
	[Export] protected float HorizontalAcceleration = 16;
	[Export] protected float HorizontalAirAcceleration = 4;
	
	[Export] protected float HorizontalDeceleration = 16;
	[Export] protected float HorizontalAirDeceleration = 4;
	
	[Export] protected float HorizontalMaxSpeed = 4;
	[Export] protected float FallingMaxSpeed = 4;

    [Export] private float FallingRotationSpeed = .5f;

    public enum Teams
	{
		None,
		Player,
		Enemy
	}

	[Export] public Teams Team;
	[Export] public bool Invincible = false;

	public int Health;
	public bool Dying;
	public bool Dead;
	protected bool Collided;

	private bool _accelerating;
	protected bool CanDecelerate;
	
	public float Angle
	{
		set =>
			Rotation = new Vector3(
				Rotation.X, value, Rotation.Z);

		get => Rotation.Y;
	}

	protected float Speed
	{
		get => new Vector2(Velocity.X, Velocity.Z).Length();

		set
		{
			// launch player in direction of tank
			var velocity
				= (Vector2.Up * value)
				.Rotated(Angle);

			// apply velocity
			Velocity = new Vector3(-velocity.X,
				Velocity.Y, velocity.Y);
		}
	}
	
	
	// Node Functions //

	public override void _Ready()
	{
		base._Ready();

		Dying = false;
		_accelerating = false;
		Health = MaxHealth;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		if (Dead)
		{
			QueueFree();
		}

		if ((!_accelerating || Speed > HorizontalMaxSpeed) && CanDecelerate)
		{
			Decelerate(delta);
		}

		_accelerating = false;


		// move based on velocity from previous frame
		Collided = MoveAndSlide();
	}

	
	// Other Functions //
	
	// jump a specific height which is calculated using the falling
	// acceleration of the actor
	public void Jump(float height)
	{
		// calculate vertical velocity using desired height
		// u = -sqrt(2as)
		var verticalVelocity
			= MathF.Sqrt(
				2 * FallingAcceleration * height);

		// apply velocity
		Velocity = new Vector3(Velocity.X,
			verticalVelocity, Velocity.Z);
	}

	// hurt an actor
	public void Hurt(int damage = 1)
	{
		if (Invincible) return;
		
		OnHurt(damage);
		
		// if health 0 or below, kill
		if (Health > 0) return;
		
		Kill();
		Dying = true;
	}

	// an overridable function to allow actors to react to being hurt
	protected virtual void OnHurt(int damage)
	{
		Health -= damage;
	}

	public virtual void Kill()
	{
		Dead = true;
	}
	
	protected void TurnTowards(float targetAngle, double delta)
	{
		float currentAngle = Angle;
		
		// get angle difference from current angle to new angle
		float angleDifference = Mathf.AngleDifference(
			currentAngle, targetAngle);
		
		// if the difference is 0, no rotation is needed
		if (angleDifference == 0) return;

		float rotationSpeed = Mathf.Tau * (float)delta
			* (IsOnFloor()
				? RotationSpeed
				: FallingRotationSpeed);
		
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

		if (IsOnFloor())
		{
			// turn character towards faced direction
			Speed = Speed;
		}
	}
	
	protected void Accelerate(double delta)
	{
		_accelerating = true;

        bool aboveMaxSpeed = Speed > HorizontalMaxSpeed;
		
		if (!IsOnFloor())
		{
			// accelerate forwards regardless of stick angle
			Velocity += Vector3.Forward.Rotated(Vector3.Up, Angle)
						* (float)delta * HorizontalAirAcceleration;
		}
		else
		{
			// accelerate forwards regardless of stick angle
			Speed += (float)delta * HorizontalAcceleration;
		}

		if (!aboveMaxSpeed && Speed > HorizontalMaxSpeed)
		{
			Speed = HorizontalMaxSpeed;
		}
	}
	
	protected void Decelerate(double delta)
	{
		if (Speed == 0) return;

		bool positivex = Velocity.X > 0;
		bool positivez = Velocity.Z > 0;
			
		if (!IsOnFloor())
        {
            Vector2 myVelocity2D = new(-Velocity.Z, -Velocity.X);

            // deccelerate in opposite direction of velocity
            Velocity -= Vector3.Forward.Rotated(Vector3.Up, myVelocity2D.Angle())
				* (float)delta * HorizontalAirDeceleration;
		}
		else
		{
			Speed -= (float)delta * HorizontalDeceleration;
		}

		// if decelerated in opposite direction
		if (positivex && Velocity.X < 0
			|| !positivex && Velocity.X > 0)
		{
			Velocity = new Vector3(
				0,
				Velocity.Y,
				Velocity.Z);
		}

		// if decelerated in opposite direction
		if (positivez && Velocity.Z < 0
			|| !positivez && Velocity.Z > 0)
		{
			Velocity = new Vector3(
				Velocity.X,
				Velocity.Y,
				0);
		}
	}
	
	// halts all horizontal velocity
	protected void Halt()
	{
		Velocity = new Vector3(0, Velocity.Y, 0);
	}

	protected void HaltFall()
	{
		Velocity = new Vector3(Velocity.X, 0, Velocity.Z);
	}

	protected void Fall(double delta)
	{
		float fallingSpeed = -Velocity.Y;

		fallingSpeed += FallingAcceleration * (float)delta;

		if (fallingSpeed > FallingMaxSpeed)
		{
			fallingSpeed = FallingMaxSpeed;
		}

		Velocity = new Vector3(Velocity.X, -fallingSpeed, Velocity.Z);
	}
}
