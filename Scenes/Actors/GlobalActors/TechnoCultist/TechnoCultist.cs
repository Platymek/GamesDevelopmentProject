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
            _animationPlayer.Play(value);
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

                LookAt(_detector.DetectedActors[0].Position);

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
        State = "idle";
    }

}
