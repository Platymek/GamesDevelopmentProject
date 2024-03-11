using Godot;
using System;

public partial class Level : Node3D
{
	private Global _global;
	private Control _hud;
	private Control _pause;

	public double Time;

	private bool _paused;
	private bool Paused
	{
		get => _paused;

		set
		{
			_hud.Visible = !value;
			_pause.Visible = value;
			GetTree().Paused = value;

			_paused = value;
		}
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();

		_global = GetNode<Global>("/root/Global");
		_hud = GetNode<HUD>("Hud");
		_pause = GetNode<Menu>("Pause");

		Time = 0;
		Paused = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);


		if (Input.IsActionJustPressed("pause"))
		{
			Paused = !Paused;
		}
		
		if (!Paused)
		{
		    Time += delta;
		}
	}

	// Signals //

	private void OnPitEntered(Area3D area)
	{
		GD.Print("hello");

		if (area.Owner is not Actor actor) return;

		if (actor is Player player)
		{
			player.Respawn();
			player.Hurt();
		}
		else
		{
			actor.Kill();
		}
	}
	
	// when the goal collision has been entered, send the player to the win screen
	private void OnGoalAreaEntered()
	{
		_global.PreviousTime = Time;
		_global.LoadMenu(Global.Menus.YouWin);
	}

	
	// Other Functions //

	public void Lose()
	{
		_global.LoadMenu(Global.Menus.YouLose);
	}
}
