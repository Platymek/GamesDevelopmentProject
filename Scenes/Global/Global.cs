using Godot;
using Godot.Collections;

public partial class Global : Node
{
	// Properties //

	public enum SceneTypes
    {
		Level,
		Story,
	}

	[Export] public Array<SceneTypes> Scenes;
	[Export] public SaveFile SaveFile;
	public int CurrentSceneIndex;


    private Areas AreasNode;
    private Node StoriesNode;

    public LevelStats CurrentLevelStats;
	public StoryStats CurrentStoryStats;
	public int CurrentLevel;
	public int CurrentArea;
	public int CurrentStory;
	public bool TimeTrialMode;
	public double PreviousTime;

	public enum Menus
	{
		MainMenu,
		YouWin,
		YouLose,
		SceneSelect,
        YouWinTimeTrial,
    }


	// Node Functions //

	public override void _Ready()
	{
		base._Ready();

		AreasNode = GetNode<Areas>("Areas");
        StoriesNode = GetNode("Stories");
		CurrentSceneIndex = 0;

		if (SaveFile == null)
		{
			SaveFile = new SaveFile();
		}
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

		if (Input.IsActionJustPressed("Reset"))
		{
			LoadLevel(CurrentLevelStats);
		}
	}


    // Other Functions //

    public void Exit()
    {
        GetTree().Quit();
    }

	public void LoadSceneIndex(int index)
	{
		int levelIndex = 0;
		int storyIndex = 0;

		for (int i = 0; i <= index; i++)
		{
			SceneTypes sceneType = Scenes[i];

			switch (sceneType)
			{
				case SceneTypes.Level:

					CurrentLevelStats = GetLevelStats(
						(int)AreasNode.Levels[levelIndex].X,
                        (int)AreasNode.Levels[levelIndex].Y);

					levelIndex++;

                    break;


                case SceneTypes.Story:

                    CurrentStoryStats = GetStoryStats(storyIndex);

					storyIndex++;

                    break;
            }
		}
	}

	public void Progress()
	{
        if (CurrentSceneIndex == SaveFile.SceneProgress
            && CurrentSceneIndex != Scenes.Count - 1)
        {
            SaveFile.SceneProgress++;
        }
    }


	// Menu Functions //

    public PackedScene GetMenu(Menus menu)
	{
		switch (menu)
		{
			case Menus.MainMenu: 

				return ResourceLoader.Load<PackedScene>("res://Scenes/Menus/MainMenu/MainMenu.tscn");

            case Menus.YouWinTimeTrial:

                return ResourceLoader.Load<PackedScene>("res://Scenes/Menus/YouWinTimeTrial/YouWinTimeTrial.tscn");

            case Menus.YouWin:

				return ResourceLoader.Load<PackedScene>("res://Scenes/Menus/YouWin/YouWin.tscn");

			case Menus.YouLose:

				return ResourceLoader.Load<PackedScene>("res://Scenes/Menus/YouLose/YouLose.tscn");

			case Menus.SceneSelect:

                return ResourceLoader.Load<PackedScene>("res://Scenes/Menus/SceneSelect/SceneSelect.tscn");

            default:

				return null;
		}
	}

	public void LoadMenu(Menus menu)
	{
		GetTree().Paused = false;
		GetTree().ChangeSceneToPacked(GetMenu(menu));
    }


    // Level Functions //

    public LevelStats GetLevelStats(int area, int level)
    {
        return AreasNode.GetChild(area).GetChild<LevelStats>(level);
    }

    public void LoadLevel(LevelStats levelStats)
    {
        CurrentLevelStats = levelStats;

        GetTree().ChangeSceneToPacked(levelStats.Level);
    }

    public void LoadLevel(int area, int level)
    {
        CurrentLevelStats = GetLevelStats(area, level);

        CurrentLevel = level;
        CurrentArea = area;

        LoadLevel(CurrentLevelStats);
    }


	// Story Functions //

    public StoryStats GetStoryStats(int index)
    {
        return StoriesNode.GetChild<StoryStats>(index);
    }

    public void LoadStory(StoryStats storyStats)
    {
        CurrentStoryStats = storyStats;

        GetTree().ChangeSceneToPacked(
			ResourceLoader.Load<PackedScene>(
				"res://Scenes/Menus/Story/Story.tscn"));
    }

    public void LoadStory(int index)
    {
        CurrentStoryStats = GetStoryStats(index);

        CurrentStory = index;

        LoadStory(CurrentStoryStats);
    }
}
