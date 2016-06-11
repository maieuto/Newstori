using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Xamarin.Forms;

namespace HyperlocalNews
{
	public class ReportMgr : DraftStorageMgr
	{

		/* singleton manager */
		private static ReportMgr instance;

		private ReportMgr () {}

		public static ReportMgr getInstance(){

			if(instance == null){
				instance = new ReportMgr();
			}
			return instance;

		}
			
		ObservableCollection<Report> _ReportsWithFeedback = new ObservableCollection<Report> ();

		/* 	Reports with Feedback
			These belong to standard reporter users. Contains reports the user has submitted that have received feedback.
		 */

		public ObservableCollection<Report> GetReportsWithFeedback(Action<bool, string> receiver){

			WebServiceRequestWithReturnFunctionAndVariable wsmgr = new WebServiceRequestWithReturnFunctionAndVariable();
			wsmgr.returnFunction = receiver;

			//the web services manager needs a user object to create the request URL.
			wsmgr.user = UserMgr.getInstance().GetUserObject();

			wsmgr.GetRequest (WebServicesMgr.WSCall.GetFeedbackReports, ReportsWithFeedbackReturned);

			return _ReportsWithFeedback;
		}

		private void ReportsWithFeedbackReturned(bool Success, string message, string JSONString, Object wsmgr){
			//JSON was returned.

			WebServiceRequestWithReturnFunctionAndVariable webServicesMgr = (WebServiceRequestWithReturnFunctionAndVariable)wsmgr;

			if(Success){
				message = "Reports with feedback received successfully";
				Collection<Report> receivedReports = JsonConvert.DeserializeObject<Collection<Report>> (JSONString);
				_ReportsWithFeedback.Clear();
				if (receivedReports != null) {
					foreach (Report r in receivedReports) {
						_ReportsWithFeedback.Add (r);
					}
				}
			}

			if(webServicesMgr.returnFunction != null){
				webServicesMgr.returnFunction (Success, message);
			}
		}



		/* 	Reports Submitted
			These belong to standard reporter users. These are the reports the user has submitted that have not received unanswered feedback.
		 */

		ObservableCollection<Report> _ReportsSubmitted = new ObservableCollection<Report> ();

		public ObservableCollection<Report> GetReportsSubmitted(Action<bool, string> receiver){

			WebServiceRequestWithReturnFunctionAndVariable wsmgr = new WebServiceRequestWithReturnFunctionAndVariable();
			wsmgr.returnFunction = receiver;

			//the web services manager needs a user object to create the request URL.
			wsmgr.user = UserMgr.getInstance().GetUserObject();

			wsmgr.GetRequest (WebServicesMgr.WSCall.GetSubmittedReports, SubmittedReportsReturned);

			return _ReportsSubmitted;
		}

		private void SubmittedReportsReturned(bool Success, string message, string JSONString, Object wsmgr){
			//JSON was returned.
			WebServiceRequestWithReturnFunctionAndVariable webServicesMgr = (WebServiceRequestWithReturnFunctionAndVariable)wsmgr;

			if(Success){
				message = "Submitted reports received successfully";
				Collection<Report> receivedReports = JsonConvert.DeserializeObject<Collection<Report>> (JSONString);
				_ReportsSubmitted.Clear();
				if(receivedReports != null){
					foreach(Report r in receivedReports){
						_ReportsSubmitted.Add (r);
					}
				}
			}

			if(webServicesMgr.returnFunction != null){
				webServicesMgr.returnFunction (Success, message);
			}
		}





		/* 	Submit Feedback Response
			This method is for the standard reporter user, when responding to admin feedback
		*/

		public void SendFeedbackResponse(Action<bool, string> receiver, Report r){

			WebServiceRequestWithReturnFunctionAndVariable wsmgr = new WebServiceRequestWithReturnFunctionAndVariable();
			wsmgr.returnFunction = receiver;
			wsmgr.returnObject = r;

			//the web services manager needs a report object to create the request URL.
			wsmgr.report = r;

			string requestString = JsonConvert.SerializeObject(r.Feedback);

			wsmgr.PostRequest (WebServicesMgr.WSCall.SubmitReportFeedbackResponse, requestString, SendFeedbackResponseReturned);
		}

		private void SendFeedbackResponseReturned(bool Success, string message, string JSONString, Object wsmgr){

			WebServiceRequestWithReturnFunctionAndVariable webServicesMgr = (WebServiceRequestWithReturnFunctionAndVariable)wsmgr;

			//success? so remove item from feedback list now.

			if (Success) {
				message = "Feeback response sent";
				_ReportsWithFeedback.Remove ((Report)webServicesMgr.returnObject);
			}

			if(webServicesMgr.returnFunction != null){
				webServicesMgr.returnFunction (Success, message);
			}
		}




		/* 	Submit Feedback 
			This method is for the Administrative  user, when posting feedback
		 */

		public void SendFeedback(Action<bool, string> receiver, Report r){

			WebServiceRequestWithReturnFunctionAndVariable wsmgr = new WebServiceRequestWithReturnFunctionAndVariable();
			wsmgr.returnFunction = receiver;

			//the web services manager needs a report object to create the request URL.
			wsmgr.report = r;

			string requestString = JsonConvert.SerializeObject(r.Feedback);

			wsmgr.PostRequest (WebServicesMgr.WSCall.SubmitReportFeedback, requestString, SendFeedbackReturned);
		}

		private void SendFeedbackReturned(bool Success, string message, string JSONString, Object wsmgr){

			WebServiceRequestWithReturnFunctionAndVariable webServicesMgr = (WebServiceRequestWithReturnFunctionAndVariable)wsmgr;

			if(Success){
				message = "Feedback sent";
			}

			if(webServicesMgr.returnFunction != null){
				webServicesMgr.returnFunction (Success, message);
			}
		}



		/* 	Submit Report 
			This method is for the standard reporter user, when submitting a report
		 */

		public void SubmitReport(Action<bool, string> receiver, Report r, List<string> images){

			WebServiceRequestWithReturnFunctionAndVariable wsmgr = new WebServiceRequestWithReturnFunctionAndVariable();
			wsmgr.returnFunction = receiver;

			string requestString = JsonConvert.SerializeObject (r);

			if(r.HyperLocal != null){
				JObject o = JObject.Parse (requestString);
				o.Remove ("HyperLocal");
				o.Add ("HyperLocalID", r.HyperLocal.ID);
				requestString = o.ToString ();
			}

			r.LocalPhotos = images;
			wsmgr.returnObject = r;

			wsmgr.MultipartPostRequest (WebServicesMgr.WSCall.SubmitNewReport, requestString, images, SubmitReportReturned);
		}

		private void SubmitReportReturned(bool success, string message, string JSON, Object wsmgr){

			WebServiceRequestWithReturnFunctionAndVariable webServicesMgr = (WebServiceRequestWithReturnFunctionAndVariable)wsmgr;

			//success? so remove item from feedback list now.
			//I don't like the idea of adding to things. The server may alter things or add things, etc. 
			//So even though we remove item from draft for immediate user visual feedback, we will wait for the "submitted" list to be updated normally.
			if (success) {
				Report r = (Report)webServicesMgr.returnObject;

				DeleteLocalFiles (r.LocalPhotos);

				message = "Draft report submitted";
				RemoveDraftReport (null, (Report)webServicesMgr.returnObject);
			}

			if(webServicesMgr.returnFunction != null){
				webServicesMgr.returnFunction (success, message);
			}
		}

		public void DeleteLocalFiles(List<string> images){

			IGalleryImageService galleryService = DependencyService.Get<IGalleryImageService>();

			foreach(string i in images){
				galleryService.DeleteFile (i);
			}
		}


		/* 	Submit Report 
			This method is for the standard reporter user, when submitting a report
		 */

		public void SubmitReportUpdate(Action<bool, string> receiver, Report r, List<string> images){

			WebServiceRequestWithReturnFunctionAndVariable wsmgr = new WebServiceRequestWithReturnFunctionAndVariable();
			wsmgr.returnFunction = receiver;
			wsmgr.report = r;

			string requestString = JsonConvert.SerializeObject (r);

			if(r.HyperLocal != null){
				JObject o = JObject.Parse (requestString);
				o.Remove ("HyperLocal");
				o.Add ("HyperLocalID", r.HyperLocal.ID);
				requestString = o.ToString ();
			}

			r.LocalPhotos = images;
			wsmgr.returnObject = r;

			wsmgr.MultipartPostRequest (WebServicesMgr.WSCall.UpdateReport, requestString, images, SubmitReportReturned);
		}

		private void SubmitReportUpdateReturned(bool success, string message, string JSON, Object wsmgr){

			WebServiceRequestWithReturnFunctionAndVariable webServicesMgr = (WebServiceRequestWithReturnFunctionAndVariable)wsmgr;

			if (success) {
				Report r = (Report)webServicesMgr.returnObject;

				IGalleryImageService galleryService = DependencyService.Get<IGalleryImageService>();

				foreach(string i in r.LocalPhotos){
					galleryService.DeleteFile (i);
				}

				message = "Report Updated";
			}

			if(webServicesMgr.returnFunction != null){
				webServicesMgr.returnFunction (success, message);
			}
		}


		public void ExportReport(Action<bool, string> receiver, Report report){

			WebServiceRequestWithReturnFunctionAndVariable wsmgr = new WebServiceRequestWithReturnFunctionAndVariable();
			wsmgr.returnFunction = receiver;
			wsmgr.returnObject = report;

			//the web services manager needs a report object to create the request URL.
			wsmgr.report = report;

			wsmgr.GetRequest (WebServicesMgr.WSCall.RequestArticleExport, ExportReportReturned);

		}

		private void ExportReportReturned(bool success, string message, string JSON, Object wsmgr){

			WebServiceRequestWithReturnFunctionAndVariable webServicesMgr = (WebServiceRequestWithReturnFunctionAndVariable)wsmgr;

			if (success) {
				message = "Report Exported";
			}

			if(webServicesMgr.returnFunction != null){
				webServicesMgr.returnFunction (success, message);
			}
		}


		public void ClaimReport(Action<bool, string> receiver, Report report){

			WebServiceRequestWithReturnFunctionAndVariable wsmgr = new WebServiceRequestWithReturnFunctionAndVariable();
			wsmgr.returnFunction = receiver;
			wsmgr.returnObject = report;

			//the web services manager needs a report object to create the request URL.
			wsmgr.report = report;

			wsmgr.GetRequest (WebServicesMgr.WSCall.ClaimReport, ClaimReportReturned);

		}

		private void ClaimReportReturned(bool success, string message, string JSON, Object wsmgr){

			WebServiceRequestWithReturnFunctionAndVariable webServicesMgr = (WebServiceRequestWithReturnFunctionAndVariable)wsmgr;

			if (success) {
				message = "Report Claimed";
			}

			if(webServicesMgr.returnFunction != null){
				webServicesMgr.returnFunction (success, message);
			}
		}


		public void UnclaimReport(Action<bool, string> receiver, Report report){

			WebServiceRequestWithReturnFunctionAndVariable wsmgr = new WebServiceRequestWithReturnFunctionAndVariable();
			wsmgr.returnFunction = receiver;
			wsmgr.returnObject = report;

			//the web services manager needs a report object to create the request URL.
			wsmgr.report = report;

			wsmgr.GetRequest (WebServicesMgr.WSCall.UnclaimReport, UnclaimReportReturned);

		}

		private void UnclaimReportReturned(bool success, string message, string JSON, Object wsmgr){

			WebServiceRequestWithReturnFunctionAndVariable webServicesMgr = (WebServiceRequestWithReturnFunctionAndVariable)wsmgr;

			if (success) {
				message = "Report Unclaimed";
			}

			if(webServicesMgr.returnFunction != null){
				webServicesMgr.returnFunction (success, message);
			}
		}

		public void DeclineReport(Action<bool, string> receiver, Report report){

			WebServiceRequestWithReturnFunctionAndVariable wsmgr = new WebServiceRequestWithReturnFunctionAndVariable();
			wsmgr.returnFunction = receiver;
			wsmgr.returnObject = report;

			//the web services manager needs a report object to create the request URL.
			wsmgr.report = report;

			wsmgr.GetRequest (WebServicesMgr.WSCall.DeclineReport, DeclineReportReturned);

		}

		private void DeclineReportReturned(bool success, string message, string JSON, Object wsmgr){

			WebServiceRequestWithReturnFunctionAndVariable webServicesMgr = (WebServiceRequestWithReturnFunctionAndVariable)wsmgr;

			if (success) {
				message = "Report Declined";
			}

			if(webServicesMgr.returnFunction != null){
				webServicesMgr.returnFunction (success, message);
			}
		}

		public void UndeclineReport(Action<bool, string> receiver, Report report){

			WebServiceRequestWithReturnFunctionAndVariable wsmgr = new WebServiceRequestWithReturnFunctionAndVariable();
			wsmgr.returnFunction = receiver;
			wsmgr.returnObject = report;

			//the web services manager needs a report object to create the request URL.
			wsmgr.report = report;

			wsmgr.GetRequest (WebServicesMgr.WSCall.UndeclineReport, UndeclineReportReturned);

		}

		private void UndeclineReportReturned(bool success, string message, string JSON, Object wsmgr){

			WebServiceRequestWithReturnFunctionAndVariable webServicesMgr = (WebServiceRequestWithReturnFunctionAndVariable)wsmgr;

			if (success) {
				message = "Report Undeclined";
			}

			if(webServicesMgr.returnFunction != null){
				webServicesMgr.returnFunction (success, message);
			}
		}




		/*	Get Reports for Hyperlocal
		 * 	This method is for hyperlocal administrators to access a list of submitted reports.
		 */
		ObservableCollection<Report> _ReportsForHyperlocal = new ObservableCollection<Report> ();

		public ObservableCollection<Report> GetReportsForHyperlocal(Action<bool, string> receiver){

			WebServiceRequestWithReturnFunctionAndVariable wsmgr = new WebServiceRequestWithReturnFunctionAndVariable();
			wsmgr.returnFunction = receiver;

			//the web services manager needs a user object to create the request URL.
			wsmgr.user = UserMgr.getInstance().GetUserObject();

			wsmgr.GetRequest (WebServicesMgr.WSCall.GetHyperlocalReports, ReportsForHyperlocalReturned);

			return _ReportsForHyperlocal;
		}

		public void ReloadReportsForHyperlocal(){

			WebServiceRequestWithReturnFunctionAndVariable wsmgr = new WebServiceRequestWithReturnFunctionAndVariable();

			//the web services manager needs a user object to create the request URL.
			wsmgr.user = UserMgr.getInstance().GetUserObject();

			wsmgr.GetRequest (WebServicesMgr.WSCall.GetHyperlocalReports, ReportsForHyperlocalReturned);

		}

		private void FakeResponse(string json, Object wsmgr){

			WebServiceRequestWithReturnFunctionAndVariable webServicesMgr = (WebServiceRequestWithReturnFunctionAndVariable)wsmgr;

			_ReportsForHyperlocal.Add (new Report{Title="Report Title Here.", Content="This is a content sentence. This is a content sentence. This is a content sentence. This is a content sentence. This is a content sentence. This is a content sentence. This is a content sentence. This is a content sentence. This is a content sentence. This is a content sentence. This is a content sentence."});
			_ReportsForHyperlocal.Add (new Report{Title="Report Title Here.", Content="This is a content sentence."});
			_ReportsForHyperlocal.Add (new Report{Title="Report Title Here.", Content="This is a content sentence."});
			_ReportsForHyperlocal.Add (new Report{Title="Report Title Here.", Content="This is a content sentence. This is a content sentence. This is a content sentence. This is a content sentence. This is a content sentence. This is a content sentence. This is a content sentence. This is a content sentence. This is a content sentence. This is a content sentence. This is a content sentence."});


			if(webServicesMgr.returnFunction != null){
				webServicesMgr.returnFunction (true, "success");
			}
		}

		private void ReportsForHyperlocalReturned(bool Success, string message, string JSONString, Object wsmgr){
			//JSON was returned.

			WebServiceRequestWithReturnFunctionAndVariable webServicesMgr = (WebServiceRequestWithReturnFunctionAndVariable)wsmgr;

			if(Success){
				message = "Reports for hyperlocal received successfully";
				Collection<Report> receivedReports = JsonConvert.DeserializeObject<Collection<Report>> (JSONString);
				_ReportsForHyperlocal.Clear();
				if(receivedReports != null){
					foreach(Report r in receivedReports){
						_ReportsForHyperlocal.Add (r);
					}
				}
			}

			if(webServicesMgr.returnFunction != null){
				webServicesMgr.returnFunction (Success, message);
			}
		}

	}
}

