using Godot;
using System;

public partial class CameraHint : Detector
{
	[Export] Camera camera;

	[Export] public bool TrackX = false;
	[Export] public bool TrackY = false;
	[Export] public bool TrackZ = false;

    public void OnNotDetecting()
	{
		camera.TrackX = TrackX;
		camera.TrackY = TrackY;
		camera.TrackZ = TrackZ;

		if (!TrackX)
		{
			camera.X = Position.X;
		}

		if (!TrackY)
		{
			camera.Y = Position.Y;
		}

		if (!TrackZ)
		{
			camera.Z = Position.Z;
		}
	}
}
