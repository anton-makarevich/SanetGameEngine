 
 
 
 
 
 
 
 
 
 
 
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sanet.XNAEngine
{
    public class TextPrinter : GameObject2D, ITextElement
    {
        #region Constructor
        public TextPrinter(string spriteFontName)
        {
            _spriteFontName = spriteFontName;
            CanDraw = true;
        }
        #endregion

        #region Fields
        CustomSpriteFont _spriteFont;
        string _spriteFontName;
        List<TextLine> _lines;
        int _aStyle = 0;
        Vector2 _modDevScale = SceneManager.RenderContext.DeviceScale;
        bool _constantRatio;

        Vector2 _dPos = Vector2.Zero;

        string _Text;

        private Vector2 _Position= new Vector2(5,5);
        private Rectangle _Rect = new Rectangle(0, 0, 1280, 768);
        private Color _TextColor=Color.White;
        private float _Rotation=0.0f;
        private float _Scale=1.0f;
        private bool _TextWrap = false;
        int _LineSpacing=0;
        private TextAlignment _Alignment = TextAlignment.TopLeft;
        
        #endregion

        #region Properties
        /// <summary>
        /// Text to print
        /// </summary>
        public string Text 
        {
            get
            {
                return _Text;
            }
            set
            {
                _Text = value;
                ProcessText();
            }
        }

        public float FontHeight
        {
            get
            {
                return _spriteFont.FontHeight;
            }
        }
        
        public Rectangle Rect
        {
            get { return _Rect; }
            set
            {
                if (_Rect != value)
                {
                    _Rect = value;
                    _Position = new Vector2(_Rect.X, _Rect.Y);
                    ProcessText();
                }
            }
        }
        /// <summary>
        /// Position where to print
        /// </summary>  
        public Vector2 Position
        {
            get { return _Position; }
            set
            {
                if (_Position != value)
                {
                    Rect = new Rectangle((int)value.X, (int)value.Y, _Rect.Width, _Rect.Height);
                }
            }
        }

        
        public bool IsConstantRatio 
        {
            get
            {
                return _constantRatio;
            }
            set
            {
                _constantRatio=value;
                _modDevScale = (!value) 
                    ? SceneManager.RenderContext.DeviceScale
                    : new Vector2(SceneManager.RenderContext.DeviceScale.X, SceneManager.RenderContext.DeviceScale.X);
            }
        }

        /// <summary>
        /// last line Size
        /// </summary>
        public Vector2 LineSize
        {
            get;
            private set;
        }

        public Vector2 LinePosition
        {
            get;
            private set;
        }

        /// <summary>
        /// Text color
        /// </summary>
        public Color TextColor
        {
            get { return _TextColor; }
            set
            {
                if (_TextColor != value)
                {
                    _TextColor = value;
                    
                }
            }
        }   

        /// <summary>
        /// TextRotation
        /// </summary>
        public float Rotation
        {
            get { return _Rotation; }
            set
            {
                if (_Rotation != value)
                {
                    _Rotation = value;
                    
                }
            }
        }   
          
        /// <summary>
        /// Text scale
        /// </summary>
        public float FontScale
        {
            get { return _Scale; }
            set
            {
                if (_Scale != value)
                {
                    _Scale = value;
                }
            }
        }

        /// <summary>
        /// Text Wrapping
        /// </summary>
        public bool TextWrap
        {
            get { return _TextWrap; }
            set
            {
                if (_TextWrap != value)
                {
                    _TextWrap = value;
                    ProcessText();
                }
            }
        }

        public int LineSpacing
        {
            get
            {
                return _LineSpacing;
            }
            set
            {
                _LineSpacing = value;
            }
        }

        /// <summary>
        /// Text Alignment 
        /// </summary>
        public TextAlignment Alignment 
        {
            get { return _Alignment; }
            set
            {
                if (_Alignment != value)
                {
                    _Alignment = value;
                    ProcessText();
                }
            }
        }

        #endregion

        #region Methods
        public override void LoadContent(ContentManager contentManager, bool isLocal)
        {
            base.LoadContent(contentManager,isLocal);
            if (isLocal == IsLocalContent)
            {
                _spriteFont = FontsProvider.GetFont(_spriteFontName);
                if (_spriteFont == null)
                    throw new ArgumentNullException("Please check the font name " + _spriteFontName);
                ProcessText();
            }
        }

        public override void Draw(RenderContext renderContext)
        {
            if (!(string.IsNullOrEmpty(Text) /*|| !CanDraw*/))
            {
                // draw text
                foreach (var line in _lines)
                {
                    // draw the line of text
                    try
                    {
                        if (line.LocalPosition == Vector2.Zero && _dPos != Vector2.Zero)
                        {
                            continue;
                        }
                        _spriteFont.Draw(
                            renderContext,
                            line.Text,
                            (line.LocalPosition * renderContext.DeviceScale),
                            TextColor,
                            Rotation,
                            (FontScale * _modDevScale)
                            );
                    }
                    catch(Exception ex)
                    {
                        var t = ex.Message;
                    }
                }
            }

            base.Draw(renderContext);

        }

        public override void Update(RenderContext renderContext)
        {
            if (string.IsNullOrEmpty(Text))
            {
                LinePosition = Vector2.Zero;
            }
            else
            {
                Vector2 pos;
                if (Parent != null)
                    _dPos = (Parent.WorldPosition / renderContext.DeviceScale);
                //else
                //    _dPos = Vector2.Zero;
                
                pos  =  Position+_dPos;
                var rect = new Rectangle((int)pos.X, (int)pos.Y, _Rect.Width, _Rect.Height);
                int lineCount = 0;
                foreach (var line in _lines)
                {
                    string txt = line.Text;
                    Vector2 size = MeasureText(txt);
                    switch (_aStyle)
                    {
                        case 0:
                            pos.X = rect.X;
                            break;

                        case 1:
                            pos.X = rect.X + ((Rect.Width / 2) - (size.X / 2));
                            break;

                        case 2:
                            pos.X = rect.Right - size.X;
                            break;

                    }
                    lineCount++;
                    line.Translate(pos);
                    pos.Y += LineSpacing;
                    if (lineCount == _lines.Count)
                        LinePosition = pos;
                }
            }

            base.Update(renderContext);
        }
        
        /// <summary>
        /// recalculating text size and devide into lines if needed
        /// </summary>
        void ProcessText()
        {
            if (string.IsNullOrEmpty(Text)) return;

            if (_spriteFont == null)
                return;

            Rectangle textBounds;
            // check if there is text to draw
            textBounds = Rect;
            

            _lines = new List<TextLine>();
            var textsForLines = Text.Split(new string[] { "\\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var text in textsForLines)
                _lines.Add(new TextLine(text));
            
            // calc the size of the rect for all the text
            Rectangle tmprect = ProcessLines();


            _aStyle = 0;

            switch (Alignment)
            {
                case TextAlignment.Bottom:
                    _Position.Y = Rect.Bottom - tmprect.Height;
                    _aStyle = 1;
                    break;

                case TextAlignment.BottomLeft:
                    _Position.Y = Rect.Bottom - tmprect.Height;
                    _aStyle = 0;
                    break;

                case TextAlignment.BottomRight:
                    _Position.Y = Rect.Bottom - tmprect.Height;
                    _aStyle = 2;
                    break;

                case TextAlignment.Left:
                    _Position.Y = Rect.Y + ((Rect.Height / 2) - (tmprect.Height / 2));
                    _aStyle = 0;
                    break;

                case TextAlignment.Middle:
                    _Position.Y = Rect.Y + ((Rect.Height / 2) - (tmprect.Height / 2));
                    _aStyle = 1;
                    break;

                case TextAlignment.Right:
                    _Position.Y = Rect.Y + ((Rect.Height / 2) - (tmprect.Height / 2));
                    _aStyle = 2;
                    break;

                case TextAlignment.Top:
                    _aStyle = 1;
                    break;

                case TextAlignment.TopLeft:
                    _aStyle = 0;
                    break;

                case TextAlignment.TopRight:
                    _aStyle = 2;
                    break;

            }

            

        }

        Rectangle ProcessLines()
        {
            // loop through each line in the collection
            Rectangle bounds = Rect;
            bounds.Width = 0;
            bounds.Height = 0;

            int index = 0;
            float Width = 0;
            bool lineInserted = false;

            while (index < _lines.Count)
            {
                // get a line of text
                string linetext = _lines[index].Text;

                //measure the line of text

                Vector2 size = MeasureText(linetext);

                // check if the line of text is geater then then the rect we want to draw it inside of
                if (TextWrap && size.X > Rect.Width)
                {
                    // find last space character in line
                    string endspace = string.Empty;
                    // deal with trailing spaces
                    if (linetext.EndsWith(" "))
                    {
                        endspace = " ";
                        linetext = linetext.TrimEnd();
                    }

                    // get the index of the last space character
                    int i = linetext.LastIndexOf(" ");
                    if (i != -1)
                    {
                        // if there was a space grab the last word in the line
                        string lastword = linetext.Substring(i + 1);
                        // move word to next line 
                        if (index == _lines.Count - 1)
                        {
                            _lines.Add(new TextLine(lastword));
                            lineInserted = true;
                        }
                        else
                        {
                            // prepend last word to begining of next line
                            if (lineInserted)
                            {
                                _lines[index + 1].Text = lastword + endspace + _lines[index + 1].Text;
                            }

                            else
                            {
                                _lines.Insert(index + 1, new TextLine(lastword));
                                lineInserted = true;
                            }
                        }

                        // crop last word from the line that is being processed
                        _lines[index].Text = linetext.Substring(0, i + 1);
                    }
                    else
                    {
                        // there appear to be no space characters on this line s move to the next line
                        lineInserted = false;
                        size = MeasureText(_lines[index].Text);
                        if (size.X > bounds.Width) Width = size.X;
                        bounds.Height += LineSpacing;// size.Y - 1;
                        index++;
                    }
                }
                else
                {
                    // this line will fit so we can skip to the next line
                    lineInserted = false;
                    size = MeasureText(_lines[index].Text);
                    if (size.X > bounds.Width) bounds.Width = (int)size.X;
                    bounds.Height += LineSpacing;//size.Y - 1;
                    index++;
                }

                if (index == _lines.Count)
                    LineSize = size;
            }

            // returns the size of the text
            return bounds;

        }

        Vector2 MeasureText(string text)
        {
            return _spriteFont.MeasureString(text) * FontScale;

        }
        #endregion
    }
}
