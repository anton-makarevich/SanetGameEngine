using System;
using Sanet.Polygame.BaseObjects;

namespace Sanet.Polygame.Utils;

//simple timer to use in game for delays and similar things
public class GameTimer : GameObject2D
{
    public event Action Completed;

    #region Fields
       
    private int _runningTime = 0;
        
    #endregion

    #region Properties
    public int Interval{get; private set;}

    public bool IsActive {get;private set;}

    public bool IsLooping {get;set;}
    #endregion
                        
    #region Methods
    public override void Update(RenderContext renderContext)
    {
        //Updating animations
        if (IsActive)
        {
            _runningTime+=renderContext.GameTime.ElapsedGameTime.Milliseconds;
            if (_runningTime>=Interval)
            {
                _runningTime=0;
                IsActive=IsLooping;
                if (Completed!=null)
                    Completed();
                    
            }
        }
        base.Update(renderContext);
    }

    public void Start(int interval, bool restart = false)
    {
        if (!restart && IsActive)
            return;
        Interval=interval;
        _runningTime=0;
        IsActive=true;
    }

    public void Stop()
    {
        IsActive = false;
    }
    #endregion
        
}