using Godot;
using System;

public partial class ColourJar : Node3D
{
	[Export] bool _unlocked = true;
	[Export] AudioStreamPlayer3D _sfx;
	bool _readyToBeUnlocked = false;


	// Node Functions //

	public override void _Ready()
	{
		base._Ready();

		Visible = _unlocked;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		if (!Visible || !_readyToBeUnlocked) return;

		Visible = false;
		_sfx.Play();

		GetOwner<Level>().CollectJar(GetIndex());
	}


	// Signals //

	private void OnDetectorDetected()
	{
		_readyToBeUnlocked = true;
	}

	private void OnDetectorNotDetecting()
	{
		_readyToBeUnlocked = false;
	}


	// Other Functions //

	private void Unlock()
	{
		Visible = true;
	}
}
