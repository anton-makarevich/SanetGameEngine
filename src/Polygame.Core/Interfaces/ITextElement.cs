using Microsoft.Xna.Framework;
using Sanet.Polygame.Texts;

namespace Sanet.Polygame.Interfaces;

public interface ITextElement
{
    Color TextColor { get; set; }
    Rectangle Rect { get; set; }
    int LineSpacing { get; set; }
    TextAlignment Alignment { get; set; }
    string Tag { get; set; }
    string Text { get; set; }
    bool TextWrap { get; set; }
}