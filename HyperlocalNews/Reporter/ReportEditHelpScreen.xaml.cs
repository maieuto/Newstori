using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace HyperlocalNews
{
	public partial class ReportEditHelpScreen : ContentPage
	{
		public ReportEditHelpScreen ()
		{
			InitializeComponent ();

			Title = TranslateExtension.Localize("GuideLabel");
		}
	}
}

