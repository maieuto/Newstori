using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Xamarin.Forms;
using System.Net.Http.Headers;


namespace HyperlocalNews
{
	public class WebServicesMgr
	{

		public enum WSCall
		{
			GetUserProfile, UpdateUserProfile, CreateNewUser, RequestUserVerification, GetUserProfileFromToken,
			GetSubmittedReports, GetFeedbackReports, SubmitNewReport, SubmitReportFeedback, SubmitReportFeedbackResponse, GetHyperlocalReports, UpdateReport,
			GetHyperlocals, CreateNewHyperlocal, RequestHyperlocalVerification,
			ClaimReport, UnclaimReport, RequestArticleExport, DeclineReport, UndeclineReport
		}

		public WebServicesMgr () {
			user = UserMgr.getInstance ().GetUserObject ();
		}

		public User user;
		public Report report;
		public Hyperlocal hyperlocal;
		public string singleUseToken;

		private static string URL = "http://scoopifi.eu1.frbit.net/";

		private string AddressForRequest(WSCall typeOfCall){

			string address = "api/v1/";

			if(typeOfCall == WSCall.GetUserProfile){
				address += "users/"+user.ID;

			}else if(typeOfCall == WSCall.UpdateUserProfile){
				address += "users/"+user.ID+"/update";

			}else if(typeOfCall == WSCall.CreateNewUser){
				address += "users";

			}else if(typeOfCall == WSCall.RequestUserVerification){
				address += "users/verify";

			}else if(typeOfCall == WSCall.GetSubmittedReports){
				//get all reports and their feedback, etc.
				address += "users/"+user.ID+"/reports/withfeedback";

			}else if(typeOfCall == WSCall.GetFeedbackReports){
				//get only the feedback I haven't replied to.
				address += "users/"+user.ID+"/reports/withopenfeedback";

			}else if(typeOfCall == WSCall.SubmitNewReport){
				address += "reports";

			}else if(typeOfCall == WSCall.UpdateReport){
				address += "reports/"+report.ID;

			}else if(typeOfCall == WSCall.SubmitReportFeedback){
				address += "reports/"+report.ID+"/feedback";

			}else if(typeOfCall == WSCall.SubmitReportFeedbackResponse){
				//In future this should be a seperate method that only taked the response string, not a whole object. To stop editing by wrong user.
				address += "reports/"+report.ID+"/feedback";

			}else if(typeOfCall == WSCall.GetHyperlocalReports){
				address += "reports/inmyarea";

			}else if(typeOfCall == WSCall.GetHyperlocals){
				address += "hyperlocals";

			}else if(typeOfCall == WSCall.CreateNewHyperlocal){
				address += "hyperlocals";

			}else if(typeOfCall == WSCall.RequestHyperlocalVerification){
				address += "hyperlocals/"+hyperlocal.ID+"/join";

			}else if(typeOfCall == WSCall.GetUserProfileFromToken){
				address += "users/verify/"+singleUseToken;

			}else if(typeOfCall == WSCall.RequestArticleExport){
				address += "reports/"+report.ID+"/export";

			}else if(typeOfCall == WSCall.ClaimReport){
				address += "reports/"+report.ID+"/claimby/"+user.ID;

			}else if(typeOfCall == WSCall.UnclaimReport){
				address += "reports/"+report.ID+"/resetclaim";

			}else if(typeOfCall == WSCall.DeclineReport){
				address += "reports/"+report.ID+"/decline";

			}else if(typeOfCall == WSCall.UndeclineReport){
				address += "reports/"+report.ID+"/undecline";
			}

			if(user != null){
				address += "?token="+user.Token;
				address += "&Language=" + user.Language;
			}

			return address;
		}



		public delegate void RequestDelegate(bool Success, string message, string JSONString, Object wsmgr);

		public async void MultipartPostRequest(WSCall typeOfCall, string requestString, List<string> files, RequestDelegate del){

			try{
				HttpClient client = new HttpClient ();

				client.BaseAddress = new Uri(URL);

				MultipartFormDataContent mpfd = new MultipartFormDataContent("----xxxx----");

				int imageCount = 0;

				foreach(string f in files){

					IGalleryImageService galleryService = DependencyService.Get<IGalleryImageService>();

					StreamContent sc = new StreamContent(galleryService.GetFileStream(f));

					sc.Headers.ContentType =   new MediaTypeHeaderValue("image/*");

					imageCount ++;
					mpfd.Add(sc, "Image" + imageCount, "Photos.jpg");

				}

				mpfd.Add(new StringContent(requestString, System.Text.Encoding.UTF8, "application/json"), "data");
				mpfd.Add(new StringContent(imageCount.ToString()), "ImageCount");

				string address = AddressForRequest (typeOfCall);
				HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, address);
				req.Content = mpfd;


				HttpResponseMessage response = await client.SendAsync (req);

				string responseJSON = response.Content.ReadAsStringAsync().Result;

				JObject o = JObject.Parse (responseJSON);

				string data="";
				string err="";
				bool success = response.IsSuccessStatusCode;

				if (success) {
					JObject obj = ((JObject)o ["data"]);
					if(obj != null){
						data = obj.ToString ();
					}

				} else {
					err = (string)o["error"]["message"];

				}

				del (success, err, data, this);

			}catch(Exception e){

				del (false, TranslateExtension.Localize("ConnectionErrorLabel"), "", this);
			}
		}  


		public async void PostRequest(WSCall typeOfCall, string requestString, RequestDelegate del){

			try{
				HttpClient client = new HttpClient ();
				client.DefaultRequestHeaders.Accept.Add (new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

				client.BaseAddress = new Uri(URL);
				client.Timeout = TimeSpan.FromSeconds(10);

				string address = AddressForRequest (typeOfCall);
				HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, address);
				req.Content = new StringContent(requestString, System.Text.Encoding.UTF8, "application/json");


				HttpResponseMessage response = await client.SendAsync (req);

				string responseJSON = response.Content.ReadAsStringAsync().Result;

				JObject o = JObject.Parse (responseJSON);

				string data="";
				string err="";
				bool success = response.IsSuccessStatusCode;

				if (success) {
					JObject obj = ((JObject)o ["data"]);
					if(obj != null){
						data = obj.ToString ();
					}

				} else {
					err = (string)o["error"]["message"];

				}
				del (success, err, data, this);
			}catch(Exception e){

				del (false, TranslateExtension.Localize("ConnectionErrorLabel"), "", this);
			}
		}


		public async void GetRequest(WSCall typeOfCall, RequestDelegate del){
			try{
				HttpClient client = new HttpClient ();
				client.DefaultRequestHeaders.Accept.Add (new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

				client.BaseAddress = new Uri(URL);
				client.Timeout = TimeSpan.FromSeconds(10);

				string address = AddressForRequest(typeOfCall);
				HttpResponseMessage response = await client.GetAsync(address);

				string responseJSON = response.Content.ReadAsStringAsync().Result;

				JObject o = JObject.Parse (responseJSON);

				string data="";
				string err="";
				bool success = response.IsSuccessStatusCode;

				if (success) {
					if(o["data"] != null){
						data = (o["data"]).ToString();
					}

				} else {
					err = (string)o["error"]["message"];

					//server can not return correct language for this request.
					if(typeOfCall == WSCall.RequestUserVerification){
						err = TranslateExtension.Localize("UserLoginFailReason");
					}
				}

				del (success, err, data, this);
			}catch(Exception e){

				del (false, TranslateExtension.Localize("ConnectionErrorLabel"), "", this);
			}
		}


		// Declare a delegate type for a fake request: 
		public delegate void FakeWebRequestDelegate(string JSONString, Object wsmgr);

		//this just returns bool as it is fake. Doesn't need to return an array
		public async void fakeWebRequest(WSCall requestType, FakeWebRequestDelegate del){
		
			Task<string> theAsyncTask = waitForJSONResponse ();
						string JSON = await theAsyncTask;

			del(JSON, this);
		}

		private async Task<string> waitForJSONResponse(){
			await Task.Delay (3000);

			return "Hello";
		}

	}


	//Used as a wrapper by other classes to pass a function and object onto its result method
	public class WebServiceRequestWithReturnFunctionAndVariable : WebServicesMgr {

		public Action<bool, string> returnFunction;
		public Object returnObject;

	}
}

