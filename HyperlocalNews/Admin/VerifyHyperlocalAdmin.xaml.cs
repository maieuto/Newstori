using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace HyperlocalNews
{
	public partial class VerifyHyperlocalAdmin : ContentPage
	{

		private Hyperlocal selectedHyperlocal;

		public VerifyHyperlocalAdmin ()
		{
			InitializeComponent ();

			Title = TranslateExtension.Localize("JoinHyperlocalLabel");

			this.ToolbarItems.Add(
				new ToolbarItem(TranslateExtension.Localize("JoinLabel"), null, () =>{
					SubmitHyperlocalVerificationRequest();
				}
				)
			);
		}

		private HyperlocalSelectorPage hyperlocalSelector;

		public void SelectHyperlocal(object sender, EventArgs args){
			this.FindByName<StackLayout> ("ActivityView").IsVisible = true; 
			hyperlocalSelector = new HyperlocalSelectorPage (HyperlocalWasSelected, HyperlocalMgr.getInstance().GetHyperlocals(GetHyperlocalsReturned));
		}
		private void GetHyperlocalsReturned(bool success, string message){
			this.FindByName<StackLayout> ("ActivityView").IsVisible = false; 
			App.navigation.PushAsync (hyperlocalSelector);
		}

		private void HyperlocalWasSelected(Hyperlocal h){
			selectedHyperlocal = h;

			(this.FindByName <Button>("Hyperlocal")).Text = selectedHyperlocal.Name;
		}

		private void SubmitHyperlocalVerificationRequest(object sender, EventArgs args){
			SubmitHyperlocalVerificationRequest ();
		}

		private void SubmitHyperlocalVerificationRequest(){
			HyperlocalMgr.getInstance ().VerifyNewHyperlocalAdministratorRequest (RegistrationReturned, UserMgr.getInstance().ID(), selectedHyperlocal);
			this.FindByName<StackLayout> ("ActivityView").IsVisible = true;
		}


		private void RegistrationReturned(bool success, string message){
			this.FindByName<StackLayout> ("ActivityView").IsVisible = false;
			if (success) {
				DisplayAlert (TranslateExtension.Localize("VerificationRequestedTitle"),TranslateExtension.Localize("VerificationRequestedLabel"), TranslateExtension.Localize("OKLabel"));
			} else {
				DisplayAlert (TranslateExtension.Localize("VerificationRequestFailedTitle"),TranslateExtension.Localize("VerificationRequestFailedLabel")+message, TranslateExtension.Localize("OKLabel"));
			}
		}
	}
}

