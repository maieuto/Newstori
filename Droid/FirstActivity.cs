using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Threading;

namespace HyperlocalNews.Droid
{

	[Activity (Theme = "@style/Theme.Splash",  NoHistory = true, Label = "Newstori", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize, ScreenOrientation = ScreenOrientation.Portrait)]
	[IntentFilter(new []{ Intent.ActionView }, DataScheme = "http", DataHost= "newstori.rantmedia.com", DataPathPrefix="/verifyuser/",
		Categories = new []{ Intent.CategoryDefault, Intent.CategoryBrowsable})]

	public class FirstActivity : global::Android.App.Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			
			base.OnCreate (bundle);

			Android.Net.Uri uri = Intent.Data;

			newIntent = new Intent(this, typeof(MainActivity));
			newIntent.AddFlags (ActivityFlags.ClearTask);
			newIntent.AddFlags (ActivityFlags.NewTask);

			if (uri != null) {
				newIntent.SetData (uri);
			}
		}
		private Intent newIntent;
		protected override void OnResume()
		{

			base.OnResume ();
			Thread.Sleep (2000); // 2 second delay for admin screen

			StartActivity(newIntent);

		}


		private Action<int, Result, Intent> _activityResultCallback;

		public void ConfigureActivityResultCallback(Action<int, Result, Intent> callback)
		{
			if (callback == null) throw new ArgumentNullException("callback");
			_activityResultCallback = callback;
		}

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);

			if (_activityResultCallback != null)
			{
				_activityResultCallback.Invoke(requestCode, resultCode, data);
				_activityResultCallback = null;
			}
		}

	}
}

