using Godot;
using System;

public partial class YouWinTimeTrial : Menu
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();


		GetNode<Label>("AspectRatioContainer/CenterContainer/VBoxContainer/Time").Text 
			= $"{Mathf.Round(Global.PreviousTime)}s";

		GetNode<Control>("AspectRatioContainer/CenterContainer/VBoxContainer/Subtitle")
			.Visible = Global.PreviousTime > Global.CurrentLevelStats.Time;

		GetNode<Label>("AspectRatioContainer/CenterContainer/VBoxContainer/Subtitle/TimeRequirement").Text
			= $"{Mathf.Round(Global.CurrentLevelStats.Time)}s?";
	}
}
