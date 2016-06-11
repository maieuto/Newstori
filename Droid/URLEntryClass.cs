
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Xamarin.Forms;

namespace HyperlocalNews.Droid
{
	[Activity (Label = "urlentryclass")]			
	public class URLEntryClass : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Create your application here
			string token = Intent.Data.EncodedAuthority;

			MessagingCenter.Send (this, "UserLoginToken", token);
		}

	}
}

