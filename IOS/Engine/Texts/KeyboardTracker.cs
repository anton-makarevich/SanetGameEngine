using UIKit;
using System;
using System.Linq;
using Foundation;

namespace Sanet.XNAEngine
{
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
        Guid _textFieldId;

        UIViewController _vc;
        UIViewController _controller;
        UITextField _textBox;

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
            _textBox.AutocapitalizationType = UITextAutocapitalizationType.None;
            _textBox.AutocorrectionType = UITextAutocorrectionType.No;
            _textBox.KeyboardType = ConvertType(type);
            _textBox.BecomeFirstResponder();
        }

        void KeyboardManager_OnTextFieldDeActivationRequest(Guid id)
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

        UIKeyboardType ConvertType(InputFormat type)
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
}