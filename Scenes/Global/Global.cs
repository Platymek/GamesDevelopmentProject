using Godot;
using System;

public partial class Global : Node
{
    // Properties //

    private Node AreasNode;

    public LevelStats CurrentLevelStats;
    public bool TimerOn;
    public double Time;

    public enum Menus
    {
        MainMenu,
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
    }


    // Other Functions //

    public PackedScene GetScene(Menus menu)
    {
        switch (menu)
        {
            case Menus.MainMenu: return ResourceLoader.Load<PackedScene>("res://Scenes/Menus/MainMenu/MainMenu.tscn");
        }

        return null;
    }

    public LevelStats GetLevelStats(int area, int level)
    {
        return AreasNode.GetChild(area).GetChild<LevelStats>(level);
    }

    public void LoadLevel(int area, int level)
    {
        LevelStats levelStats = GetLevelStats(area, level);

        GetTree().ChangeSceneToPacked(levelStats.Level);
    }

    public void Exit()
    {
        GetTree().Quit();
    }
}
