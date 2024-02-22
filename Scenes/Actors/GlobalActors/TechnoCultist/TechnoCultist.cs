using Godot;
using System;

public partial class TechnoCultist : Actor
{
	// Properties //

	AnimationPlayer _animationPlayer;
	Detector _detector;

	private string _state = "idle";

	[Export] private string State
	{
		get => _state;

		set
		{
			switch (value)
			{
				case "idle":

					Halt();
					
					break;
			}
			
			_animationPlayer?.Play(value);
			_state = value;
		}
	}


	// Node Functions //

	public override void _Ready()
	{
		base._Ready();

		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		_detector = GetNode<Detector>("Detector");

		State = "idle";
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		switch (State)
		{
			case "run":

				Node3D nodeToChase = _detector.DetectedActors[0];

				var chasePosition = new Vector2(
					nodeToChase.Position.Z, nodeToChase.Position.X);
				
				var myPosition = new Vector2(
					Position.Z, Position.X);
				
				TurnTowards(chasePosition.AngleToPoint(myPosition), 
					delta);
				
				Accelerate(delta);

				break;
		}
	}


	// Signals //

	private void OnDetectorDetected()
	{
		State = "shock";
	}

	private void OnDetectorNotDetecting()
	{
		GD.Print("hi");
		
		State = "idle";
	}

}
