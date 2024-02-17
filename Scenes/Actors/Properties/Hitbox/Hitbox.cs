using Godot;
using System;

public partial class Hitbox : Area3D
{
	[Export] private int _bounceHeight = 0;
	private Actor _owner;

	public override void _Ready()
	{
		base._Ready();

		_owner = Owner as Actor;
	}

	private void OnAreaEntered(Area3D area)
	{
		// check that owner is actor
		if (area.Owner is not Actor actor) return;
		
		// check that are is hurtbox
		
		if (area is not Hurtbox hurtbox) return;
		
		// check that actor is not detecting itself
		if (actor == _owner) return;

		actor.Jump(_bounceHeight);

		if (hurtbox.Damage == 0) return;
		
		_owner.Hurt(hurtbox.Damage);
		
		GD.Print($"{_owner} was hurt by {actor}");
	}
}