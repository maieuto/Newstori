using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace HyperlocalNews
{
	public partial class LanguagePage : ContentPage
	{

		private Action onCompletion;


		public LanguagePage (Action completionAction)
		{
			InitializeComponent ();

			onCompletion = completionAction;
		}

		protected override void OnAppearing(){
			NavigationPage.SetHasNavigationBar (this, false);

		}

		void LanguageSelected(object sender, EventArgs args)
		{

			if(sender == this.FindByName <Button>("englishButton")){
				UserMgr.getInstance ().SetPreferedLanguage ("eng");

			}else if(sender == this.FindByName <Button>("cymraegButton")){
				UserMgr.getInstance ().SetPreferedLanguage ("cym");

			}

			onCompletion();
		}


	}
}

