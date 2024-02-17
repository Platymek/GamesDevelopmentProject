using Godot;
using System;

public partial class Emitter : Node3D
{
	[Export] private PackedScene _sceneToEmit;
	[Export] private bool _onDeath;

	// emit the packed scene
	public void Emit()
	{
		Node3D nodeToEmit = _sceneToEmit.Instantiate<Node3D>();

		// add to the same node as the owner of the emitter
		Owner.AddSibling(nodeToEmit);

		// set the position and rotation to that of the emitter itself
		nodeToEmit.GlobalPosition = GlobalPosition;
		nodeToEmit.GlobalRotation = GlobalRotation;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		if (!_onDeath) return;

		if (Owner is not Actor owner) return;
		
		if (owner.Dying)
		
		Emit();
	}
}
