using System;
using Xamarin.Forms;

namespace HyperlocalNews
{
	public abstract class BackButtonOverridePage : ContentPage
	{

		/* 	If a page subclasses this page, implenting the OnBackButton 
		 * 	will override the android hardware back button behaviour.
		 */

		public abstract void OnBackButton();
	}
}

