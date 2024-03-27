using Godot;
using System;
using System.Collections.Generic;

public partial class Level : Node3D
{
	[Signal] public delegate void JarCollectedEventHandler(int index);

	private Global _global;
	private HUD _hud;
	private Menu _pause;
	public Checkpoint LastCheckpoint;

	public double Time;
	public bool[] Collectables;
	public int TotalCollected;

	private bool _paused;
	private bool Paused
	{
		get => _paused;

		set
		{
			_pause.Visible = value;
			_pause.Focus();
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
        GetNode<Node3D>("ColourJars").Visible = !_global.TimeTrialMode;

        Collectables = new bool[_global.CurrentLevelStats.NumberOfCollectables];

		Time = 0;
		TotalCollected = 0;
		Paused = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);


		if (Input.IsActionJustPressed("pause"))
		{
			Pause();
		}
		
		if (!Paused)
		{
			Time += delta;
		}
	}


	// Signals //

	private void OnPitEntered(Area3D area)
	{
		if (area.Owner is not Actor actor) return;

		if (actor is Player player)
		{
			player.Respawn(LastCheckpoint.Position);
			player.Hurt();
			LastCheckpoint._cameraHint.OnNotDetecting();
		}
	}
	
	// when the goal collision has been entered, send the player to the win screen
	private void OnGoalAreaEntered()
	{
		_global.PreviousTime = Time;
		_global.Progress();

		if (!_global.TimeTrialMode)
        {
			GD.Print(TotalCollected);
			_global.SaveMaxJars(TotalCollected);
            _global.LoadMenu(Global.Menus.YouWin);
			return;
        }

        _global.SaveTimeTrial((int)Time);
        _global.LoadMenu(Global.Menus.YouWinTimeTrial);
    }

	
	// Other Functions //

	public void Lose()
	{
		_global.LoadMenu(Global.Menus.YouLose);
	}

	public void CollectJar(int index)
	{
		GD.Print(index);

		Collectables[index] = true;
		TotalCollected++;

        EmitSignal(SignalName.JarCollected, index);
	}

	public void Pause()
	{
		Paused = !Paused;
	}
}
