using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace HyperlocalNews
{
	public partial class AdminSetup : ContentPage
	{

		private Action onCompletion;

		public AdminSetup (Action completionMethod)
		{
			InitializeComponent ();

			onCompletion = completionMethod;

			UserMgr.getInstance ().CheckUserHyperlocalStatus (AdminCheckReturned);
		}

		private void AdminCheckReturned(bool success, string message){
			if(success){
				if (UserMgr.getInstance ().AdministeringHyperLocal() != null) {
					onCompletion ();
				} else {
					this.FindByName<StackLayout> ("ActivityView").IsVisible = false;
					this.FindByName<StackLayout> ("layout").IsVisible = true;
				}
			}
		}



		void CreateHyperlocal(object sender, EventArgs args){

			RegisterNewHyperlocal register = new RegisterNewHyperlocal(AdminIsSetup);

			App.navigation.PushAsync (register);
		}

		void JoinHyperlocal(object sender, EventArgs args){

			VerifyHyperlocalAdmin verify = new VerifyHyperlocalAdmin ();

			App.navigation.PushAsync (verify);
		}

		void AdminIsSetup(){

			onCompletion ();
		}
	}
}

