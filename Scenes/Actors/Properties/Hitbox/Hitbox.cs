using Godot;
using System;

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
		
		if (_animationPlayer != null && (!_played || !_playOnce))
        {
            _played = true;
            _animationPlayer.Play(_playAnimation);
        }

		// check that owner is actor
		if (area.Owner is not Actor actor) return;
		
		// check that actor is not detecting itself
		if (actor.Team == _owner.Team && !_ignoreTeam) return;

		actor.Jump(_bounceHeight);

		if (hurtbox.Damage == 0) return;
		
		_owner.Hurt(hurtbox.Damage);
		
		GD.Print($"{_owner} was hurt by {actor}");
	}
}
