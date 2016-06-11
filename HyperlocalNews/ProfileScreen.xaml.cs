using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Xamarin.Forms;

namespace HyperlocalNews
{
	public partial class ProfileScreen : ContentPage
	{

		Hyperlocal selectedHyperlocal;
		string preferredLanguage = UserMgr.getInstance().PreferedLanguage();
		bool hasChangedLanguage = false;

		public ProfileScreen ()
		{
			InitializeComponent ();

			Icon = "ProfileIcon.png";

			(this.FindByName <Entry>("FullName")).Text = UserMgr.getInstance().Fullname();

			(this.FindByName <Entry>("Email")).Text = UserMgr.getInstance().Email();

			selectedHyperlocal = UserMgr.getInstance ().AssociatedHyperLocal();

			if(selectedHyperlocal != null){
				(this.FindByName <Button>("Hyperlocal")).Text = selectedHyperlocal.Name;
			}

			preferredLanguage = UserMgr.getInstance ().PreferedLanguage ();
			SetLanguageButtonText ();


			this.ToolbarItems.Clear ();
			this.ToolbarItems.Add(
				new ToolbarItem(TranslateExtension.Localize("UpdateLabel"), null, () =>{
					SubmitProfileUpdate();
				}
				)
			);
		}

		private void SwitchLanguage(object sender, EventArgs args){
			if (preferredLanguage == "eng") {
				preferredLanguage = "cym";
			} else {
				preferredLanguage = "eng";
			}
			SetLanguageButtonText ();

			hasChangedLanguage = !hasChangedLanguage;
		}
		private void SetLanguageButtonText(){
			if (preferredLanguage == "eng") {
				this.FindByName<Button> ("Language").Text = "English";
			} else {
				this.FindByName<Button> ("Language").Text = "Cymraeg";
			}
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

		void SubmitProfileUpdate(object sender, EventArgs args){
			SubmitProfileUpdate ();
		}

		void SubmitProfileUpdate(){
			string name = (this.FindByName <Entry> ("FullName")).Text;
			string email = (this.FindByName <Entry>("Email")).Text;
			UserMgr.getInstance ().UpdateUserProfile (SubmitProfileUpdateResponse, name, email, selectedHyperlocal, preferredLanguage);
		}

		private void SubmitProfileUpdateResponse(bool success, string message){
			if(success){
				if(hasChangedLanguage){
					string alertTitle = TranslateExtension.Localize("ProfileUpdatedTitleREVERSE");
					string alertBody = TranslateExtension.Localize("ProfileUpdatedLabelREVERSE");
					string alertButton = TranslateExtension.Localize("OKLabelREVERSE");
					((App)App.Current).ChangeLanguage (alertTitle, alertBody, alertButton);
				}else{
					string alertTitle = TranslateExtension.Localize("ProfileUpdatedTitle");
					string alertBody = TranslateExtension.Localize("ProfileUpdatedLabel");
					string alertButton = TranslateExtension.Localize("OKLabel");
					DisplayAlert (alertTitle, alertBody, alertButton);
				}
			}else{
				DisplayAlert (TranslateExtension.Localize("ProfileUpdateFailedTitle"), TranslateExtension.Localize("ProfileUpdateFailedLabel")+message, TranslateExtension.Localize("OKLabel"));
			}
		}
	}
}

