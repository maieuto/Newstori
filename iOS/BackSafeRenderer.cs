using System;

using HyperlocalNews;
using HyperlocalNews.iOS;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly:ExportRenderer(typeof(ReportEditScreen), typeof(BackSafeRenderer))]

namespace HyperlocalNews.iOS
{
	public class BackSafeRenderer : PageRenderer
	{
		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			// Xamarin.Forms wraps page's view controller
			ViewController.ParentViewController.NavigationItem.SetHidesBackButton(true, false);
		}

	}

}

