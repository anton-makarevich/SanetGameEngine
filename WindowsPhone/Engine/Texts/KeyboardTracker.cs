using System;
using System.Linq;

#if NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
#else
using System.Windows.Controls;
using System.Windows.Input;
using Sanet.Common;
#endif


namespace Sanet.XNAEngine
{
    /// <summary>
    /// This control implementation is specific to Windows8 and depends on native UI 
    /// </summary>
    public class KeyboardTracker : IKeyboardTracker
    {
        #region Constructor
        public KeyboardTracker(TextBox textBox, Button button)
        {
            _textBox = textBox;
            _button = button;

            KeyboardManager.OnTextFieldActivationRequest += KeyboardManager_OnTextFieldActivationRequest;
            KeyboardManager.OnTextFieldDeActivationRequest += KeyboardManager_OnTextFieldDeActivationRequest;

            _textBox.TextChanged += _textBox_TextChanged;
            PrevText = "";
        }


        #endregion

        #region Fields
        TextBox _textBox;
        Button _button;

        Guid _textFieldId;

        #endregion

        #region Properties
        public string Text { get; set; }
        public string PrevText { get; set; }
        public Guid TextFieldId
        {
            get
            {
                return _textFieldId;
            }
        }
        #endregion

        #region Methods

        void KeyboardManager_OnTextFieldActivationRequest(Guid id, InputFormat type)
        {
#if WINDOWS_PHONE
            SmartDispatcher.BeginInvoke(() =>
            {

#endif
                _textFieldId = id;
                _textBox.Focus(
#if NETFX_CORE
                FocusState.Pointer
#endif
);
                _textBox.InputScope = ConvertType(type);
#if WINDOWS_PHONE
            });
#endif
        }

        void KeyboardManager_OnTextFieldDeActivationRequest(Guid id)
        {
#if WINDOWS_PHONE
            SmartDispatcher.BeginInvoke(() =>
            {
#endif
                if (_textFieldId == id)
                    _button.Focus(
#if NETFX_CORE
                FocusState.Programmatic
#endif
);
#if WINDOWS_PHONE
            });
#endif
        }

        void _textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Text = _textBox.Text;
            this.OnTextChange();
        }

        public InputScope ConvertType(InputFormat type)
        {
            InputScope scope = new InputScope();
            InputScopeName name = new InputScopeName();
            switch (type)
            {
                case InputFormat.Numeric:
                    name.NameValue = InputScopeNameValue.Number;
                    break;
                default:
                    name.NameValue = InputScopeNameValue.Default;
                    break;
            }
            scope.Names.Add(name);
            return scope;
        }
        #endregion
    }
}