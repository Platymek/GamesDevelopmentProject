using Godot;
using System;

public partial class Hitbox : Area3D
{
	[Export] private bool CopyOwnerTeam = true;
	[Export] private Actor.Teams Team;
	[Export] private float _bounceHeight;

	[Signal] public delegate void HurtEventHandler();

	public override void _Ready()
	{
		base._Ready();


        if (!CopyOwnerTeam || Owner is not Actor actor) return;

		Team = actor.Team;
    }

	private void OnAreaEntered(Area3D area)
	{
		if (area is not Hurtbox hurtbox) return;
		if (Team == hurtbox.Team && Team != Actor.Teams.None) return;

		string myName = Name;
		string yourName = hurtbox.Name;

		if (Owner is not Actor me)
		{
			EmitSignal(SignalName.Hurt);
			hurtbox.ConfirmHurt();
		}
		else
		{
			me.Hurt(hurtbox.Damage);
			myName = me.Name;

            hurtbox.ConfirmHurt(me);
		}

		if (hurtbox.Owner is Actor you)
		{
			yourName = you.Name;
            you.Jump(_bounceHeight);
        }

		GD.Print($"{myName}, of team {Team}, was hurt by {yourName}, of team {hurtbox.Team}, " +
			$"with {hurtbox.Damage} damage and {_bounceHeight} bounce height");
	}
}
