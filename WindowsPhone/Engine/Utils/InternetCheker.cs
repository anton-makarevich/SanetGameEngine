
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;



namespace Sanet.Common
{
    public static class InternetCheker
    {
        /// <summary>
        /// Check for internet connection
        /// </summary>
        /// <returns></returns>
        public static bool IsInternetAvailable()
        {
            return NetworkInterface.GetIsNetworkAvailable();
            
        }
    }
}
