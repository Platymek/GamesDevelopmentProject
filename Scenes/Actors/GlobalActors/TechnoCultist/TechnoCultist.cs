using Godot;
using System;
using System.Diagnostics;

public partial class TechnoCultist : Actor
{
	// Properties //


	private string _state = "idle";

	[Export]
	protected virtual string State
	{
		get => _state;

		set
		{
			// if the state is changes, stop all velocity
			Halt();

			_state = value;

			// if the animation player exists, play the animation
			// with the same name as the state
			if (AnimationPlayer == null) return;

			if (AnimationPlayer.HasAnimation(value))
			{
				AnimationPlayer.Play(value);
			}
			else
			{
				AnimationPlayer.Play("RESET");
			}
		}
	}


	[ExportGroup("Nodes")]

	[Export] protected AnimationPlayer AnimationPlayer;
	[Export] private Detector _chaseDetector;
	[Export] private Detector _attackDetector;
	[Export] private Label3D _debugLabel;


	// Node Functions //

	public override void _Process(double delta)
	{
		base._Process(delta);

		switch (State)
		{
			case "run":

				// if no more players are being detected
				if (_chaseDetector.DetectedActors.Count <= 0)
				{
					State = "idle";
					break;
				}

				// select first detected player
				Node3D nodeToChase = _chaseDetector.DetectedActors[0];

				var chasePosition = new Vector2(
					nodeToChase.Position.Z, nodeToChase.Position.X);
				
				var myPosition = new Vector2(
					Position.Z, Position.X);
				
				// get angle to player and turn towards them
				TurnTowards(chasePosition.AngleToPoint(myPosition), 
					delta);
				
				// accelerate regardless of whether or not facing player
				Accelerate(delta);

				break;
		}

		// if no floor is detected, fall
		if (!IsOnFloor())
		{
			Fall(delta);
		}

		_debugLabel.Text = $"State: {State}";
	}


	// Signals //

	private void OnDetectorDetected()
	{
		if (State != "idle") return;
		
		State = "shock";
	}

	private void OnDetectorNotDetecting()
	{
		if (State == "attack") return;

		State = "idle";
	}

	private void OnAttackDetected()
	{
		State = "attack";
	}
}
