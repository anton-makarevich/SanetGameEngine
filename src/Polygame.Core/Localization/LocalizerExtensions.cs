using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sanet.Polygame.Localization;

public static class LocalizerExtensions
{
    private static ResourceModel _RModel;
        
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
        get => _RModel.CurrentLanguage;
        set => _RModel.CurrentLanguage = value;
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