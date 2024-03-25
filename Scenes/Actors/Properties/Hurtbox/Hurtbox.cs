using Godot;
using System;

public partial class Hurtbox : Area3D
{
	[Export] public bool CopyOwnerTeam = true;
	[Export] public Actor.Teams Team = Actor.Teams.None;
    [Export] public int Damage = 1;
    [Signal] public delegate void HurtEventHandler(Actor victim);

    public override void _Ready()
    {
        base._Ready();


        if (!CopyOwnerTeam || Owner is not Actor actor) return;

        Team = actor.Team;
    }

    public void ConfirmHurt()
    {
        EmitSignal(SignalName.Hurt);
    }

    public void ConfirmHurt(Actor victim)
    {
        EmitSignal(SignalName.Hurt, victim);
    }
}
