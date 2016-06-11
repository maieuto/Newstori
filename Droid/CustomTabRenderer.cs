using System;

using Android.App;
using Android.Content;
using Xamarin.Forms;
using Android.Renderscripts;

namespace HyperlocalNews.Droid
{
	public class CustomTabRenderer : TabbedPage
	{
		public CustomTabRenderer ()
		{
		}

		private Activity activity;
		private const string COLOR = "#f7f8f8";

		//This flag is used in the case when the app is not completely closed, and the user return back.
		private bool isFirstDesign = true;

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);
			activity = this.Context as Activity;
		}

		protected override void OnWindowVisibilityChanged(ViewStates visibility)
		{
			base.OnWindowVisibilityChanged(visibility);
			if (isFirstDesign)
			{
				ActionBar actionBar = activity.ActionBar;

				ColorDrawable colorDrawable = new ColorDrawable(Android.Graphics.Color.ParseColor(COLOR));
				actionBar.SetStackedBackgroundDrawable(colorDrawable);
				ActionBarTabsSetup(actionBar);

				isFirstDesign = false; 
			}
		}

		private void ActionBarTabsSetup(ActionBar actionBar)
		{
			Android.App.ActionBar.Tab keypad = actionBar.GetTabAt(0);
			if (TabIsEmpty(keypad))
				TabSetup(keypad, Resource.Drawable.keypad);

			Android.App.ActionBar.Tab contacts = actionBar.GetTabAt(1);
			if (TabIsEmpty(contacts))
				TabSetup(contacts, Resource.Drawable.contacts);

			Android.App.ActionBar.Tab favorites = actionBar.GetTabAt(2);
			if (TabIsEmpty(favorites))
				TabSetup(favorites, Resource.Drawable.favorites);

			Android.App.ActionBar.Tab callsLog = actionBar.GetTabAt(3);
			if (TabIsEmpty(callsLog))
				TabSetup(callsLog, Resource.Drawable.calls_log);

			Android.App.ActionBar.Tab chat = actionBar.GetTabAt(4);
			if (TabIsEmpty(chat))
				TabSetup(chat, Resource.Drawable.messages);
		}

		private bool TabIsEmpty(ActionBar.Tab tab)
		{
			if (tab != null)
			if (tab.CustomView == null)
				return true;
			return false;
		}

		private void TabSetup(ActionBar.Tab tab, int resourceID)
		{
			ImageView iv = new ImageView(activity);
			iv.SetImageResource(resourceID);
			iv.SetPadding(-35, 8, -35, 16);

			tab.SetCustomView(iv);
		}
	}
}

