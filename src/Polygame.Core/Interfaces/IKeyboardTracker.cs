using System;

namespace Sanet.Polygame.Interfaces;

public interface IKeyboardTracker
{
    string Text { get; set; }
    string PrevText { get; set; }
    Guid TextFieldId { get; }
}