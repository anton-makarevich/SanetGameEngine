 
 
 
 
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;


namespace Sanet.XNAEngine.Animations
{
    public class FrameAnimation : AnimationBase
    {
        public event Action Completed;

        #region Fields
        private readonly int _rowCount;
        private readonly int _columnCount;
        private int _totalFrameTime;
        private Rectangle _frameRect;
        #endregion

        #region Properties

        int _numFrames;
        public int NumFrames
        {
            get
            {
                return _numFrames;
            }
            private set
            {
                _framesOrder = new List<int>();
                for (var i = 0; i < value; i++)
                    _framesOrder.Add(i);
                _numFrames = value;
            }
        }

        public int CurrentFrameNumber
        {
            get
            { return _framesOrder[CurrentFrame]; }
        }

        public Vector2 FrameSize { get; protected set; }
        public int CurrentFrame { get; protected set; }

        public List<int> _framesOrder;

        public int FrameInterval { get; set; }


        public Rectangle FrameRect
        {
            get
            { return _frameRect; }
        }
        #endregion

        #region Constructor
        public FrameAnimation(XElement xmldata)
            : this(
                int.Parse(xmldata.Attribute("Frames").Value),
                int.Parse(xmldata.Attribute("FrameInterval").Value),
                int.Parse(xmldata.Attribute("FramesInRow").Value))
        {
            IsLooping = xmldata.Attribute("IsLooping").Value.ToLower() == "true";
            PlayOnClick = xmldata.Attribute("PlayOnClick").Value.ToLower() == "true";
            var order = xmldata.Attribute("FramesOrder");
            if (order != null)
            {
                _framesOrder = order.Value.Split(',').Select(s => int.Parse(s)).ToList();
            }

        }

        public FrameAnimation(int numFrames, int frameInterval, Vector2 frameSize) :
            this(numFrames, frameInterval, numFrames) { }

        public FrameAnimation(int numFrames, int frameInterval, int framesPerRow)
        {
            NumFrames = numFrames;
            FrameInterval = frameInterval;

            _rowCount = 1;
            _columnCount = numFrames;

            if (framesPerRow < numFrames)
            {
                _rowCount = numFrames / framesPerRow;
                _columnCount = framesPerRow;
            }
            IsLooping = true;
        }
        #endregion

        #region Methods

        public void SetSpriteSize(Vector2 spriteSize)
        {
            FrameSize = new Vector2(spriteSize.X / _columnCount, spriteSize.Y / _rowCount);
            _frameRect = new Rectangle(0, 0, (int)FrameSize.X, (int)FrameSize.Y);
        }

        public override void StopAnimation()
        {
            var oldIsPlaying = IsPlaying;
            base.StopAnimation();
            CurrentFrame = 0;
            _totalFrameTime = 0;
            if (oldIsPlaying && Completed != null)
                Completed();
        }

        public override void Update(RenderContext renderContext)
        {
            if (IsPlaying && !IsPaused)
            {
                _totalFrameTime += renderContext.GameTime.ElapsedGameTime.Milliseconds;

                if (_totalFrameTime >= FrameInterval)
                {
                    _totalFrameTime = 0;

                    NextFrame();

                }
            }

        }

        protected virtual void NextFrame()
        {
            ++CurrentFrame;

            CheckFrame();

            UpdateFrame();

            CheckFrame();
        }

        private void CheckFrame()
        {
            if (CurrentFrame >= (_framesOrder.Count))
            {
                CurrentFrame = 0;
                IsPlaying = IsLooping;
                if (Completed != null)
                    Completed();

            }
        }
        private void UpdateFrame() 
        {
            var frameNo = CurrentFrameNumber;
            if (_rowCount > 1)
            {
                _frameRect.Location = new Point(
                    (int)FrameSize.X *
                            (frameNo % _columnCount),

                          (int)FrameSize.Y * (int)Math.Floor((float)
                        (frameNo / _columnCount)
                        )
                    );
            }
            else _frameRect.Location = new Point(
                (int)FrameSize.X * frameNo, 0
                );

        }

        public void SetStaticFrame(int frame)
        {
            StopAnimation();
            if (frame >= _framesOrder.Count)
                return;
            CurrentFrame = frame;
            UpdateFrame();
        }
        #endregion
    }
}
