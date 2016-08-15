
using Coding4Fun.Toolkit.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanet.Common
{
    public static class UserCommunicator
    {
        /// <summary>
        /// Asyncronius version of user notification - waits until user closes it
        /// </summary>
        /// <param name="title">notification title</param>
        /// <param name="message">notification message</param>
        /// <returns></returns>
        public async static Task<bool> ShowMessageAsync(string title, string message)
        {
            var tcs = new TaskCompletionSource<bool>();
            SmartDispatcher.BeginInvoke(() =>
            {
                MessagePrompt messageBox = new MessagePrompt();
                messageBox.Title = title;
                messageBox.Message = message;
                messageBox.Completed += (s, e) =>
                    {
                        tcs.SetResult(true);
                    };
                messageBox.Show();
            });
            return await tcs.Task;

        }

        /// <summary>
        /// regular version of user notification, game continue while it's opened
        /// </summary>
        /// <param name="title">notification title</param>
        /// <param name="message">notification message</param>
        public static void ShowMessage(string title, string message)
        {
            SmartDispatcher.BeginInvoke(() =>
            {
                MessagePrompt messageBox = new MessagePrompt();
                messageBox.Title = title;
                messageBox.Message = message;
                messageBox.Show();
            });
        }
    }
}
