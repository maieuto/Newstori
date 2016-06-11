using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using System.Threading;

using Xamarin.Forms;

using XLabs.Platform.Services.Geolocation;

namespace HyperlocalNews.Droid
{
	[Activity (Theme = "@style/MyTheme", Label = "Newstori", Icon = "@drawable/transparent", ConfigurationChanges = ConfigChanges.ScreenSize, ScreenOrientation = ScreenOrientation.Portrait)]
	public class MainActivity : XLabs.Forms.XFormsApplicationDroid
	{

		Bundle b;

		App app;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			//register XLabs Geolocator service
			DependencyService.Register<Geolocator> ();

			b = bundle;

			global::Xamarin.Forms.Forms.Init (this, bundle);
			Xamarin.FormsMaps.Init (this, bundle);

			Android.Net.Uri uri = Intent.Data;

			string token = null;
			string language = "eng";

			if(uri != null){
				string firstFegment = uri.PathSegments [0];
				if(firstFegment.Equals("verifyuser")){
					token = uri.LastPathSegment;

					Console.WriteLine (token);
				}
				string l = uri.GetQueryParameter("Language");
				if(l !=null){
					language = l;
					if(!language.Equals("cym")){
						language = "eng";
					}
				}
			}

			if (!String.IsNullOrEmpty(token) ) {
				LoadApplication (new App (token, language));
			} else {
				LoadApplication (new App ());
			}
		}

		private void LoadXamarin(){

			Bundle bundle = b;

			global::Xamarin.Forms.Forms.Init (this, bundle);
			Xamarin.FormsMaps.Init (this, bundle);

			Android.Net.Uri uri = Intent.Data;

			string token = null;
			string language = "eng";

			if(uri != null){
				string firstFegment = uri.PathSegments [0];
				if(firstFegment.Equals("verifyuser")){
					token = uri.LastPathSegment;
				}
				string l = uri.GetQueryParameter("Language");
				if(l !=null){
					language = l;
				}
			}

			if (!String.IsNullOrEmpty(token) ) {
				LoadApplication (new App (token, language));
			} else {
				LoadApplication (new App ());
			}
		}

		protected override void OnResume(){
			base.OnResume();
			//LoadXamarin ();
		}




		public override void OnBackPressed()
		{
			Object page = App.navigation.CurrentPage;
			if (page is BackButtonOverridePage) {
				((BackButtonOverridePage)App.navigation.CurrentPage).OnBackButton ();
			} else {
				base.OnBackPressed ();
			}
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

