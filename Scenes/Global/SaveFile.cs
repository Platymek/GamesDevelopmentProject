using Godot;
using Godot.Collections;
using System;

public partial class SaveFile : Resource
{
	[Export] public int SceneProgress;
	[Export] public Dictionary<int, int> TimeTrialTimes;
	[Export] public Dictionary<int, int> MaxJars;

    public SaveFile() 
        : this(0, new Dictionary<int, int>(), new Dictionary<int, int>()) {}

	public SaveFile(int sceneProgress, Dictionary<int, int> 
        timeTrialTimes, Dictionary<int, int> maxJars)
    {
        SceneProgress = sceneProgress;
        TimeTrialTimes = timeTrialTimes;
        MaxJars = maxJars;
    }
}
