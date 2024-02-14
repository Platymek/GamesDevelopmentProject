using Godot;
using System;

public partial class Emitter : Node3D
{
    [Export] PackedScene _sceneToEmit;

    // emit the packed scene
    public void Emit()
    {
        Node3D nodeToEmit = _sceneToEmit.Instantiate<Node3D>();

        // add to the same node as the owner of the emitter
        GetOwner<Node3D>().AddSibling(nodeToEmit);

        // set the position and rotation to that of the emitter itself
        nodeToEmit.GlobalPosition = GlobalPosition;
        nodeToEmit.GlobalRotation = GlobalRotation;
    }
}
