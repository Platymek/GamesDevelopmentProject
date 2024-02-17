using Godot;
using System;

public partial class Shell : Actor
{
	// Properties //

	[Export] float _speed;
	[Export] float _distance;

	private Timer _timer;


	// Node Functions //

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();

		_timer = GetNode<Timer>("Timer");

		_timer.WaitTime = _distance / _speed;
		_timer.Start();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);

		TranslateObjectLocal(Vector3.Forward * _speed * (float)delta);
	}

	private void OnTimerTimeout()
	{
		QueueFree();
	}
}
