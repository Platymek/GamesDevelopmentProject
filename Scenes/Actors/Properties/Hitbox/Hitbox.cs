using Godot;
using System;
using static Actor;

public partial class Hitbox : Area3D
{
	[Export] private int _bounceHeight = 0;
	[Export] private bool _ignoreTeam = false;
	[Export] private AnimationPlayer _animationPlayer;
	[Export] private string _playAnimation;
	[Export] private bool _playOnce;

	private bool _played;
	private Actor _owner;

	public override void _Ready()
	{
		base._Ready();

		_owner = Owner as Actor;
	}

	private void OnAreaEntered(Area3D area)
	{
		// check that area is hurtbox
		if (area is not Hurtbox hurtbox) return;
		if (hurtbox.Damage == 0) return;

		if (_animationPlayer != null && (!_played || !_playOnce))
		{
			_played = true;
			_animationPlayer.Play(_playAnimation);
		}


		// check team
		if (!_ignoreTeam)
		{
			if (area.Owner is Actor actor && hurtbox.CopyOwnerTeam)
			{
				if (_owner.Team == actor.Team || _ignoreTeam) return;
			}
			else
			{
				if (_owner.Team == hurtbox.Team || _ignoreTeam) return;
			}
		}

		if (area.Owner is Actor a)
		{
			// if not same team, bounce actor
			a.Jump(_bounceHeight);
		}
		
		_owner.Hurt(hurtbox.Damage);
		
		GD.Print($"{_owner} was hurt by {area.Owner.Name} with {hurtbox.Damage} damage");
	}
}
