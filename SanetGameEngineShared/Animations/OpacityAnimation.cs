using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Sanet.XNAEngine.Animations
{
    public class OpacityAnimation : AnimationBase
    {
        #region Fields

        float _inFrom;
        float _inTo;

        float _opacityFrom;
        float _opacityTo;
        float _currentOpacity = 1;
        int _animationTime;
        int _elapsedTime;
        int _flashTimes;

        public event Action Completed;

        #endregion

        #region Constructor
        public OpacityAnimation(float from, float to, int time)
        {
            _inFrom=_opacityFrom = from;
            _inTo=_opacityTo = to;
            _animationTime = time;

            IsLooping = false;
            PlayOnClick = false;
        }

        public OpacityAnimation(XElement xmldata) :
            this(float.Parse(xmldata.Attribute("From").Value, CultureInfo.InvariantCulture),
                 float.Parse(xmldata.Attribute("To").Value, CultureInfo.InvariantCulture),
                 int.Parse(xmldata.Attribute("Time").Value)) { }

        #endregion

        #region Properties


        public float Opacity
        {
            get
            {
                return _currentOpacity;
            }
        }




        #endregion

        #region methods
        public void PlayAnimation(int flashTimes)
        {
            PlayAnimation();
            _flashTimes = flashTimes;
        }

        public override void PlayAnimation()
        {
            base.PlayAnimation();
            _opacityFrom = _inFrom;
            _opacityTo = _inTo;
            _elapsedTime = 0;
            _flashTimes = 0;
        }

        public override void StopAnimation()
        {
            base.StopAnimation();
            _currentOpacity = _opacityFrom;
            _flashTimes = 0;
        }

        public override void Update(RenderContext renderContext)
        {

            if (IsPlaying && !IsPaused)
            {
                _elapsedTime += renderContext.GameTime.ElapsedGameTime.Milliseconds;

                if (_elapsedTime > _animationTime)
                {
                    if (_flashTimes == 0)
                    {
                        StopAnimation();
                        _currentOpacity = _opacityFrom;
                        if (Completed != null)
                            Completed();
                        return;
                    }
                    else
                    {
                        var to = _opacityFrom;
                        var from = _opacityTo;
                        //if flash more than 10,000 times - make it invinite;
                        if (_flashTimes<10000)
                            _flashTimes--;
                        _elapsedTime = 0;
                        _opacityFrom = from;
                        _opacityTo = to;
                        //Debug.WriteLine("flashTimes: " + _flashTimes.ToString());
                    }
                }
                else
                    _currentOpacity = _opacityFrom + (_opacityTo - _opacityFrom) * _elapsedTime / _animationTime;

            }
        }


        #endregion
    }
}
