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

[assembly:ExportRenderer(typeof(ReportEditScreen), typeof(BackSafeRenderer))]

namespace HyperlocalNews.Droid
{
	public class BackSafeRenderer : PageRenderer
	{

		protected override void OnElementChanged(ElementChangedEventArgs<Page> e){

			base.OnElementChanged (e);
			var actionBar = ((Activity)Context).ActionBar;	
			actionBar.SetHomeButtonEnabled (false);         
			actionBar.SetDisplayHomeAsUpEnabled (false);    
			actionBar.SetDisplayShowHomeEnabled (false);	
		}
		protected override void OnDraw (global::Android.Graphics.Canvas canvas)
		{
			base.OnDraw (canvas);

			var actionBar = ((Activity) Context).ActionBar;	

			actionBar.SetHomeButtonEnabled (false);         // Don't activate button behavior
			actionBar.SetDisplayHomeAsUpEnabled (false);    // Don't show back arrow
			actionBar.SetDisplayShowHomeEnabled (false);    // Don't show back arrow and icon
		}
	}

}