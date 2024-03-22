using Godot;
using System;

public partial class Hurtbox : Area3D
{
	[Export] public bool CopyOwnerTeam = true;
	[Export] public Actor.Teams Team = Actor.Teams.None;
	[Export] public int Damage = 1;
}
