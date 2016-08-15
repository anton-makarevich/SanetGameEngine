using System.IO.IsolatedStorage;



namespace Sanet.Common
{
    public static class LocalSettings
    {
        
        public static string GetValue(string key)
        {
            key = CheckKey(key);
            return IsolatedStorageSettings.ApplicationSettings.Contains(key) ? IsolatedStorageSettings.ApplicationSettings[key].ToString() : null;
        }

        public static  void SetValue(string key, object value)
        {
            IsolatedStorageSettings.ApplicationSettings[key] = value;
            IsolatedStorageSettings.ApplicationSettings.Save();
        }

        
        static string CheckKey(string key)
        {
            key=key.Replace(" ", "_");
            return key;
        }

        public static void InitLocalSettings() { }
        
    }
   
}
