using Godot;
using System;

public partial class HUD : Control
{
	[Export] private Player _player;

	private TextureProgressBar _health;
	private TextureProgressBar _shells;
	private Label _reloadHint;
	private Label _time;
	private Level _level;
	private Control _emptyJars;
	private Control _fullJars;
    private Global _global;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		base._Ready();


		_health = GetNode<TextureProgressBar>("Health");
		_shells = GetNode<TextureProgressBar>("Shells");
		_reloadHint = _shells.GetNode<Label>("Hint");

        _time	= GetNode<Label>("Time");
		_level	= GetParent<Level>();
		_emptyJars = GetNode<Control>("EmptyJars");
		_fullJars = GetNode<Control>("FullJars");

        _global = GetNode<Global>("/root/Global");
        _time.Visible = _global.TimeTrialMode;
        _emptyJars.Visible = !_global.TimeTrialMode;
        _fullJars.Visible = !_global.TimeTrialMode;


		// spawn the number of jars in the level on the HUD
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

		_reloadHint.Visible = _player.Ammo == 0;

        _time.Text = $"Time: {Mathf.Round(_level.Time)}";
	}
	
	private void OnLevelJarCollected(long index)
	{
		_fullJars.GetChild<Control>((int)index).Visible = true;
	}
}
