using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Resources;
using System.Globalization;
using System.Reflection;

namespace HyperlocalNews
{

	public class ReporterNavigationPage : NavigationPage{
		public ReporterNavigationPage(Page root): base(root){}
	}



	public class App : Application
	{

		private string verificationToken;

		public static NavigationPage navigation;

		public App ()
		{
			SetupStyleResources ();
		}
		public App (string tok, string language)
		{
			SetupStyleResources ();

			verificationToken = tok;
		}

		public void SetTokenAndLanguage(string tok, string language){

			verificationToken = tok;

			if(!UserMgr.getInstance().HasAUser()){
				VerifyUser ();
			}
		}

		private void LaunchStandardUserProcess(){

			if (UserMgr.getInstance ().HasSelectedLanguagePreference()) {
				LanguageSelected ();
			} else {
				LanguagePage lp = new LanguagePage (LanguageSelected);

				try
				{
					Application.Current.MainPage = lp;
				}
				catch (Exception ex)
				{
					// Only happens in iOS
				}
			}
		}

		private void LaunchAdminUserProcess(){
			//force an admin permission check on access.
			LaunchAdminSetup ();
		}

		private void LanguageSelected(){

			//Were we launched with a verification token? IF so, deal with that here. Check it, if good, launch user home.
			if (UserMgr.getInstance ().HasAUser()) {
				LaunchUserHome ();
			} else {
				LaunchUserSetup ();
			}
		}

		private void ReportButtonClicked (){
			ReportEditScreen res = new ReportEditScreen ();
			navigation.PushAsync (res);
		}

		public void ChangeLanguage(string alertTitle, string alertBody, string alertButton){
			TranslateExtension.UpdateCurrentLanguage ();

			LaunchUserHome ();

			navigation.DisplayAlert (alertTitle, alertBody, alertButton);
		}

		private void LaunchUserHome(){

			TabbedPage tabs = new TabbedPage (){ Title="Newstori" };

			tabs.Icon = "user.png";

			ToolbarItem editorLinkButton = new ToolbarItem ("Editor", "togglereporter.png", () => {
												LaunchAdminUserProcess ();
			}, ToolbarItemOrder.Default);

			ToolbarItem reportLinkButton = new ToolbarItem ("Report", "ic_logo.png", () => {
												ReportButtonClicked ();
			}, ToolbarItemOrder.Default);

			ReporterFeedbackScreen rfs = new ReporterFeedbackScreen ();
			rfs.ToolbarItems.Add (reportLinkButton);
			rfs.ToolbarItems.Add (editorLinkButton);
			tabs.Children.Add (rfs);

			ReporterDraftsScreen rds = new ReporterDraftsScreen();
			rds.ToolbarItems.Add (reportLinkButton);
			rds.ToolbarItems.Add (editorLinkButton);
			tabs.Children.Add (rds);

			ReporterSentScreen rss = new ReporterSentScreen();
			rss.ToolbarItems.Add (reportLinkButton);
			rss.ToolbarItems.Add (editorLinkButton);
			tabs.Children.Add (rss);

			tabs.Children.Add (new ProfileScreen());

			ReporterNavigationPage navigation = new ReporterNavigationPage(tabs);
			MessagingCenter.Subscribe <Object>(this, "ReportButtonHit", (sender) => {
				ReportButtonClicked();
			});

			App.SetNavigation (navigation);
		}


		public void LaunchUserSetup (){
			LoginPage lp = new LoginPage (UserIsLoggedIn);
			ReporterNavigationPage navigation = new ReporterNavigationPage (lp);
			App.SetNavigation (navigation);
		}


		public void UserIsLoggedIn (){
			LaunchUserHome ();
		}



		public void LaunchAdminHome (){
			AdminHyperlocalPage home = new AdminHyperlocalPage ();

			home.ToolbarItems.Add(
				new ToolbarItem("Reporter", "toggleservice.png", () =>{
					LaunchStandardUserProcess();
				}, ToolbarItemOrder.Default
				)
			);

			NavigationPage navigation = new NavigationPage(home);
			App.SetNavigation (navigation);
		}

		public void LaunchAdminSetup (){
			AdminSetup setup = new AdminSetup (AdminIsLoggedIn);

			setup.ToolbarItems.Add(
				new ToolbarItem("Reporter", "toggleservice.png", () =>{
					LaunchStandardUserProcess();
				}, ToolbarItemOrder.Default
				)
			);

			NavigationPage navigation = new NavigationPage(setup);
			App.SetNavigation (navigation);
		}

		public void AdminIsLoggedIn(){
			LaunchAdminHome ();
		}



		public static void SetNavigation(NavigationPage np){
			navigation = np;

			try{
				Application.Current.MainPage = navigation;
			}catch(Exception e){
				//
			}
		}


		public static void SetNavigation(ReporterNavigationPage np){
			navigation = np;

			navigation.Popped += (object sender, NavigationEventArgs e) =>
			{
				MessagingCenter.Send(navigation, "Reporter popped");
			};
			navigation.Pushed += (object sender, NavigationEventArgs e) =>
			{
				MessagingCenter.Send(navigation, "Reporter pushed");
			};

			try{
				Application.Current.MainPage = navigation;
			}catch(Exception e){
				//
			}
		}

		protected override void OnStart ()
		{
			if(verificationToken != null && ! UserMgr.getInstance ().HasAUser()){

				VerifyUser ();

			}else{
				LaunchStandardUserProcess ();
				// Handle when your app starts
			}
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
			//intentionally empty
		}

		protected override void OnResume ()
		{

			if(verificationToken != null && ! UserMgr.getInstance ().HasAUser()){

				VerifyUser ();

			}
			// Handle when your app resumes
		}

		private void VerifyUser(){

			VerifyUserToken vut = new VerifyUserToken(verificationToken, LaunchUserHome);

			Application.Current.MainPage = vut;

			verificationToken = null;
		}


		private void SetupStyleResources (){
		
			Application.Current.Resources = new ResourceDictionary ();

			Style styleNav = new Style (typeof(NavigationPage)) {
				Setters = {
					new Setter { Property = NavigationPage.BarBackgroundColorProperty, Value = BrandColors.LightPink },
					new Setter { Property = NavigationPage.BarTextColorProperty, Value = Color.White }
				}
			};

			Style styleCustomNav = new Style (typeof(ReporterNavigationPage)) {
				Setters = {
					new Setter { Property = NavigationPage.BarBackgroundColorProperty, Value = BrandColors.LightPink },
					new Setter { Property = NavigationPage.BarTextColorProperty, Value = Color.White },
				}
			};

			Style styleButton = new Style (typeof(Button)) {
				Setters = {
					new Setter { Property = Button.BackgroundColorProperty, Value = BrandColors.LightGrey },
					new Setter { Property = Button.TextColorProperty, Value = BrandColors.LightPink },
					new Setter { Property = Button.BorderRadiusProperty, Value = 0 }
				}
			};
			
			Style styleLabel = new Style (typeof(Label)) {
				Setters = {
					new Setter { Property = Label.XAlignProperty, Value = TextAlignment.Center },
					new Setter { Property = Label.TextColorProperty, Value = BrandColors.Black }
				}
			};

			Style styleStack = new Style (typeof(StackLayout)) {
				Setters = {
					new Setter { Property = StackLayout.PaddingProperty, Value = 10 }
				}
			};

			Style styleEntry = new Style (typeof(Entry)) {
				Setters = { 
					new Setter { Property = Entry.BackgroundColorProperty, Value = Color.FromHex("#eeeeee")  }
				}
			};

			Style styleStyledEntry = new Style (typeof(StyledEntry)) {
				Setters = { 
					new Setter { Property = Entry.BackgroundColorProperty, Value = Color.FromHex("#eeeeee")  },
					new Setter { Property = Entry.MinimumHeightRequestProperty, Value = "42dp" }
				}
			};

			Style styleFrame = new Style (typeof(Frame)) {
				Setters = {
					new Setter { Property = Frame.HasShadowProperty, Value = false },
					new Setter { Property = Frame.OutlineColorProperty, Value = Color.Transparent },
					new Setter { Property = Frame.BackgroundColorProperty, Value = Color.Transparent }
				}
			};

			Style styleEditor = new Style (typeof(Editor)) {
				Setters = { 
					new Setter { Property = Editor.BackgroundColorProperty, Value = Color.FromHex("#eeeeee")  }
				}
			};

			Style styleStyledEditor = new Style (typeof(StyledEditor)) {
				Setters = { 
					new Setter { Property = Editor.BackgroundColorProperty, Value = Color.FromHex("#eeeeee")  }
				}
			};

			Style styleDatePicker = new Style (typeof(DatePicker)) {
				Setters = { 
					new Setter { Property = DatePicker.BackgroundColorProperty, Value = Color.FromHex("#eeeeee")  }
				}
			};

			Style styleCell = new Style (typeof(Cell)) {
				Setters = { 
					new Setter { Property = VisualElement.BackgroundColorProperty, Value = Color.FromHex("#eeeeee") }
				}
			};

			Application.Current.Resources.Add (styleNav);
			Application.Current.Resources.Add (styleCustomNav);
			Application.Current.Resources.Add (styleButton);
			Application.Current.Resources.Add (styleLabel);
			Application.Current.Resources.Add (styleStack);
			Application.Current.Resources.Add (styleEntry);
			Application.Current.Resources.Add (styleStyledEntry);
			Application.Current.Resources.Add (styleEditor);
			Application.Current.Resources.Add (styleStyledEditor);
			Application.Current.Resources.Add (styleDatePicker);
			Application.Current.Resources.Add (styleCell);
			Application.Current.Resources.Add (styleFrame);
		}


	}


}
