using System;
namespace Sanet.Polygame
{
    public interface IKeyboardTracker
    {
        string Text { get; set; }
        string PrevText { get; set; }
        Guid TextFieldId { get; }
    }
}