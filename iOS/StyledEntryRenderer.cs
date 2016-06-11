using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

using UIKit;

[assembly: ExportRenderer (typeof (HyperlocalNews.StyledEntry), typeof (HyperlocalNews.iOS.StyledEntryRenderer))]

namespace HyperlocalNews.iOS
{
	public class StyledEntryRenderer : EntryRenderer
	{
		// Override the OnElementChanged method so we can tweak this renderer post-initial setup
		protected override void OnElementChanged (ElementChangedEventArgs<Entry> e)
		{
			base.OnElementChanged (e);

			if (Control != null) {   // perform initial setup
				// do whatever you want to the UITextField here
				Control.BorderStyle = UITextBorderStyle.RoundedRect;
				Control.Layer.CornerRadius = 0;
				Control.Layer.BorderWidth = 3;
				Control.Layer.BorderColor = BrandColors.LightGrey.ToCGColor();

			}
		}
	}
}