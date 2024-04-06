using Godot;
using System;

public partial class YouWin : Menu
{
    public override void _Ready()
    {
        base._Ready();

        Global.Progress();
    }

    private void NextLevelPressed()
	{
		Global.CurrentSceneIndex++;
		Global.LoadScene(Global.CurrentSceneIndex);
	}
}

