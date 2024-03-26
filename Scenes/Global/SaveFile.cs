using Godot;
using Godot.Collections;
using System;

public partial class SaveFile : Resource
{
	[Export] public int SceneProgress;
	[Export] public Array<int> TimeTrialTimes;
	[Export] public Array<int> MaxJars;

    public SaveFile() : this(0, new Array<int>(), new Array<int>()) {}

	public SaveFile(int sceneProgress, Array<int> timeTrialTimes, Array<int> maxJars)
    {
        SceneProgress = sceneProgress;
        TimeTrialTimes = timeTrialTimes;
        MaxJars = maxJars;
    }

}
