using Godot;
using System;
using System.Diagnostics;

public partial class TechnoCultist : Actor
{
	// Properties //

	protected AnimationPlayer AnimationPlayer;
	private Detector _chaseDetector;
	private Detector _attackDetector;

	private Label3D _debugLabel;

	private string _state = "idle";

	[Export] protected virtual string State
	{
		get => _state;

		set
		{
			Halt();
			
			_state = value;

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


	// Node Functions //

	public override void _Ready()
	{
		base._Ready();

		AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		_chaseDetector = GetNode<Detector>("Chase");
		_attackDetector = GetNode<Detector>("Attack");

		_debugLabel = GetNode<Label3D>("DebugLabel");

		State = "idle";
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		switch (State)
		{
			case "run":

				Node3D nodeToChase = _chaseDetector.DetectedActors[0];

				var chasePosition = new Vector2(
					nodeToChase.Position.Z, nodeToChase.Position.X);
				
				var myPosition = new Vector2(
					Position.Z, Position.X);
				
				TurnTowards(chasePosition.AngleToPoint(myPosition), 
					delta);
				
				Accelerate(delta);

				break;
		}

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
