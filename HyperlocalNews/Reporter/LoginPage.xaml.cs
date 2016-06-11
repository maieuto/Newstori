using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace HyperlocalNews
{
	public partial class LoginPage : ContentPage
	{

		private Action onCompletionMethod;

		public LoginPage (Action completionAction)
		{
			onCompletionMethod = completionAction;

			InitializeComponent ();
		}

		void Join(object sender, EventArgs args){

			RegisterNewUser register = new RegisterNewUser(UserLoggedIn);

			App.navigation.PushAsync (register);
		}

		void Existing(object sender, EventArgs args){

			ExistingUserLogin login = new ExistingUserLogin (UserLoggedIn);
		
			App.navigation.PushAsync (login);
		}

		private void UserLoggedIn(){
			onCompletionMethod ();
		}

		private void TestToken(object sender, EventArgs args){
			VerifyUserToken vut = new VerifyUserToken(this.FindByName<Entry> ("testToken").Text, null);
		}
	}
}

