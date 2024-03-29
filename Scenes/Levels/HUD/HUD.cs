using Godot;
using System;

public partial class HUD : Control
{
	[Export] private Player _player;

	[Export] private TextureProgressBar _health;
	[Export] private TextureProgressBar _shells;
	[Export] private Label _reloadHint;
	[Export] private Label _time;
	[Export] private Level _level;
	[Export] private Control _emptyJars;
	[Export] private Control _fullJars;

    private Global _global;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		base._Ready();


        _global = GetNode<Global>("/root/Global");
        _time.Visible = _global.TimeTrialMode;
        _emptyJars.Visible = !_global.TimeTrialMode;
        _fullJars.Visible = !_global.TimeTrialMode;


		// spawn the number of jars in the level on the HUD
        for (int i = 1; 
			i < _global.CurrentLevelStats.NumberOfCollectables; 
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

		_reloadHint.Visible = _player.Ammo == 0;

        _time.Text = $"Time: {Mathf.Round(_level.Time)}";
	}
	
	private void OnLevelJarCollected(long index)
	{
		_fullJars.GetChild<Control>((int)index).Modulate = Colors.White;
	}
}
