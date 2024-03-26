using Godot;
using System;

public partial class HUD : Control
{
	[Export] private Player _player;

	private TextureProgressBar _health;
	private TextureProgressBar _shells;
	private Label _time;
	private Level _level;
	private Control _emptyJars;
	private Control _fullJars;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();

		_health = GetNode<TextureProgressBar>("Health");
		_shells = GetNode<TextureProgressBar>("Shells");
		_time	= GetNode<Label>("Time");
		_level	= GetParent<Level>();
		_emptyJars = GetNode<Control>("EmptyJars");
		_fullJars = GetNode<Control>("FullJars");

		_time.Visible = GetNode<Global>("/root/Global").TimeTrialMode;
        _emptyJars.Visible = !GetNode<Global>("/root/Global").TimeTrialMode;
        _fullJars.Visible = !GetNode<Global>("/root/Global").TimeTrialMode;


        for (int i = 1; 
			i < GetNode<Global>("/root/Global").CurrentLevelStats.NumberOfCollectables; 
			i++)
		{
			_emptyJars.AddChild(_emptyJars.GetChild(0).Duplicate());
			_fullJars .AddChild(_fullJars .GetChild(0).Duplicate());
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);

		_health.Value = _player.Health;
		_shells.Value = _player.Ammo;

		_time.Text = $"Time: {Mathf.Round(_level.Time)}";
	}
	
	private void OnLevelJarCollected(long index)
	{
		_fullJars.GetChild<Control>((int)index).Visible = true;

	}
}
