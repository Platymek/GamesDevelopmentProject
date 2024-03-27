using Godot;
using System;

public partial class YouWin : Menu
{
	private void NextLevelPressed()
	{
		Global.CurrentSceneIndex++;
		Global.LoadScene(Global.CurrentSceneIndex);
	}
}

