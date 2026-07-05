
using Android.Content;
using Android.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giftr.Common
{
    public static class InternetCheker
    {
        static Context _androidContext;

        public static void Init(Context context)
        {
            _androidContext = context;
        }

        /// <summary>
        /// Check for internet connection
        /// </summary>
        /// <returns></returns>
        public static bool IsInternetAvailable()
        {
            try
            {

                var connectivityManager = (ConnectivityManager)_androidContext.GetSystemService(Context.ConnectivityService);

                var activeConnection = connectivityManager.ActiveNetworkInfo;

                if ((activeConnection != null) && activeConnection.IsConnected)
                {
                    return true;
                }
            }
            catch { }
            return false;
        }
    }
}
