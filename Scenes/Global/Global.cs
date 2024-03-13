using Godot;
using System;

public partial class Global : Node
{
	// Properties //

	private Node AreasNode;

	public LevelStats CurrentLevelStats;
	public int CurrentLevel;
	public int CurrentArea;
	public bool TimerOn;
	public double PreviousTime;

	public enum Menus
	{
		MainMenu,
		YouWin,
		YouLose,
	}


	// Node Functions //

	public override void _Ready()
	{
		base._Ready();

		AreasNode = GetNode("Areas");
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		if (Input.IsActionJustPressed("Fullscreen"))
		{
			// toggle fullscreen

			DisplayServer.WindowSetMode(
				
				DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Windowed

				? DisplayServer.WindowMode.Fullscreen
				: DisplayServer.WindowMode.Windowed);
		}

		if(Input.IsActionJustPressed("Reset"))
		{
			TimerOn = true;
			LoadLevel(1, 0);
		}
	}


	// Other Functions //

	public PackedScene GetMenu(Menus menu)
	{
		switch (menu)
		{
			case Menus.MainMenu: 

				return ResourceLoader.Load<PackedScene>("res://Scenes/Menus/MainMenu/MainMenu.tscn");

			case Menus.YouWin:

				return ResourceLoader.Load<PackedScene>("res://Scenes/Menus/YouWin/YouWin.tscn");

			case Menus.YouLose:

				return ResourceLoader.Load<PackedScene>("res://Scenes/Menus/YouLose/YouLose.tscn");

			default:

				return null;
		}
	}

	public void LoadMenu(Menus menu)
	{
		GetTree().ChangeSceneToPacked(GetMenu(menu));
	}

	public LevelStats GetLevelStats(int area, int level)
	{
		return AreasNode.GetChild(area).GetChild<LevelStats>(level);
	}

	public void LoadLevel(int area, int level)
	{
		CurrentLevelStats = GetLevelStats(area, level);

		CurrentLevel = level;
		CurrentArea = area;

		GetTree().ChangeSceneToPacked(CurrentLevelStats.Level);
	}

	public void Exit()
	{
		GetTree().Quit();
	}
}
