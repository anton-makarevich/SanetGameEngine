using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Sanet.XNAEngine
{
    /// <summary>
    /// New concept of TextPrinter that should have better performance and be compatible with whole engine object system
    /// The idea is to render each line into texture on loading and using it as GameSprite
    /// </summary>
    public class TextBlockStandard : GameObject2D, ITextElement
    {
        #region Constructor
        public TextBlockStandard(string spriteFontName)
        {
            _spriteFontName = spriteFontName;
        }
        #endregion

        #region Fields
        SpriteFont _spriteFont;
        string _spriteFontName;
        
        string _Text;

        Vector2 _fontScale = Vector2.One;

        List<TextLineStandard> _lines;
                        
        private Rectangle _rect = new Rectangle(0, 0, 1280, 768);
        private Color _TextColor = Color.White;

        int _aStyle = 0;

        int _LineSpacing = 0;
        private TextAlignment _Alignment = TextAlignment.TopLeft;

        bool _textWrap;

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
                if (_Text != value)
                {
                    _Text = value;
                    ProcessText();
                }
            }
        }

        public float FontHeight
        {
            get
            {
                return _spriteFont.LineSpacing;
            }
        }

        public Rectangle Rect
        {
            get { return _rect; }
            set
            {
                if (_rect != value)
                {
                    _rect = value;
                    Translate(_rect.X, _rect.Y);
                    ProcessText();
                }
            }
        }

        public Vector2 FontScale
        {
            get
            {
                return _fontScale;
            }
            set
            {
                if (_fontScale != value)
                {
                    _fontScale = value;
                    ProcessText();
                }
            }
        }

        /// <summary>
        /// last line Size
        /// </summary>
        public Vector2 LineSize
        {
            get
            {
                if (_lines == null || _lines.Count == 0)
                    return Vector2.Zero;
                return _lines[0].Size;
            }
            
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
                    if (_lines!=null)
                    {
                        foreach (var line in _lines)
                            line.TextColor = value;
                    }
                }
            }
        }

        /// <summary>
        /// Text Wrapping
        /// </summary>
        public bool TextWrap
        {
            get { return _textWrap; }
            set
            {
                if (_textWrap != value)
                {
                    _textWrap = value;
                    ProcessText();
                }
            }
        }

        public bool TextTrim { get; set; }

        public bool TextFit { get; set; }

        public bool ForceYAlign { get; set; }

        public int LineSpacing
        {
            get
            {
                if (_LineSpacing == 0)
                    return (int)(LineSize.Y*FontScale.Y);
                return _LineSpacing;
            }
            set
            {
                if (_LineSpacing != value)
                {
                    _LineSpacing = value;
                    ProcessText();
                }
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

        public float ActualHeight { get; private set; }
        #endregion

        #region Methods
        public override void LoadContent(Microsoft.Xna.Framework.Content.ContentManager contentManager, bool isLocal)
        {
            if (isLocal)
                return;
            if (_spriteFont==null)
            {
                _spriteFont = FontProviderStandard.GetFont(_spriteFontName);
                ProcessText();
            }
            base.LoadContent(contentManager, isLocal);
            
            
        }

        /// <summary>
        /// recalculating text size and devide into lines if needed
        /// </summary>
        public void ProcessText()
        {
            if (Text==null)
                return;

            if (_spriteFont == null)
                return;

            Rectangle textBounds;
            // check if there is text to draw
            textBounds = Rect;

            if (_lines == null)
            {
                _lines = new List<TextLineStandard>();
            }
            else
            {
                foreach (var l in _lines)
                    RemoveChild(l);
                 _lines.Clear();
            }

            if (string.IsNullOrEmpty(Text))
            {
                _lines.Add(new TextLineStandard(_spriteFont, ""));
            }
            else
            {
                var textsForLines = Text.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var text in textsForLines)
                {
                    _lines.Add(new TextLineStandard(_spriteFont, text));
                }
            }
            // calc the size of the rect for all the text
            
                Rectangle tmprect = ProcessLines();
                if (!string.IsNullOrEmpty(Text))
                {
                _aStyle = 0;

                switch (Alignment)
                {
                    case TextAlignment.Bottom:
                        Translate(LocalPosition.X, Rect.Bottom - tmprect.Height);
                        _aStyle = 1;
                        break;

                    case TextAlignment.BottomLeft:
                        Translate(LocalPosition.X, Rect.Bottom - tmprect.Height);
                        _aStyle = 0;
                        break;

                    case TextAlignment.BottomRight:
                        Translate(LocalPosition.X, Rect.Bottom - tmprect.Height);
                        _aStyle = 2;
                        break;

                    case TextAlignment.Left:
                        Translate(LocalPosition.X, Rect.Y + ((Rect.Height / 2) - (tmprect.Height / 2)));
                        _aStyle = 0;
                        break;

                    case TextAlignment.Middle:
                        Translate(LocalPosition.X, Rect.Y + ((Rect.Height / 2) - (tmprect.Height / 2)));
                        _aStyle = 1;
                        break;

                    case TextAlignment.Right:
                        Translate(LocalPosition.X, Rect.Y + ((Rect.Height / 2) - (tmprect.Height / 2)));
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
            var pos = Vector2.Zero;
            if (ForceYAlign)
                pos.Y = (_rect.Height - tmprect.Height) / 2;

            var rect = new Rectangle(0, 0, _rect.Width , _rect.Height);
            int lineCount = 0;
            Children.Clear();
            foreach (var line in _lines)
            {
                line.TextColor = TextColor;

                var scaledSize = line.Size * FontScale;
                switch (_aStyle)
                {
                    case 1:
                        pos.X = rect.X + ((rect.Width / 2) - (scaledSize.X / 2));
                        break;

                    case 2:
                        pos.X = rect.Right - scaledSize.X;
                        break;

                }
                lineCount++;
                line.Translate(pos);
                line.Scale(FontScale);
                if (lineCount == _lines.Count)
                    LinePosition = pos;

                AddChild(line);

                pos.Y += LineSpacing;
            }
            ActualHeight = pos.Y;
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
                var line=_lines[index];
                string lineText = line.Text;

                if (TextFit && _lines.Count==1)
                {
                    if (line.Size.X*FontScale.X>Rect.Width)
                    {
                        var ts = Rect.Width / (line.Size.X * FontScale.X);
                        line.Scale(line.LocalScale * ts);
                    }
                }

                // check if the line of text is geater then then the rect we want to draw it inside of
                if (TextWrap && line.Size.X * FontScale.X > Rect.Width)
                {
                    // find last space character in line
                    string endspace = string.Empty;
                    // deal with trailing spaces
                    if (lineText.EndsWith(" "))
                    {
                        endspace = " ";
                        lineText = line.Text.TrimEnd();
                    }
                    else if (lineText.EndsWith("-"))
                    {
                        endspace = "-";
                        
                    }
                    if (endspace!=string.Empty)
                        lineText = line.Text.TrimEnd(new char[]{endspace[0]});

                    // get the index of the last space character
                    int i = Math.Max(lineText.LastIndexOf(" "),lineText.LastIndexOf("-"));

                    if (i != -1)
                    {
                        // if there was a space grab the last word in the line
                        string lastword = lineText.Substring(i + 1);
                        // move word to next line 
                        if (index == _lines.Count - 1)
                        {
                            _lines.Add(new TextLineStandard(_spriteFont,  lastword));
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
                                _lines.Insert(index + 1, new TextLineStandard(_spriteFont, lastword));
                                lineInserted = true;
                            }
                        }

                        // crop last word from the line that is being processed
                        _lines[index].Text = lineText.Substring(0, i + 1);
                    }
                    else
                    {
                        // there appear to be no space characters on this line so move to the next line
                        lineInserted = false;
                        if (line.Size.X * FontScale.X > bounds.Width) Width = line.Size.X * FontScale.X;
                        index++;
                    }
                }
                else if (!TextWrap && TextTrim && line.Size.X*FontScale.X > Rect.Width)
                {
                    //Need to trim text to fit into boundaries
                    //trimming works only for one line texts
                    do
                    {

                        line.Text = line.Text.Remove(line.Text.Length - 4, 4) + "...";
                    }
                    while (line.Size.X * FontScale.X > Rect.Width);

                    lineInserted = false;
                    if (line.Size.X* FontScale.X > bounds.Width) Width = line.Size.X * FontScale.X;
                }
                else
                {
                    // this line will fit so we can skip to the next line
                    lineInserted = false;
                    if (line.Size.X * FontScale.X > bounds.Width) Width = line.Size.X * FontScale.X;
                    index++;
                }

                

                
            }
            if (_lines.Count > 1)
                bounds.Height = (int)(LineSpacing*_lines.Count*FontScale.Y);//size.Y - 1;
            else
                bounds.Height = (int)(_lines[0].Size.Y * FontScale.Y);

            // returns the size of the text
            return bounds;
        }

        Color _c;
        public override void Update(RenderContext renderContext)
        {
            base.Update(renderContext);

        }
        #endregion
    }
}