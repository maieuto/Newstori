using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace HyperlocalNews
{
	public partial class RegisterNewHyperlocal : BackContentPage
	{

		private Action onCompletion;

		private double lat;
		private double lon;
		private bool hasLocation = false;


		public RegisterNewHyperlocal (Action completionMethod)
		{
			InitializeComponent ();

			Title = TranslateExtension.Localize("RegisterHyperlocalLabel");

			onCompletion = completionMethod;

			this.ToolbarItems.Add(
				new ToolbarItem(TranslateExtension.Localize("JoinLabel"), null, () =>{
					SubmitHyperlocalRegistration();
				}
				)
			);
		}

		protected override void OnAppearing ()
		{
			base.OnAppearing ();
		}


		private void SubmitHyperlocalRegistration(object sender, EventArgs args){
			SubmitHyperlocalRegistration ();
		}


		private void SubmitHyperlocalRegistration(){

			if (hasLocation) {
				string name = (this.FindByName <Entry> ("HyperlocalName")).Text;
				string email = (this.FindByName <Entry> ("Email")).Text;

				HyperlocalMgr.getInstance ().RegisterNewHyperlocal (RegistrationReturned, name, email, lat, lon, UserMgr.getInstance ().ID ());
				this.FindByName<StackLayout> ("ActivityView").IsVisible = true;
			} else {
				DisplayAlert (TranslateExtension.Localize("CannotRegisterTitle"), TranslateExtension.Localize("CannotRegisterLabel"), TranslateExtension.Localize("OKLabel"));
			}
		}


		private void RegistrationReturned(bool success, string error){
			this.FindByName<StackLayout> ("ActivityView").IsVisible = false;
			if (success) {
				DisplayAlert (TranslateExtension.Localize("RegistrationRequestedTitle"), TranslateExtension.Localize("RegistrationRequestedLabel"), TranslateExtension.Localize("OKLabel"));
			} else {
				DisplayAlert (TranslateExtension.Localize("RegistrationRequestFailedTitle"), TranslateExtension.Localize("RegistrationRequestFailedLabel")+error, TranslateExtension.Localize("OKLabel"));
			}
		}


		private void AttachLocation(object senderObj, EventArgs eventArgs){

			double latitudeForLaunch = 51.481581;
			double longitudeForLaunch = -3.179090;

			if(hasLocation){
				latitudeForLaunch = lat;
				longitudeForLaunch = lon;
			}

			LocationPage lp = new LocationPage (LocationReturned, latitudeForLaunch, longitudeForLaunch, TranslateExtension.Localize("LocationLabel"), false, !hasLocation);

			App.navigation.PushAsync (lp);

		}

		private void LocationReturned(bool success, double newLat, double newLon){
			if(success){
				hasLocation = true;
				lat = newLat;
				lon = newLon;

				App.navigation.PopAsync ();

				this.FindByName <Button>("GetTheLocation").BackgroundColor = BrandColors.DarkGreen;
				this.FindByName <Button>("GetTheLocation").TextColor = Color.White;
			}
		}
	}
}

