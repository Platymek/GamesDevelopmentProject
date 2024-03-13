using Godot;
using System;

public partial class ColourJar : Node3D
{
	private void OnDetectorDetected()
	{
		if (!Visible) return;

		Visible = false;

		GetParent().GetParent<Level>().CollectJar(GetIndex());
	}
}
