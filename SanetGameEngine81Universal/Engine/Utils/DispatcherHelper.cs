using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace Sanet.XNAEngine
{
    public static class SmartDispatcher
    {
        private static CoreDispatcher _instance;
        private static void RequireInstance()
        {
            try
            {
                _instance = Window.Current.CoreWindow.Dispatcher;

            }
            catch (Exception e)
            {
                throw new InvalidOperationException("The first time SmartDispatcher is used must be from a user interface thread. Consider having the application call Initialize, with or without an instance.", e);
            }

            if (_instance == null)
            {
                throw new InvalidOperationException("Unable to find a suitable Dispatcher instance.");
            }
        }

        public static void Initialize(CoreDispatcher dispatcher)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException("dispatcher");
            }

            _instance = dispatcher;
        }

        public static bool CheckAccess()
        {
            if (_instance == null)
            {
                RequireInstance();
            }
            return _instance.HasThreadAccess;
        }

        public static void BeginInvoke(Action a)
        {
            if (_instance == null)
            {
                RequireInstance();
            }

            // If the current thread is the user interface thread, skip the
            // dispatcher and directly invoke the Action.
            if (CheckAccess())
            {
                a();
            }
            else
            {
                _instance.RunAsync(CoreDispatcherPriority.Normal, () => { a(); });
            }
        }
    }
}
