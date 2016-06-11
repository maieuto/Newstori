using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Xamarin.Forms;

namespace HyperlocalNews
{
	public class UserMgr
	{

		/* singleton manager */
		private static UserMgr instance;

		private UserMgr () {}

		public static UserMgr getInstance(){

			if(instance == null){
				instance = new UserMgr();
				instance.RetrieveSavedUser ();
			}
			return instance;
		}

		private User currentUser;

		private string selectedLanguage;


		public void RegisterNewUser(Action<bool, string> receiver,string fullName, string email, Hyperlocal hl){
			if(currentUser == null){
				currentUser = new User();
			}
			User editedUser = currentUser;
			editedUser.Language = this.PreferedLanguage ();
			editedUser.Fullname = fullName;
			editedUser.Email = email;

			string requestString = JsonConvert.SerializeObject (currentUser);

			//may be null
			if(hl != null){
				JObject o = JObject.Parse (requestString);
				o.Add ("AssociatedHyperLocalID", hl.ID);
				requestString = o.ToString ();

				editedUser.AssociatedHyperLocal = hl;
			}


			WebServiceRequestWithReturnFunctionAndVariable wsmgr = new WebServiceRequestWithReturnFunctionAndVariable();
			wsmgr.returnFunction = receiver;
			wsmgr.returnObject = editedUser;

			wsmgr.PostRequest (WebServicesMgr.WSCall.CreateNewUser, requestString, RegisterNewUserReturned);
		}
		private void RegisterNewUserReturned(bool success, string message, string JSON, Object wsmgr){

			WebServiceRequestWithReturnFunctionAndVariable webServicesMgr = (WebServiceRequestWithReturnFunctionAndVariable)wsmgr;

			if (success) {

				//returned object contains ID and token.
				currentUser = JsonConvert.DeserializeObject <User>(JSON);

				SaveUser ();
				message = "User registration complete";
			}

			if(webServicesMgr.returnFunction != null){
				webServicesMgr.returnFunction (success, message);
			}
		}


		public void RequestExistingUserRecovery(Action<bool, string> receiver, string email){

			WebServiceRequestWithReturnFunctionAndVariable wsmgr = new WebServiceRequestWithReturnFunctionAndVariable();
			wsmgr.returnFunction = receiver;
			wsmgr.returnObject = email;

			User userObjectForRequest = new User ();
			userObjectForRequest.Email = email;
			userObjectForRequest.Language = this.PreferedLanguage();

			string requestString = JsonConvert.SerializeObject (userObjectForRequest);

			wsmgr.PostRequest (WebServicesMgr.WSCall.RequestUserVerification, requestString, RequestExistingUserRecoveryReturned);
		}
		private void RequestExistingUserRecoveryReturned(bool success, string message, string JSON, Object wsmgr){

			WebServiceRequestWithReturnFunctionAndVariable webServicesMgr = (WebServiceRequestWithReturnFunctionAndVariable)wsmgr;

			if (success) {
				message = "User verification requested";
			}

			if(webServicesMgr.returnFunction != null){
				webServicesMgr.returnFunction (success, message);
			}
		}


		public void VerifyUserRecovery(Action<bool, string> receiver, string token){

			WebServiceRequestWithReturnFunctionAndVariable wsmgr = new WebServiceRequestWithReturnFunctionAndVariable();
			wsmgr.returnFunction = receiver;

			wsmgr.singleUseToken = token;

			wsmgr.GetRequest (WebServicesMgr.WSCall.GetUserProfileFromToken, VerifyUserRecoveryReturned);
		}
		public void VerifyUserRecoveryReturned(bool success, string message, string JSON, Object wsmgr){
			WebServiceRequestWithReturnFunctionAndVariable webServicesMgr = (WebServiceRequestWithReturnFunctionAndVariable)wsmgr;

			if (success) {

				message = "User Verified";

				currentUser = JsonConvert.DeserializeObject<User> (JSON);

				SaveUser ();

				SetPreferedLanguage (currentUser.Language);
			}

			if(webServicesMgr.returnFunction != null){
				webServicesMgr.returnFunction (success, message);
			}
		}


		public void UpdateUserProfile(Action<bool, string> receiver,string fullName, string email, Hyperlocal hl, string preferredLanguage){
			if(currentUser == null){
				currentUser = new User();
			}

			User editedUser = currentUser;

			Hyperlocal administrativeHyperlocal = editedUser.AdministeringHyperLocal;


			editedUser.Language = preferredLanguage;
			editedUser.Fullname = fullName;
			editedUser.Email = email;

			string requestString = JsonConvert.SerializeObject (editedUser);

			JObject o = JObject.Parse (requestString);
			o.Remove ("AssociatedHyperLocal");
			//may be null
			if(hl != null){
				o.Add ("AssociatedHyperLocalID", hl.ID);
			}


			o.Remove ("AdministeringHyperLocal");
			//may be null
			if(administrativeHyperlocal != null){
				o.Add ("AdministeringHyperLocalID", administrativeHyperlocal.ID);
			}

			requestString = o.ToString ();
			editedUser.AssociatedHyperLocal = hl;
			editedUser.AdministeringHyperLocal = administrativeHyperlocal;


			WebServiceRequestWithReturnFunctionAndVariable wsmgr = new WebServiceRequestWithReturnFunctionAndVariable();
			wsmgr.returnFunction = receiver;
			wsmgr.returnObject = editedUser;

			//the web services manager needs a user object to create the request URL.
			wsmgr.user = currentUser;
			wsmgr.PostRequest (WebServicesMgr.WSCall.UpdateUserProfile, requestString, UpdateUserProfileReturned);
		}

		public void UpdateUserProfileReturned(bool success, string message, string JSON, Object wsmgr){

			WebServiceRequestWithReturnFunctionAndVariable webServicesMgr = (WebServiceRequestWithReturnFunctionAndVariable)wsmgr;

			if (success) {

				message = "User updated";

				currentUser = (User)webServicesMgr.returnObject;

				SaveUser ();

				SetPreferedLanguage (currentUser.Language);
			}

			if(webServicesMgr.returnFunction != null){
				webServicesMgr.returnFunction (success, message);
			}
		}

		public string Fullname(){
			return currentUser.Fullname;
		}
		public string Email(){
			return currentUser.Email;
		}
		public Hyperlocal AssociatedHyperLocal(){
			return currentUser.AssociatedHyperLocal;
		}
		public Hyperlocal AdministeringHyperLocal(){
			return currentUser.AdministeringHyperLocal;
		}
		public string ID(){
			return currentUser.ID;
		}


		public User GetUserObject(){
			return currentUser;
		}


		public bool HasSelectedLanguagePreference(){

			bool hasSelectedLanguage = false;
		
			ISharedPreferencesService sps = DependencyService.Get<ISharedPreferencesService>();
			string savedLanguage = sps.getStringPref ("Language");


			if(savedLanguage != null){
				//fill this so we won't need to retrieve it again in the PreferredLanguage() method.
				selectedLanguage = savedLanguage;

				hasSelectedLanguage = true;
			}

			return hasSelectedLanguage;
		}


		public string PreferedLanguage(){

			//default to en_GB
		
			if (selectedLanguage == null) {

				selectedLanguage = "eng";
				ISharedPreferencesService sps = DependencyService.Get<ISharedPreferencesService> ();
				string savedLanguage = sps.getStringPref ("Language");

				if (savedLanguage != null) {
					//fill this so we won't need to keep retrieving it.
					selectedLanguage = savedLanguage;
				}
			}

			return selectedLanguage;
		}


		public void SetPreferedLanguage(string languageCode){

			selectedLanguage = languageCode;

			ISharedPreferencesService sps = DependencyService.Get<ISharedPreferencesService>();
			sps.putStringPref ("Language", languageCode);
		}


		public bool HasAUser(){

			bool hasUser = false;
		
			ISharedPreferencesService sps = DependencyService.Get<ISharedPreferencesService>();
			string JSONString = sps.getStringPref ("User");

			if (JSONString != null) {
				hasUser = true;
			}

			return hasUser;
		}


		public void SaveUser(){

			string JSONString = JsonConvert.SerializeObject (currentUser);

			ISharedPreferencesService sps = DependencyService.Get<ISharedPreferencesService>();
			sps.putStringPref ("User", JSONString);

		}


		private void RetrieveSavedUser (){
			ISharedPreferencesService sps = DependencyService.Get<ISharedPreferencesService>();
			string JSONString = sps.getStringPref ("User");

			if (JSONString != null) {
				currentUser = JsonConvert.DeserializeObject<User> (JSONString);
			}
		}


		public void CheckUserHyperlocalStatus (Action<bool, string> receiver){

			WebServiceRequestWithReturnFunctionAndVariable wsmgr = new WebServiceRequestWithReturnFunctionAndVariable();
			wsmgr.returnFunction = receiver;

			//the web services manager needs a user object to create the request URL.
			wsmgr.user = currentUser;

			wsmgr.GetRequest (WebServicesMgr.WSCall.GetUserProfile, CheckUserHyperlocalStatusReturned);
		}
		private void CheckUserHyperlocalStatusReturned(bool success, string message, string JSON, Object wsmgr){

			WebServiceRequestWithReturnFunctionAndVariable webServicesMgr = (WebServiceRequestWithReturnFunctionAndVariable)wsmgr;

			if(success){

				message = "User profile retrieved successfully";
				currentUser = (User)JsonConvert.DeserializeObject<User> (JSON);
				SaveUser ();

			}

			if(webServicesMgr.returnFunction != null){
				webServicesMgr.returnFunction (success, message);
			}
		}
			
	}


	public interface ISharedPreferencesService
	{
		string getStringPref(string key);
		void putStringPref(string key, string value);
	}
}

