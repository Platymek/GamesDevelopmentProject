using Godot;
using System;

public partial class StoryStats : Node
{
	[Export] public Texture2D[] Frames;
	[Export] public string[] Captions;
	[Export] public string Title;
	[Export] public string Summary;
}
