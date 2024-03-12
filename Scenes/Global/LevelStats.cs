using Godot;
using System;

public partial class LevelStats : Node
{
	[Export] public PackedScene Level;
	[Export] public double Time;
	[Export] public string Title;
	[Export] public int NumberOfCollectables = 2;
}
