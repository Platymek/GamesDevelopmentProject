using Godot;
using System;

public partial class SceneSelect : Menu
{
	// Properties //

	public Global.SceneTypes CurrentSceneType;

	private Label _title;
	private Label _summary;
	private Label _timeToBeat;

	private Button _next;
	private Button _previous;
	private Button _timeTrial;


	// Node Functions //

	public override void _Ready()
	{
		base._Ready();


		_title = GetNode<Label>("AspectRatioContainer/CenterContainer/VBoxContainer" +
			"/CenterContainer/PanelContainer/MarginContainer/Buttons/Title");
		_summary = GetNode<Label>("AspectRatioContainer/CenterContainer/VBoxContainer" +
			"/CenterContainer/PanelContainer/MarginContainer/Buttons/Summary");
		_timeToBeat = GetNode<Label>("AspectRatioContainer/CenterContainer/VBoxContainer" +
			"/CenterContainer/PanelContainer/MarginContainer/Buttons/CenterContainer/VBoxContainer/TimeToBeat");

		_next = GetNode<Button>("AspectRatioContainer/CenterContainer/VBoxContainer/CenterContainer" +
			"/PanelContainer/MarginContainer/Buttons/CenterContainer/VBoxContainer/Next");
		_previous = GetNode<Button>("AspectRatioContainer/CenterContainer/VBoxContainer" +
			"/CenterContainer/PanelContainer/MarginContainer/Buttons/CenterContainer/VBoxContainer/Previous");
		_timeTrial = GetNode<Button>("AspectRatioContainer/CenterContainer/VBoxContainer" +
			"/CenterContainer/PanelContainer/MarginContainer/Buttons/CenterContainer/VBoxContainer/TimeTrial"); 
		

		CurrentSceneType = Global.Scenes[Global.CurrentSceneIndex];
		Global.LoadSceneIndex(Global.CurrentSceneIndex);


		if (Global.CurrentSceneIndex == Global.SaveFile.SceneProgress)
		{
			_next.Visible = false;
		}

		if (Global.CurrentSceneIndex == 0)
		{
			_previous.Visible = false;
		}


		switch (CurrentSceneType)
		{
			case Global.SceneTypes.Level:

				_title.Text = Global.CurrentLevelStats.Title;

				_summary.Text 
					= $"Most Jars: 0/{Global.CurrentLevelStats.NumberOfCollectables}\n"
					+ $"Best Time: 0s";

				_timeToBeat.Text
					= $"Time to beat: {Global.CurrentLevelStats.Time}s";

				break;

			case Global.SceneTypes.Story:

				_timeToBeat.Visible = false;
				_timeTrial.Visible = false;

				_title.Text = Global.CurrentStoryStats.Title;
				_summary.Text = Global.CurrentStoryStats.Summary;

				break;
		}
	}


	// Other Functions //

	private void Play(bool timeTrialMode = false)
	{
		Global.TimeTrialMode = timeTrialMode;


		switch (CurrentSceneType)
		{
			case Global.SceneTypes.Level:

				Global.LoadLevel(Global.CurrentLevelStats);

				break;

			case Global.SceneTypes.Story:

				Global.LoadStory(Global.CurrentStoryStats);

				break;
		}
	}

	private void Next()
	{
		Global.CurrentSceneIndex++;

		Global.LoadMenu(Global.Menus.SceneSelect);
	}

	private void Previous()
	{
		Global.CurrentSceneIndex--;

		Global.LoadMenu(Global.Menus.SceneSelect);
	}
}
