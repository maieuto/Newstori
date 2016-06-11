using System;
using Xamarin.Forms;

namespace HyperlocalNews
{
	public class BackContentPage : ContentPage
	{
		/*
		 * subclassing this page will overrule the iOS apps back button and swap it out for a localised back button.
		 * The only rubbish part is that apple don't allow you to create a custom back button and maintain the style,
		 * so you will lose the back arrow.
		 */

		public void OnPopping(){
			this.Navigation.PopAsync ();
		}
	}
}

