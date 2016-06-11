using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace HyperlocalNews
{
	public partial class ExistingUserLogin : ContentPage
	{
		Action onCompletion;

		public ExistingUserLogin (Action completionAction)
		{
			InitializeComponent ();

			onCompletion = completionAction;

			this.ToolbarItems.Add(
				new ToolbarItem(TranslateExtension.Localize("RecoverTitleAlt"), null, () =>{
					SubmitRequest();
				}
				)
			);
		}


		private void SubmitRequest(object sender, EventArgs args){
			SubmitRequest ();
		}

		private void SubmitRequest(){
			string email = (this.FindByName <Entry>("Email")).Text;
			UserMgr.getInstance ().RequestExistingUserRecovery (SubmitRequestReturned, email);
			this.FindByName<StackLayout> ("ActivityView").IsVisible = true;
		}


		private void SubmitRequestReturned(bool success, string message){
			this.FindByName<StackLayout> ("ActivityView").IsVisible = false;
			if (success) {
				DisplayAlert (TranslateExtension.Localize("UserVerificationRequestProcessedTitle"), TranslateExtension.Localize("UserVerificationRequestProcessedLabel"), TranslateExtension.Localize("OKLabel"));
			} else {
				DisplayAlert (TranslateExtension.Localize("UserVerificationRequestFailedTitle"), TranslateExtension.Localize("UserVerificationRequestFailedLabel")+message, TranslateExtension.Localize("OKLabel"));
			}
		}
	}
}

