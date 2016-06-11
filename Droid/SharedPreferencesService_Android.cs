using System;

using Android.App;
using Android.Content;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(HyperlocalNews.Droid.SharedPreferencesService_Android))]

namespace HyperlocalNews.Droid
{
	public class SharedPreferencesService_Android : Java.Lang.Object, ISharedPreferencesService
	{
		public SharedPreferencesService_Android ()
		{
		}

		public string getStringPref(string key){

			MainActivity androidContext = (MainActivity)Forms.Context;

			ISharedPreferences sharedPref = androidContext.GetSharedPreferences("prefs", FileCreationMode.Private);

			return sharedPref.GetString(key, null);
		}


		public void putStringPref(string key, string value){

			MainActivity androidContext = (MainActivity)Forms.Context;

			ISharedPreferences sharedPref = androidContext.GetSharedPreferences("prefs", FileCreationMode.Private);

			ISharedPreferencesEditor edit = sharedPref.Edit();

			edit.PutString(key, value);

			edit.Commit();
		}
	}
}

