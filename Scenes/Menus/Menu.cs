using Godot;
using System;

public partial class Menu : Control
{
	// Properties //

	protected Global Global;
	protected bool TimerOn;


	// Node Functions //

	public override void _Ready()
	{
		base._Ready();

		Global = GetNode<Global>("/root/Global");
	}


	// Signals //

	private void OnPlayPressed()
	{
		Global.TimerOn = TimerOn;

		// load ProtoLevel
		Global.LoadLevel(0, 0);
	}

	private void OnExitPressed()
	{
		Global.Exit();
	}

	private void OnTimerToggled(bool toggledOn)
	{
		TimerOn = toggledOn;
	}
}
