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

	private void OnRetryPressed()
	{
		Global.LoadLevel(Global.CurrentLevelStats);
	}

	private void OnPlayPressed(int area, int level)
	{
		Global.TimerOn = TimerOn;

		// load ProtoLevel
		Global.LoadLevel(area, level);
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
