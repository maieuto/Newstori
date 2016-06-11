using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

using Foundation;
using UIKit;

using XLabs.Platform.Services.Geolocation;

namespace HyperlocalNews.iOS
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : XLabs.Forms.XFormsApplicationDelegate
	{
		string verificationToken = null;
		string language = "eng";

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			Forms.Init();
			Xamarin.FormsMaps.Init();

			//register XLabs Geolocator service
			DependencyService.Register<Geolocator>();

			LoadApplication (new App());

			if (UIDevice.CurrentDevice.CheckSystemVersion (7, 0)) {
				// Code that uses features from Xamarin.iOS 7.0
				UITabBar.Appearance.BarTintColor = BrandColors.DarkPink.ToUIColor ();
			} else {
				//no alternative
			}

			UITabBar.Appearance.SelectedImageTintColor = BrandColors.LightGrey.ToUIColor ();

			UITextAttributes textAttributes = new UITextAttributes ();
			textAttributes.TextColor = BrandColors.LightGrey.ToUIColor ();
			UITabBarItem.Appearance.SetTitleTextAttributes (textAttributes, UIControlState.Normal);

			UITextAttributes selectedTextAttributes = new UITextAttributes ();
			selectedTextAttributes.TextColor = BrandColors.LightGrey.ToUIColor ();
			UITabBarItem.Appearance.SetTitleTextAttributes (selectedTextAttributes, UIControlState.Selected);

			return base.FinishedLaunching (app, options);
		}

		public override bool OpenUrl (UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
		{
			Console.WriteLine (url);

			if(url.AbsoluteString.StartsWith("newstori://newstori.rantmedia.com/verifyuser/")){
				if(verificationToken == null){
					verificationToken = url.LastPathComponent;
					Console.WriteLine (verificationToken);
					string query = url.Query;
					if (query.Contains ("Language=")) {
						language = query.Replace ("Language=", "");
						if(!language.Equals("cym")){
							language = "eng";
						}
					}
					if(App.Current != null){
						((App)App.Current).SetTokenAndLanguage (verificationToken, language);

					}
				}
				return true;
			}else{
				return false;
			}
		}
	}
}