using Godot;
using System;

public partial class Detectable : Area3D
{
	[Export] public Actor.Teams Team;

	public override void _Ready()
	{
		base._Ready();

		if (Owner is Actor actor)
		{
			Team = actor.Team;
		}
	}
}
