using Godot;
using System;

public partial class Actor : CharacterBody3D
{
	[Export] protected float FallingAcceleration;
	[Export] protected int MaxHealth;


	public enum Teams
	{
		None,
		Player,
		Enemy
	}

	[Export] public Teams Team;

	protected int Health;
	public bool Dying;

	
	// Node Functions //

	public override void _Ready()
	{
		base._Ready();

		Dying = false;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		if (Dying)
		{
			QueueFree();
		}
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
			Dying = true;
		}
	}

	// an overridable function to allow actors to react to being hurt
	protected virtual void OnHurt(int damage)
	{
		Health -= damage;
	}
}
