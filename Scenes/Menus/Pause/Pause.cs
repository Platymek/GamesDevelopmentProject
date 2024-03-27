using Godot;
using System;

public partial class Pause : Menu
{
	[Export] private string[] _hints;

	private Label _hintLabel;
    private int _hintIndex;

    public override void _Ready()
    {
        base._Ready();

        _hintLabel = GetNode<Label>("AspectRatioContainer/CenterContainer" +
            "/VBoxContainer/CenterContainer/PanelContainer/MarginContainer/Buttons/HintLabel");

        _hintIndex = 0;
    }

    private void OnVisibilityChanged()
	{
		if (!Visible) return;

        NextHint();
	}

    private void NextHint()
    {
        _hintLabel.Text = $"Hint: {_hints[_hintIndex]}";

        _hintIndex++;

        if (_hintIndex == _hints.Length) 
        {
            _hintIndex = 0;
        }
    }
}
