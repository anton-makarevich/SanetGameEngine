using Android.App;
using Android.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giftr.Common
{
    public static class UserCommunicator
    {
        static Context _activity;

        public static void Init(Context activity)
        {
            _activity = activity;
        }
        
        /// <summary>
        /// Asyncronius version of user notification - waits until user closes it
        /// </summary>
        /// <param name="title">notification title</param>
        /// <param name="message">notification message</param>
        /// <returns></returns>
        public async static Task<bool> ShowMessageAsync(string title, string message)
        {
            var tcs = new TaskCompletionSource<bool>();
            AlertDialog.Builder builder = new AlertDialog.Builder(_activity);
            builder.SetTitle(title);
            builder.SetMessage(message);
            builder.SetPositiveButton("Ok", (sender, e) =>
            {
                tcs.SetResult(true);
            });

            builder.Show();
            return await tcs.Task;

        }
        
        /// <summary>
        /// regular version of user notification, game continue while it's opened
        /// </summary>
        /// <param name="title">notification title</param>
        /// <param name="message">notification message</param>
        public static void ShowMessage(string title, string message)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(_activity);
            builder.SetTitle(title);
            builder.SetMessage(message);
            builder.SetPositiveButton("Ok", (sender, e) =>
            {
                
            });

            builder.Show();
            
        }
    }
}
