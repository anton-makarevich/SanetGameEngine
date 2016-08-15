using System;
namespace Sanet.XNAEngine
{
    public interface IKeyboardTracker
    {
        string Text { get; set; }
        string PrevText { get; set; }
        Guid TextFieldId { get; }
    }
}