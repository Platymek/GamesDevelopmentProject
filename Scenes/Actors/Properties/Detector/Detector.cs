using Godot;
using Godot.Collections;
using System;

public partial class Detector : Area3D
{
	// Properties //

	[Export] private Actor.Teams _teamToDetect;

	[Signal] public delegate void DetectedEventHandler();
	[Signal] public delegate void NotDetectingEventHandler();

	public Array<Node3D> DetectedActors = new();


	// Node Functions //

	public override void _Process(double delta)
	{
		base._Process(delta);


		foreach (var actor in DetectedActors)
		{
			if (actor.IsQueuedForDeletion())
			{
				DetectedActors.Remove(actor);

				// if first actor detected, send a signal
				if (DetectedActors.Count == 0)
				{
					EmitSignal(SignalName.NotDetecting);
				}
			}
		}
	}


	// Signals //

	private void OnAreaEntered(Area3D area)
	{
		if (area is not Detectable detectable) return;

		if (_teamToDetect != detectable.Team) return;


		// if first actor detected, send a signal
		if (DetectedActors.Count == 0)
		{
			EmitSignal(SignalName.Detected);
		}

		DetectedActors.Add(detectable.GetOwner<Node3D>());
	}

	private void OnAreaExited(Area3D area)
	{
		if (area is not Detectable detectable) return;

		if (_teamToDetect != detectable.Team) return;


		DetectedActors.Remove(detectable.GetOwner<Node3D>());

		// if first actor detected, send a signal
		if (DetectedActors.Count == 0)
		{
			EmitSignal(SignalName.NotDetecting);
		}
	}
}
