using Godot;
using System;

public partial class Camera : Node3D
{
    // Properties

	[Export] private CharacterBody3D _target;

	[Export] public bool TrackX;
    [Export] public bool TrackY;
    [Export] public bool TrackZ;

	private Vector3 _trackPosition;

    public float X
    {
        get => _trackPosition.X;
        set => _trackPosition.X = value;
    }

    public float Y
    {
        get => _trackPosition.Y;
        set => _trackPosition.Y = value;
    }

    public float Z
    {
        get => _trackPosition.Z;
        set => _trackPosition.Z = value;
    }


    // Node Functions //

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		base._Ready();

        _trackPosition = Position;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
    {
        Position = Position.MoveToward(_trackPosition, (float)delta * 3f * Position.DistanceTo(_trackPosition));

        if (TrackX)
        {
            X = _target.Position.X + _target.Velocity.X * 0.4f;
        }

        if (TrackY)
        {
            Y = _target.Position.Y + _target.Velocity.Y * 0.4f;
        }

        if (TrackZ)
        {
            Z = _target.Position.Z + _target.Velocity.Z * 0.4f;
        }
    }
}
