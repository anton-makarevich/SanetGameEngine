using System;

namespace Sanet.Polygame.SceneManager;

public class SceneChangedEventsArgs:EventArgs
{
    public string NewScene
    {
        get;
        private set;
    }
    public string OldScene
    {
        get;
        private set;
    }

    public SceneChangedEventsArgs(string newScene, string oldScene):base()
    {
        NewScene = newScene;
        OldScene = oldScene;
    }

}