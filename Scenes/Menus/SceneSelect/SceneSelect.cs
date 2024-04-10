using Godot;
using System;

public partial class SceneSelect : Menu
{
	// Properties //

	public Global.SceneTypes CurrentSceneType;

	[Export] private Label _title;
	[Export] private Label _summary;
	[Export] private Label _timeToBeat;

	[Export] private Button _next;
	[Export] private Button _previous;
	[Export] private Button _timeTrial;


	// Node Functions //

	public override void _Ready()
	{
		base._Ready();


		CurrentSceneType = Global.Scenes[Global.CurrentSceneIndex];
		Global.LoadSceneStats(Global.CurrentSceneIndex);


		// if first time playing the level, hide the level stats such as how many jars collected
		// and time trial reqs as well as next level button
		if (Global.CurrentSceneIndex == Global.SaveFile.SceneProgress)
		{
			_next.Visible = false;
			_timeToBeat.Visible = false;
			_timeTrial.Visible = false;
			_summary.Visible = false;
		}

		// if first scene, remove previous button
		if (Global.CurrentSceneIndex == 0)
		{
			_previous.Visible = false;
		}


		switch (CurrentSceneType)
		{
			case Global.SceneTypes.Level:

				_title.Text = Global.CurrentLevelStats.Title;

				if (_summary.Visible)
				{
					if (Global.SaveFile.MaxJars.ContainsKey(Global.CurrentSceneIndex))
					{
						_summary.Text
							= $"Most Jars: {Global.SaveFile.MaxJars[Global.CurrentSceneIndex]}"
							+ $"/{Global.CurrentLevelStats.NumberOfCollectables}\n";
					}

					if (Global.SaveFile.TimeTrialTimes.ContainsKey(Global.CurrentSceneIndex))
					{
						_summary.Text
							+= $"Best Time: {Global.SaveFile.TimeTrialTimes[Global.CurrentSceneIndex]}s";
					}
				}

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

		Global.LoadScene(Global.CurrentSceneIndex);
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
