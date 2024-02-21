using Godot;
using System;

public partial class Actor : CharacterBody3D
{
	[Export] protected float FallingAcceleration;
	[Export] protected int MaxHealth;
	[Export] protected bool PlayAnimationOnDeath;
	[Export] protected string DeathAnimation;

	public enum Teams
	{
		None,
		Player,
		Enemy
	}

	[Export] public Teams Team;

	protected int Health;
	public bool Dying;
	public bool Dead;
	protected bool Collided;

	
	// Node Functions //

	public override void _Ready()
	{
		base._Ready();

		Dying = false;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		if (Dead)
		{
			QueueFree();
		}

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
		OnHurt(damage);
		
		// if health 0 or below, kill
		if (Health <= 0)
		{
			if (PlayAnimationOnDeath)
			{
				if (!Dying)
				{
					GetNode<AnimationPlayer>("AnimationPlayer")
						.Play(DeathAnimation);
				}
			}
			else
			{
				Kill();
			}

			Dying = true;
		}
	}

	// an overridable function to allow actors to react to being hurt
	protected virtual void OnHurt(int damage)
	{
		Health -= damage;
	}

	public void Kill()
	{
		Dead = true;
	}
}
