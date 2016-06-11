using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace HyperlocalNews
{
	public partial class VerifyUserToken : ContentPage
	{

		Action OnCompletion;
		string theToken;

		public VerifyUserToken (string token, Action CompletionMethod)
		{
			InitializeComponent ();

			OnCompletion = CompletionMethod;

			theToken = token;

			this.FindByName<Label> ("TokenLabel").Text = "Verifying with token: "+token;

		}


		protected override void OnAppearing ()
		{
			base.OnAppearing ();


			UserMgr.getInstance ().VerifyUserRecovery (TokenVerified, theToken);
		}

		public void TokenVerified(bool success, string message){
			if (success) {
				OnCompletion ();
			} else {
				DisplayAlert (TranslateExtension.Localize("UserVerifyScreenTitle"), TranslateExtension.Localize("UserVerifyScreenMessage"), TranslateExtension.Localize("OKLabel"));
			}
		}
	}
}