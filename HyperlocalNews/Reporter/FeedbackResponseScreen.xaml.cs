using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace HyperlocalNews
{
	public partial class FeedbackResponseScreen : ContentPage
	{
		private Report r;

		//these are used for adding additional content to the report with feedback.
		private List<string> additionalImages = new List<string> ();
		private List<Image> additionalImageViews = new List<Image>();
		private double lat;
		private double lon;
		private DateTime newDate;

		public FeedbackResponseScreen (Report report)
		{
			InitializeComponent ();

			r = report;

			Title = report.Title;

			Icon = "user.png";

			if(r.Feedback != null && r.Feedback.Message != null){
				this.FindByName<StackLayout> ("FeedbackSection").IsVisible = true;

				this.ToolbarItems.Add(
					new ToolbarItem(TranslateExtension.Localize("SendLabel"), null, () =>{
						SendResponse();
					}
					)
				);
			}

			(this.FindByName <StackLayout>("layout")).BindingContext = r;

			if (r.Feedback.RequestPhoto == 1) {
				Button b = (this.FindByName <Button> ("PhotosButton"));
				b.BackgroundColor = BrandColors.MediumGreen;
				b.TextColor = Color.White;
			}
			if (r.Feedback.RequestDate == 1) {
				Button b = (this.FindByName <Button> ("DateButton"));
				b.BackgroundColor = BrandColors.MediumGreen;
				b.TextColor = Color.White;
			}
			if (r.Feedback.RequestLocation == 1) {
				Button b = (this.FindByName <Button> ("LocationButton"));
				b.BackgroundColor = BrandColors.MediumGreen;
				b.TextColor = Color.White;
			}

			if(r.Photos.Count > 0){
				foreach(Photo ph in r.Photos){
					AddRemoteImageToView (ph.PublicUrl);
				}
			}

			lat = Double.Parse (report.LocationLatitude);
			lon = Double.Parse (report.LocationLongitude);
		}

		private void SendResponse (object sender, EventArgs args){

			SendResponse ();
		}

		private void SendResponse (){

			r.LocationLatitude = lat.ToString ();
			r.LocationLongitude = lon.ToString ();
			//no need to set date as it is bound to the report.

			ReportMgr.getInstance ().SubmitReportUpdate (UpdateReportReturned, r, additionalImages);

			this.FindByName<StackLayout> ("ActivityView").IsVisible = true;
		}

		private void UpdateReportReturned (bool success, string message){

			if (success) {

				r.Feedback.ResponseMessage = (this.FindByName <Editor> ("FeedbackResponse")).Text;
				ReportMgr.getInstance ().SendFeedbackResponse (receivedSubmissionResponse, r);

			} else {
				DisplayAlert (TranslateExtension.Localize("FeedbackResponseFailedTitle"), TranslateExtension.Localize("FeedbackResponseFailedLabel")+message, TranslateExtension.Localize("OKLabel"));
			}
		}


		private void receivedSubmissionResponse(bool success, string message){

			this.FindByName<StackLayout> ("ActivityView").IsVisible = false;

			if(success){
				App.navigation.PopAsync ();
				DisplayAlert (TranslateExtension.Localize("FeedbackResponseSentTitle"), TranslateExtension.Localize("FeedbackResponseSentLabel"), TranslateExtension.Localize("OKLabel"));
			}else{
				DisplayAlert (TranslateExtension.Localize("FeedbackResponseFailedTitle"), TranslateExtension.Localize("FeedbackResponseFailedLabel")+message, TranslateExtension.Localize("OKLabel"));
			}
		}


		private void AddRemoteImageToView(String imgSrcUri){

			Image img = new Image ();

			img.Source = imgSrcUri;

			this.FindByName<StackLayout> ("ImageHolder").Children.Add (img);
			this.FindByName<StackLayout> ("ImageHolder").IsVisible = true;

		}


		private void AddLocalImageToView(String imgSrcUri){

			//these are added additionaly and submitted along with feedback.
			Image img = new Image ();

			IGalleryImageService galleryService = DependencyService.Get<IGalleryImageService>();
			img.Source = ImageSource.FromStream(() => galleryService.GetThumb(imgSrcUri));

			this.FindByName<StackLayout> ("AdditionalImageHolder").Children.Add (img);
			this.FindByName<StackLayout> ("AdditionalImageHolder").IsVisible = true;

		}

		private void ViewLocation(object sender, EventArgs args){
			ViewLocation ();	
		}

		private void ViewLocation(){
			LocationPage lp = new LocationPage (null, Double.Parse(r.LocationLatitude), Double.Parse(r.LocationLongitude), r.Title, true, false);
			App.navigation.PushAsync (lp);
		}


		private void AttachPhoto(object senderObj, EventArgs eventArgs){

			IGalleryImageService galleryService = DependencyService.Get<IGalleryImageService>();

			galleryService.LaunchImageSelector (gotUserImage);

		}

		private void gotUserImage(String selectedImgSrc){
			if(selectedImgSrc != null){

				//don't add it to the object yet. We don't want it committed to drafts until confirmed. Hold it in images for now.
				additionalImages.Add (selectedImgSrc);

				AddLocalImageToView (selectedImgSrc);

				this.FindByName <Button>("AttachNewPhoto").BackgroundColor = BrandColors.MediumGreen;
				this.FindByName <Button>("AttachNewPhoto").TextColor = Color.White;
			}
		}

		private void EditLocation(object senderObj, EventArgs eventArgs){

			double latitudeForLaunch = 51.481581;
			double longitudeForLaunch = -3.179090;

			if(lat != 0 && lon != 0){
				latitudeForLaunch = lat;
				longitudeForLaunch = lon;
			}

			LocationPage lp = new LocationPage (LocationReturned, latitudeForLaunch, longitudeForLaunch, TranslateExtension.Localize("LocationLabel"), false, false);

			App.navigation.PushAsync (lp);

		}

		private void LocationReturned(bool success, double newLat, double newLon){
			if(success){
				lat = newLat;
				lon = newLon;

				App.navigation.PopAsync ();

				this.FindByName <Button>("ChangeLocation").BackgroundColor = BrandColors.MediumGreen;
				this.FindByName <Button>("ChangeLocation").TextColor = Color.White;
			}
		}


	}
}