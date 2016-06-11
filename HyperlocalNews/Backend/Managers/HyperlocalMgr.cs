using System;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace HyperlocalNews
{
	public class HyperlocalMgr
	{

		/* singleton manager */
		private static HyperlocalMgr instance;

		private HyperlocalMgr () {}

		public static HyperlocalMgr getInstance(){

			if(instance == null){
				instance = new HyperlocalMgr();
			}
			return instance;

		}

		//This is unlike the lists held by ReportsMgr, as hyperlocals could be requested based on locations etc. so there are many possibilities, and as a result,
		//this class will not hold a shared collection, but will return a new one for each request.
		public ObservableCollection<Hyperlocal> GetHyperlocals(Action<bool, string> receiver){

			WebServiceRequestWithReturnFunctionAndVariable wsmgr = new WebServiceRequestWithReturnFunctionAndVariable();
			wsmgr.returnFunction = receiver;
			wsmgr.returnObject = new ObservableCollection<Hyperlocal> ();

			wsmgr.GetRequest (WebServicesMgr.WSCall.GetHyperlocals, GetHyperlocalsReturned);

			return (ObservableCollection<Hyperlocal>)wsmgr.returnObject;
		}

		private void GetHyperlocalsReturned(bool success, string message, string JSON, Object wsmgr){

			WebServiceRequestWithReturnFunctionAndVariable webServicesMgr = (WebServiceRequestWithReturnFunctionAndVariable)wsmgr;
			ObservableCollection<Hyperlocal> hyperlocals = (ObservableCollection<Hyperlocal>)webServicesMgr.returnObject;


			if(success){

				Collection<Hyperlocal> hypers = JsonConvert.DeserializeObject<Collection<Hyperlocal>> (JSON);
				foreach(Hyperlocal h in hypers){
					hyperlocals.Add (h);
				}
			}

			if(webServicesMgr.returnFunction != null){
				webServicesMgr.returnFunction (success, message);
			}
		}





		public void VerifyNewHyperlocalAdministratorRequest(Action<bool, string> receiver, string userId, Hyperlocal hyperlocal){

			WebServiceRequestWithReturnFunctionAndVariable wsmgr = new WebServiceRequestWithReturnFunctionAndVariable();
			wsmgr.returnFunction = receiver;
			wsmgr.user = UserMgr.getInstance ().GetUserObject ();

			//the web services manager needs a hyperlocal object to create the request URL.
			wsmgr.hyperlocal = hyperlocal;

			wsmgr.GetRequest (WebServicesMgr.WSCall.RequestHyperlocalVerification, VerifyNewHyperlocalAdministratorRequestReturned);
		}
		private void VerifyNewHyperlocalAdministratorRequestReturned(bool success, string message, string JSON, Object wsmgr){

			WebServiceRequestWithReturnFunctionAndVariable webServicesMgr = (WebServiceRequestWithReturnFunctionAndVariable)wsmgr;

			if(success){
				message = "Hyperlocal administration request submitted successfully";
			}

			if(webServicesMgr.returnFunction != null){
				webServicesMgr.returnFunction (success, message);
			}
		}

		public void RegisterNewHyperlocal (Action<bool, string> receiver, string name, string email, double lat, double lon, string userId){

			Hyperlocal newHyperlocal = new Hyperlocal ();
			newHyperlocal.Name = name;
			newHyperlocal.AdminEmail = email;
			newHyperlocal.LocationLatitude = lat.ToString();
			newHyperlocal.LocationLongitude = lon.ToString ();

			string requestString = JsonConvert.SerializeObject (newHyperlocal);

			WebServiceRequestWithReturnFunctionAndVariable wsmgr = new WebServiceRequestWithReturnFunctionAndVariable();
			wsmgr.returnFunction = receiver;
			wsmgr.user = UserMgr.getInstance ().GetUserObject ();

			wsmgr.PostRequest (WebServicesMgr.WSCall.CreateNewHyperlocal, requestString, RegisterNewHyperlocalReturned);
		}
		private void RegisterNewHyperlocalReturned(bool success, string message, string JSON, Object wsmgr){

			WebServiceRequestWithReturnFunctionAndVariable webServicesMgr = (WebServiceRequestWithReturnFunctionAndVariable)wsmgr;

			if(success){
				message = "Hyperlocal registration submitted successfully";
				//We won't save this because it will need to be approved;
			}

			if(webServicesMgr.returnFunction != null){
				webServicesMgr.returnFunction (success, message);
			}
		}
	}
}

