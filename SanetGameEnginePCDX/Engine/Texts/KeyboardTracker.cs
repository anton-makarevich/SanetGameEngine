using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;

namespace Sanet.XNAEngine
{
    public class KeyboardTracker : IKeyboardTracker
    {
        private KeyboardState _previousKeyboardState;
        private Guid _textFieldId;

        public KeyboardTracker()
        {
            KeyboardManager.OnTextFieldActivationRequest += KeyboardManager_OnTextFieldActivationRequest;
            KeyboardManager.OnTextFieldDeActivationRequest += KeyboardManager_OnTextFieldDeActivationRequest;
            PrevText = "";
        }

        public string Text { get; set; }
        public string PrevText { get; set; }
        public Guid TextFieldId => _textFieldId;

        void KeyboardManager_OnTextFieldActivationRequest(Guid id, InputFormat type)
        {
            _textFieldId = id;
        }

        void KeyboardManager_OnTextFieldDeActivationRequest(Guid id)
        {
            if (_textFieldId == id)
                _textFieldId = Guid.Empty;
        }

        public void Update()
        {
            if (_textFieldId == Guid.Empty)
                return;

            var keyboardState = Keyboard.GetState();
            var pressedKeys = keyboardState.GetPressedKeys();
            var previousKeys = _previousKeyboardState.GetPressedKeys();

            foreach (var key in pressedKeys)
            {
                if (!previousKeys.Contains(key))
                {
                    var ch = KeyToChar(key);
                    if (ch != '\0')
                        KeyboardManager.InputKey(_textFieldId, ch);
                }
            }

            if (keyboardState.IsKeyDown(Keys.Back) && !_previousKeyboardState.IsKeyDown(Keys.Back))
                KeyboardManager.DeleteKey(_textFieldId);

            if (keyboardState.IsKeyDown(Keys.Enter) && !_previousKeyboardState.IsKeyDown(Keys.Enter))
                KeyboardManager.InputKey(_textFieldId, '\n');

            _previousKeyboardState = keyboardState;
        }

        private static char KeyToChar(Keys key)
        {
            if (key >= Keys.A && key <= Keys.Z)
                return (char)('a' + (key - Keys.A));

            if (key >= Keys.D0 && key <= Keys.D9)
                return (char)('0' + (key - Keys.D0));

            if (key >= Keys.NumPad0 && key <= Keys.NumPad9)
                return (char)('0' + (key - Keys.NumPad0));

            switch (key)
            {
                case Keys.Space: return ' ';
                case Keys.OemPeriod: return '.';
                case Keys.OemComma: return ',';
                case Keys.OemMinus: return '-';
                case Keys.OemPlus: return '+';
                case Keys.OemQuestion: return '/';
                case Keys.OemSemicolon: return ';';
                case Keys.OemQuotes: return '\'';
                case Keys.OemOpenBrackets: return '(';
                case Keys.OemCloseBrackets: return ')';
                case Keys.OemPipe: return '|';
                case Keys.OemTilde: return '~';
                case Keys.Tab: return '\t';
            }
            return '\0';
        }
    }
}
