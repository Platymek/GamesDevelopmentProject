using Godot;
using System;

public partial class YouWin : Menu
{
    private void NextLevelPressed()
    {
        Global.NextSceneIfPossible();
        Global.LoadScene(Global.CurrentSceneIndex);
	}
}
