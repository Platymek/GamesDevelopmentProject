using Godot;
using System;

public partial class YouWin : Menu
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();

		GetNode<Label>("AspectRatioContainer/CenterContainer/VBoxContainer/Time").Text 
			= "" + Mathf.Round(Global.PreviousTime);

		GetNode<Node3D>("AspectRatioContainer/CenterContainer/VBoxContainer/Subtitle")
			.Visible = Global.PreviousTime > Global.TimeRequirement;
	}
}
