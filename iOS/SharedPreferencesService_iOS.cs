using System;
using Foundation;

[assembly: Xamarin.Forms.Dependency(typeof(HyperlocalNews.iOS.SharedPreferencesService_iOS))]

namespace HyperlocalNews.iOS
{
	public class SharedPreferencesService_iOS : ISharedPreferencesService
	{
		public SharedPreferencesService_iOS ()
		{
		}

		public string getStringPref(string key){

			string value = NSUserDefaults.StandardUserDefaults.StringForKey(key); 
			if (value == null)
				return null;
			else
				return value;

		}


		public void putStringPref(string key, string value){

			NSUserDefaults.StandardUserDefaults.SetString(value, key); 
			NSUserDefaults.StandardUserDefaults.Synchronize ();

		}
	}
}

