using Godot;
using System;

public partial class Story : Menu
{
	// Properties //

	private int _currentFrameIndex;

	private TextureRect _frame;
	private Label _caption;


	// Node Functions //

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();


		_frame = GetNode<TextureRect>(
			"AspectRatioContainer/CenterContainer/VBoxContainer/CenterContainer/PanelContainer" +
			"/MarginContainer/Buttons/PanelContainer/MarginContainer/CenterContainer2/Frame");

		_caption = GetNode<Label>(
			"AspectRatioContainer/CenterContainer/VBoxContainer/CenterContainer/PanelContainer" +
			"/MarginContainer/Buttons/Caption");

        _currentFrameIndex = 0;

		LoadNextFrame();
    }


	// Other Functions //

	private void LoadNextFrame()
    {
		// if reached end of story, load menu on next scene and exit
		if (_currentFrameIndex >= Global.CurrentStoryStats.Frames.Length)
        {
            Global.Progress();
            Global.NextSceneIfPossible();
            Global.LoadMenu(Global.Menus.SceneSelect);

            return;
		}


		// if not reached end of story, load next frame
        _frame.Texture = Global.CurrentStoryStats.Frames[_currentFrameIndex];
        _caption.Text = Global.CurrentStoryStats.Captions[_currentFrameIndex];

        _currentFrameIndex++;
    }
}
