using System;
using System.Linq;
using System.Windows.Input;
using System.Windows.Forms;


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

            _textBox.TextChanged+=_textBox_TextChanged;
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

                _textFieldId = id;
                _textBox.Focus();
                //_textBox.InputScope = ConvertType(type);

        }

        void KeyboardManager_OnTextFieldDeActivationRequest(Guid id)
        {

                if (_textFieldId == id)
                    _button.Focus();
        }
        void _textBox_TextChanged(object sender, EventArgs e)
        {
            Text = _textBox.Text;
            this.OnTextChange();
        }

       
        #endregion
    }
}