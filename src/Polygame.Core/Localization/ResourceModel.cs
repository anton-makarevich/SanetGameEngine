using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using Sanet.Polygame.Utils;

namespace Sanet.Polygame.Localization;

public class ResourceModel
{
    Dictionary<string,XDocument> _strings;
    string[] _supportedLocales;
    int _loadedLocales = 0;

    public ResourceModel(string[] supportedLocales)
    {
        _strings = new Dictionary<string, XDocument>();
        _supportedLocales = supportedLocales;
    }

    public bool IsLoaded { get; private set; }

        
    public void LoadStep()
    {
        if (_supportedLocales.Length > _loadedLocales)
        {
            var locale = _supportedLocales[_loadedLocales];
            _strings.Add(locale, XMLLoader.LoadDocument(string.Format("Data/Strings/{0}/Resources.resw", locale)));
            _loadedLocales++;
        }
        else
            IsLoaded = true;
    }

    public string GetString(string resource)
    {
        if (_strings.Count == 0)
            return resource;
        try
        {
            var resDoc = _strings[CurrentLanguage];
            var node = resDoc.Descendants("data").FirstOrDefault(f=>f.Attribute("name").Value==resource);
            return node.Element("value").Value;
        }
        catch (Exception)
        {
            return resource;
        }

    }

    public string CurrentLanguage
    {
        get
        {
            var lang = ApplicationResources.UiCulture.TwoLetterISOLanguageName;
            if (!_supportedLocales.Contains (lang)) 
            {
                lang = _supportedLocales [0];
                ApplicationResources.UiCulture =  new CultureInfo(lang);
            }
            return lang;
        }
        set => ApplicationResources.UiCulture =  new CultureInfo(value);
    }
}