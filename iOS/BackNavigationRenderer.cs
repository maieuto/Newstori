using System;

using HyperlocalNews;
using HyperlocalNews.iOS;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly:ExportRenderer(typeof(BackContentPage), typeof(BackNavigationPageRenderer))]

namespace HyperlocalNews.iOS
{
	public class BackNavigationPageRenderer : PageRenderer
	{
		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			if (UserMgr.getInstance().PreferedLanguage() != "eng") {

				var root = this.NavigationController.TopViewController;

				root.NavigationItem.SetLeftBarButtonItem (new UIKit.UIBarButtonItem (TranslateExtension.Localize ("BackLabel"), UIKit.UIBarButtonItemStyle.Plain, (sender, args) => {
					//(Element as BackContentPage).OnPopping();
					((BackContentPage)Element).OnPopping ();

				}), true);
			}

		}
	}
}

