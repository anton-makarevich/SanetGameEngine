using System;
using Foundation;
using Sanet.Polygame.Enums;
using Sanet.Polygame.Interfaces;
using Sanet.Polygame.Texts;
using Sanet.Polygame.Utils;
using UIKit;

namespace Sanet.Polygame.iOS.Engine.Texts;

/// <summary>
/// This control implementation is specific to Android and depends on native UI 
/// trying to keep this specific part as small as possible, applying shared code through extension
/// </summary>
public class KeyboardTracker : IKeyboardTracker
{
    #region Constructor
    public KeyboardTracker()
    {
        KeyboardManager.OnTextFieldActivationRequest += KeyboardManager_OnTextFieldActivationRequest;
        KeyboardManager.OnTextFieldDeActivationRequest += KeyboardManager_OnTextFieldDeActivationRequest;
        _textBox = new UITextField();

        _vc = new UIViewController();
        _vc.View.AddSubview(_textBox);
        _controller = UIApplication.SharedApplication.Windows[0].RootViewController;
        _controller.View.AddSubview(_vc.View);
        PrevText = "";
        NSNotificationCenter.DefaultCenter.AddObserver
            (UITextField.TextFieldTextDidChangeNotification, TextChangedEvent);
    }


    #endregion

    #region Fields

    private Guid _textFieldId;

    private readonly UIViewController _vc;
    private readonly UIViewController _controller;
    private readonly UITextField _textBox;

    #endregion

    #region Properties
    public string Text { get; set; }
    public string PrevText { get; set; }
    public Guid TextFieldId => _textFieldId;

    #endregion

    #region Methods

    private void KeyboardManager_OnTextFieldActivationRequest(Guid id, InputFormat type)
    {
        _textFieldId = id;
        _textBox.AutocapitalizationType = UITextAutocapitalizationType.None;
        _textBox.AutocorrectionType = UITextAutocorrectionType.No;
        _textBox.KeyboardType = ConvertType(type);
        _textBox.BecomeFirstResponder();
    }

    private void KeyboardManager_OnTextFieldDeActivationRequest(Guid id)
    {
        if (_textFieldId == id)
        {
            _textBox.ResignFirstResponder();

        }
    }

    private void TextChangedEvent(NSNotification notification)
    {

        if (notification.Object == _textBox)
        {
            Text = _textBox.Text;
            this.OnTextChange();
        }
    }

    private UIKeyboardType ConvertType(InputFormat type)
    {
        switch (type)
        {
            case InputFormat.Numeric:
                return UIKeyboardType.NumberPad;
            default:
                return UIKeyboardType.Default;
        }
    }

    #endregion
}