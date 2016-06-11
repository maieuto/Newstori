using System;
using System.Collections.ObjectModel;

using Xamarin.Forms;
using HyperlocalNews;

namespace HyperlocalNews
{
	public partial class RegisterNewUser : BackContentPage
	{

		public bool OverrideBackButton = true;

		Action onCompletionMethod;

		Hyperlocal selectedHyperlocal;

		public RegisterNewUser (Action completionAction)
		{

			onCompletionMethod = completionAction;

			InitializeComponent ();

			this.ToolbarItems.Add(
				new ToolbarItem(TranslateExtension.Localize("JoinButtonLabel"), null, () =>{
					SubmitProfileRegistration();
				}
				)
			);

			var html = new HtmlWebViewSource {
				Html = TranslateExtension.Localize("RegisterNewHyperlocalSuggestionHTML")
			};
			WebView wv = this.FindByName <WebView> ("webView");
			wv.Source = html;

			wv.Navigating += (object sender, WebNavigatingEventArgs e) => {
				int i = 0;
				if (Uri.IsWellFormedUriString(e.Url, UriKind.Absolute))
				{
					Device.OpenUri(new Uri(e.Url));
					e.Cancel = true;
				}
			};

		}

		private void LaunchURL(){
			Device.OpenUri (new Uri("http://www.communityjournalism.co.uk"));
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


		private void SubmitProfileRegistration(object sender, EventArgs args){
			SubmitProfileRegistration();
		}
		private void SubmitProfileRegistration(){
			string name = (this.FindByName <Entry> ("FullName")).Text;
			string email = (this.FindByName <Entry>("Email")).Text;
			UserMgr.getInstance ().RegisterNewUser (RegistrationReturned, name, email, selectedHyperlocal);
			this.FindByName<StackLayout> ("ActivityView").IsVisible = true;
		}


		private void RegistrationReturned(bool success, string error){
			this.FindByName<StackLayout> ("ActivityView").IsVisible = false;
			if (success) {
				onCompletionMethod ();
			} else {
				DisplayAlert (TranslateExtension.Localize("UserRegistrationFailedTitle"), TranslateExtension.Localize("UserRegistrationFailedLabel")+error, TranslateExtension.Localize("OKLabel"));
			}
		}
			
	}
}

