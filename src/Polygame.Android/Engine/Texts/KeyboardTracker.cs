using System;
using Android.App;
using Android.Content;
using Android.Text;
using Android.Views.InputMethods;
using Android.Widget;
using Sanet.Polygame.Enums;
using Sanet.Polygame.Interfaces;
using Sanet.Polygame.Texts;
using Sanet.Polygame.Utils;

namespace Sanet.Polygame.Android.Engine.Texts;

/// <summary>
/// This control implementation is specific to Android and depends on native UI 
/// trying to keep this specific part as small as possible, applying shared code through extension
/// </summary>
public class KeyboardTracker : IKeyboardTracker
{
    #region Constructor
    public KeyboardTracker(EditText textBox, Activity activity)
    {
        _textBox = textBox;
        _activity = activity;

        KeyboardManager.OnTextFieldActivationRequest += KeyboardManager_OnTextFieldActivationRequest;
        KeyboardManager.OnTextFieldDeActivationRequest += KeyboardManager_OnTextFieldDeActivationRequest;

        _textBox.TextChanged += _textBox_TextChanged;
        PrevText = "";
    }



    #endregion

    #region Fields

    private Guid _textFieldId;
    private readonly EditText _textBox;
    private readonly Activity _activity;


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

        if (_textBox.HasFocus)
            _textBox.ClearFocus();

        _textBox.RequestFocus();
        _textBox.InputType = ConvertType(type);
        var manager = (InputMethodManager)_activity.GetSystemService(Context.InputMethodService);
        manager.ShowSoftInput(_textBox, 0);


    }

    private InputTypes ConvertType(InputFormat type)
    {
        switch (type)
        {
            case InputFormat.Numeric:
                return InputTypes.ClassNumber;
            default:
                return InputTypes.TextFlagNoSuggestions;
        }
    }

    private void KeyboardManager_OnTextFieldDeActivationRequest(Guid id)
    {
        if (_textFieldId == id)
        {
            var manager = (InputMethodManager)_activity.GetSystemService(Context.InputMethodService);
            manager.HideSoftInputFromWindow(_textBox.WindowToken, 0);
        }
    }

    private void _textBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        //actually in android it's more easy to use Properties of e arg
        //but to keep it easier to maintain through different platforms, code is the same everuwhere.
        Text = _textBox.Text;
        this.OnTextChange();

    }
    #endregion
}