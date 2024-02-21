using Godot;
using System;

public partial class Hint : Node3D
{
	public override void _Ready()
	{
		Visible = Engine.IsEditorHint();
	}
}
