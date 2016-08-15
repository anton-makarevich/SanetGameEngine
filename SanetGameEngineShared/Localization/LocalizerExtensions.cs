using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sanet.XNAEngine.Localization
{
    public static class LocalizerExtensions
    {
        static ResourceModel _RModel;
        
        public static void Initialize(string[] locales)
        {
            _RModel = new ResourceModel(locales);
        }

        public static bool IsLoaded
        {
            get
            {
                if (_RModel == null)
                    return false;
                return _RModel.IsLoaded;
            }
            
        }

        public static string CurrentLanguage
        {
            get
            {
                return _RModel.CurrentLanguage;
            }
            set
            {
                _RModel.CurrentLanguage = value;
            }
        }

        public static void LoadStep()
        {
            _RModel.LoadStep();
        }

        public static void Load()
        {
            while (!IsLoaded)
                _RModel.LoadStep();
        }


        public static string Localize(this string value)
        {
            if (!string.IsNullOrEmpty(value))
                return _RModel.GetString(value);
            return "";

        }
    }
}
