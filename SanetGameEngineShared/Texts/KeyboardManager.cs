using System;
using System.Collections.Generic;

namespace Sanet.XNAEngine
{
    /// <summary>
    /// Idea on how this should work:
    /// there are 3 layers, 2 are common and one is separate for each platform:
    /// 1) We have Monogamecontrol "TextField" based on GameSpriteTouchable (GameButton??) and TextPrinter as it child
    /// 2) This class provides TextField with informtion about key strokes user press
    /// 3) The information about key strkes is received from Native UI controls (should be implemented separately for each paltform as part of GamePage, Activity etc)
    /// </summary>
    public static class KeyboardManager
    {
        #region Events
        public static event Action<Guid, InputFormat> OnTextFieldActivationRequest;
        public static event Action<Guid> OnTextFieldDeActivationRequest;

        public static event Action<Guid> OnTextFieldActivated;
        public static event Action<Guid> OnTextFieldDeActivated;

        public static event Action<Guid,Char> OnKeyInput;
        public static event Action<Guid> OnKeyDelete;
        #endregion

        #region Fields
        static List<Guid> _registeredTextFields = new List<Guid>();
        static Guid _currentActiveTextField;
        #endregion

        #region Methods
        public static void RequestTextFieldActivation(Guid textFieldID, InputFormat type)
        {
            if (_currentActiveTextField != Guid.Empty)
            {
                //TODO
                //we already have active textbox - deactivate it??
				if (OnTextFieldDeActivated!=null)
                OnTextFieldDeActivated(_currentActiveTextField);
                _currentActiveTextField = Guid.Empty;
                
                
            }
            if (!_registeredTextFields.Contains(textFieldID))
                _registeredTextFields.Add(textFieldID);
            if (OnTextFieldActivationRequest != null) 
                OnTextFieldActivationRequest(textFieldID, type);

            _currentActiveTextField = textFieldID;
        }

        public static void RequestTextFieldDeActivation(Guid textFieldID)
        {
            if (_currentActiveTextField != textFieldID)
            {
                //TODO
                //something wrong - other textfield is active??
                return;
            }
            
            if (OnTextFieldDeActivationRequest != null) 
                OnTextFieldDeActivationRequest(textFieldID);
			_currentActiveTextField = Guid.Empty;
        }

        public static void ActivateTextField(Guid textFieldID)
        {
            _currentActiveTextField = textFieldID;
            if (OnTextFieldDeActivated != null)
                OnTextFieldDeActivated(textFieldID);
        }

        public static void InputKey(Guid textFieldID, char key)
        {
            if (OnKeyInput != null)
                OnKeyInput(textFieldID, key);
        }

        public static void DeleteKey(Guid textFieldID)
        {
            if (OnKeyDelete != null)
                OnKeyDelete(textFieldID);
        }

        #endregion
    }
}