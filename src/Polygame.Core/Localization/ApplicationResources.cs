using System.Globalization;
using System.Threading;

namespace Sanet.Polygame.Localization;

public class ApplicationResources
{
    private static CultureInfo uiCulture = Thread.CurrentThread.CurrentUICulture;
    public static CultureInfo UiCulture
    {
        get => uiCulture;
        set
        {
            var lang = value.Name.Split('-')[0];
            var culture = new CultureInfo(lang);
                        
            uiCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            Thread.CurrentThread.CurrentCulture = culture;
        }
    }

        
}