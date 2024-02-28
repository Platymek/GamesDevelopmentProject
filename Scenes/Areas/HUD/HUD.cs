using Godot;
using System;

public partial class HUD : Control
{
	[Export] private Player _player;

	private TextureProgressBar _health;
	private TextureProgressBar _shells;
	private Label _time;
	private Level _level;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();

		_health = GetNode<TextureProgressBar>("Health");
		_shells = GetNode<TextureProgressBar>("Shells");
		_time	= GetNode<Label>("Time");
		_level	= GetParent<Level>();

		_time.Visible = GetNode<Global>("/root/Global").TimerOn;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);

		_health.Value = _player.Health;
		_shells.Value = _player.Ammo;

		_time.Text = $"Time: {Mathf.Round(_level.Time)}";
	}
}
