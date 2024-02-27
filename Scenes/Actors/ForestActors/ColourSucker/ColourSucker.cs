using Godot;
using System;

public partial class ColourSucker : TechnoCultist
{
	private string _subState;

	protected override string State
	{
		get => base.State;
		
		set
		{
			base.State = value;
			
			if (value == "attack")
			{
				_subState = "attack";
               
				AnimationPlayer.Play("ColourSucker/attack");
			}
		}
	}
}
