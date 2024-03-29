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
	private SceneTypes _currentSceneType;


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

	public void LoadSceneStats(int index)
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
                    _currentSceneType = SceneTypes.Level;

                    break;


                case SceneTypes.Story:

                    CurrentStoryStats = GetStoryStats(storyIndex);

					storyIndex++;
                    _currentSceneType = SceneTypes.Story;

                    break;
            }
		}
    }

	public void LoadScene(int index)
	{
		LoadSceneStats(index);

        switch (_currentSceneType)
        {
            case SceneTypes.Level:

                LoadLevel(CurrentLevelStats);

                break;


            case SceneTypes.Story:

                LoadStory(CurrentStoryStats);

                break;
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

    public void SaveTimeTrial(int time)
    {
        if (SaveFile.TimeTrialTimes.ContainsKey(CurrentSceneIndex))
        {
            if (SaveFile.TimeTrialTimes[CurrentSceneIndex] > time)
            {
                SaveFile.TimeTrialTimes[CurrentSceneIndex] = time;
                Save();
            }

            return;
        }

        SaveFile.TimeTrialTimes.Add(CurrentSceneIndex, time);
        Save();
    }

    public void SaveMaxJars(int maxJars)
    {
        if (SaveFile.MaxJars.ContainsKey(CurrentSceneIndex))
        {
            if (SaveFile.MaxJars[CurrentSceneIndex] < maxJars)
            {
                SaveFile.MaxJars[CurrentSceneIndex] = maxJars;
                Save();
            }

            return;
        }

        SaveFile.MaxJars.Add(CurrentSceneIndex, maxJars);
        GD.Print(maxJars);
        Save();
    }

    public void Save()
    {
        ResourceSaver.Save(SaveFile, "user://save.tres");
    }

    public void Load()
    {
		if (!FileAccess.FileExists("user://save.tres")) return;

        SaveFile = ResourceLoader.Load<SaveFile>("user://save.tres");
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
