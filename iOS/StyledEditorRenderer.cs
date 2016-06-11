using System;
using System.Collections.Generic;
using System.Linq;
using HyperlocalNews;
using HyperlocalNews.iOS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Foundation;
using UIKit;

[assembly: ExportRenderer(typeof(StyledEditor), typeof(StyledEditorRenderer))]


namespace HyperlocalNews.iOS // The iOS-Namespace to my App 
{
	public class StyledEditorRenderer : EditorRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
		{
			base.OnElementChanged(e);
			if (e.OldElement == null)
			{   // perform initial setup
				// lets get a reference to the native control
				var nativeTextView = (UITextView)Control;

				foreach (UIView ChildView in nativeTextView.InputAccessoryView.Subviews) 
				{
					foreach (var btnChild in ChildView.Subviews.OfType<UIButton>())
					{
						if(btnChild.TitleLabel.Text == "Done")
							btnChild.SetTitle (TranslateExtension.Localize ("DoneLabel"), UIControlState.Normal);
					}
				}


			}
		}
	}
}

