using Godot;
using System;

public partial class CameraHint : Detector
{
	[Export] Camera camera;

	[Export] private bool TrackX = false;
	[Export] private bool TrackY = false;
	[Export] private bool TrackZ = false;

	private void OnNotDetecting()
	{
		camera.TrackX = TrackX;
		camera.TrackY = TrackY;
		camera.TrackZ = TrackZ;

		if (TrackX)
		{
			camera.X = Position.X;
		}

		if (TrackY)
		{
			camera.Y = Position.Y;
		}

		if (TrackZ)
		{
			camera.Z = Position.Z;
		}
	}
}
