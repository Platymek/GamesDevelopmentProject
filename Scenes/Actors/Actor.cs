using Godot;
using System;

public partial class Actor : CharacterBody3D
{
	[Export] protected float FallingAcceleration;
	[Export] protected int MaxHealth;

	protected int Health;
	
	// jump a specific height which is calculated using the falling
	// acceleration of the acter
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
			QueueFree();
		}
	}

	// an overridable function to allow actors to react to being hurt
	protected virtual void OnHurt(int damage)
	{
		Health -= damage;
	}
}
