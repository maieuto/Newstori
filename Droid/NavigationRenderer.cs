using System;

using HyperlocalNews;
using HyperlocalNews.Droid;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

[assembly: ExportRenderer(typeof(ReporterNavigationPage), typeof(ReporterNavigationPageRenderer))]

namespace HyperlocalNews.Droid
{
	public class ReporterNavigationPageRenderer : NavigationRenderer
	{

		protected override void OnElementChanged(ElementChangedEventArgs<NavigationPage> e){

			base.OnElementChanged (e);
		}

		private void HideReportButton(){
			//intentionally empty
		}

		private void ShowReportButton(){
			//intentionally empty
		}

	}
}

