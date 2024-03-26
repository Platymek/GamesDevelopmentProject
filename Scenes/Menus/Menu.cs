using Godot;
using System;

public partial class Menu : Control
{
	// Properties //

	[Signal] public delegate void PausePressedEventHandler();
	[Export] public Button FocusedButton;

	protected Global Global;
	protected bool TimerOn;


	// Node Functions //

	public override void _Ready()
	{
		base._Ready();

		Global = GetNode<Global>("/root/Global");
		Focus();
	}


	// Signals //

	private void OnContinueGamePressed()
	{
		Global.LoadMenu(Global.Menus.SceneSelect);
	}

	private void OnRetryPressed()
	{
		Global.LoadLevel(Global.CurrentLevelStats);
	}

	private void OnStoryPressed(int index)
	{
		Global.LoadStory(index);
	}

	private void OnExitPressed()
	{
		Global.Exit();
	}

	private void OnMenuPressed()
	{
		Global.LoadMenu(Global.Menus.MainMenu);
	}

	private void OnTimerToggled(bool toggledOn)
	{
		TimerOn = toggledOn;
	}

	private void OnContinuePressed()
	{
		EmitSignal(SignalName.PausePressed);
	}

	public void Focus()
	{
		FocusedButton.GrabFocus();
	}
}
