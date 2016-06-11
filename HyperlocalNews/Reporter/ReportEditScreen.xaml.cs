using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

using Xamarin.Forms;

namespace HyperlocalNews
{
	public partial class ReportEditScreen : BackButtonOverridePage
	{
		private Report r;
		private bool isCreatingNew = true;
		private List<string> images = new List<string> ();
		private List<Image> imageViews = new List<Image>();
		private double lat;
		private double lon;
		private bool hasLocation = false;
		private bool hasTargetHyperlocal = false;

		private bool isWorking;

		public ReportEditScreen ()
		{
			r = new Report("","");

			StandardSetup ();
		}

		public ReportEditScreen (Report reportToEdit)
		{
			r = reportToEdit;

			isCreatingNew = false;

			StandardSetup ();


			(this.FindByName <Entry>("TitleBox")).Text = r.Title;
			(this.FindByName <Editor>("ContentBox")).Text = r.Content;
			(this.FindByName <DatePicker>("DatePicker")).Date = r.Date;

			if(r.LocationLatitude != null){
				hasLocation = true;
				lat = Double.Parse (r.LocationLatitude);
				lon = Double.Parse (r.LocationLongitude);

				(this.FindByName <Button>("GetTheLocation")).BackgroundColor = BrandColors.MediumGreen;
				(this.FindByName <Button>("GetTheLocation")).TextColor = Color.White;
			}

			if(r.Photos.Count > 0){
				foreach(Photo ph in r.Photos){
					//AddLocalImageToView (imgSrc);
					AddRemoteImageToView (ph.PublicUrl);
				}
				(this.FindByName <Button>("GetTheImage")).BackgroundColor = BrandColors.MediumGreen;
				(this.FindByName <Button>("GetTheImage")).TextColor = Color.White;
			}


			/* Set up a holder for the list of local(unsubmitted) images, any changes we make go here until
			 * we save or submit, so that we can discard changes without affecting the original list.
			*/
			images = new List<string> ();

			foreach(string i in r.LocalPhotos){
				images.Add (i);
			}

			if(r.LocalPhotos.Count > 0){
				foreach(string ph in images){
					//AddLocalImageToView (imgSrc);
					AddLocalImageToView (ph);
				}
				(this.FindByName <Button>("GetTheImage")).BackgroundColor = BrandColors.MediumGreen;
				(this.FindByName <Button>("GetTheImage")).TextColor = Color.White;
			}

		}

		public void StandardSetup(){

			InitializeComponent ();

			Title = TranslateExtension.Localize("ReportLabel");

			Icon = "user.png";


			/*  we use renderers (BackSafeRenderer)
			 * to remove the back buttons.
			 */
			NavigationPage.SetHasBackButton (this, false);

			this.ToolbarItems.Add(
				new ToolbarItem(TranslateExtension.Localize("DoneLabel"), null, () =>{
					PresentOptions();
				}
				)
			);

			if (UserMgr.getInstance ().AssociatedHyperLocal () != null) {
				this.FindByName <Frame> ("PostScopeTitleLabel").IsVisible = true;
				this.FindByName <Button> ("PostScopeButton").IsVisible = true;

				if (r.HyperLocal != null) {
					this.FindByName <Button> ("PostScopeButton").Text = r.HyperLocal.Name;
					hasTargetHyperlocal = true;
				}
			} else {
				this.FindByName <Frame> ("PostScopeFixedLabel").IsVisible = true;
			}
		}

		private async void PresentOptions(){
			if(!isWorking){

				//stop user launching another pop up.
				isWorking = true;

				string action = await DisplayActionSheet (
						TranslateExtension.Localize("NewReportLabel"), 
					TranslateExtension.Localize("DiscardLabel"), null, 
					TranslateExtension.Localize("SubmitLabelAlternative"), 
					TranslateExtension.Localize("SaveAsDraftLabel"));

				if (action != null) {
					if (action.Equals (TranslateExtension.Localize ("DiscardLabel"))) {
						DeleteImagesAndPop ();
					} else if (action.Equals (TranslateExtension.Localize ("SubmitLabelAlternative"))) {
						SubmitReport ();
					} else if (action.Equals (TranslateExtension.Localize ("SaveAsDraftLabel"))) {
						SaveAsDraft (true);
					}
				} else {
					isWorking = false;
				}
			}
		}

		public override void OnBackButton ()
		{
			PresentOptions ();
		} 

		private void DeleteImagesAndPop(){
			List<string> imagesToDelete = new List <string>();

			/*	Changes are discarded, so we can get rid of any local copies we created of images added, that 
			 * 	we aren't going to keep references to. (When we add new photos, the galleryservice creates local copies)
			 */

			foreach(string i in images){
				bool contained = false;
				foreach(string j in r.LocalPhotos){
					if(i.Equals(j)){
						contained = true;
					}
				}

				if(!contained){
					imagesToDelete.Add (i);
				}
			}

			if(imagesToDelete.Count > 0){
				ReportMgr.getInstance ().DeleteLocalFiles (imagesToDelete);
			}

			isWorking = false;

			App.navigation.PopAsync ();
		}


		private void SwitchPostScope (object sender, EventArgs args){

			hasTargetHyperlocal = !hasTargetHyperlocal;

			if (hasTargetHyperlocal) {
				this.FindByName <Button> ("PostScopeButton").Text = UserMgr.getInstance ().AssociatedHyperLocal ().Name;
			} else {
				this.FindByName <Button> ("PostScopeButton").Text = TranslateExtension.Localize ("NearbyHyperlocalLabel");
			}
		}


		private void SaveAsDraft (object sender, EventArgs args){
			SaveAsDraft (true);
		}

		private void SaveAsDraft (bool ShowConfirmation){
			r.Title = (this.FindByName <Entry>("TitleBox")).Text;
			r.Content = (this.FindByName <Editor>("ContentBox")).Text;
			r.Date = (this.FindByName <DatePicker>("DatePicker")).Date;

			if (hasTargetHyperlocal) {
				r.HyperLocal = UserMgr.getInstance ().AssociatedHyperLocal ();
			} else {
				r.HyperLocal = null;
			}
		

			/* 	as the images list is a temporary edited copy of localphotos, we can now update the saved local photos list 
				as we will not need to discard these changes.
			*/
			r.LocalPhotos.Clear ();
			foreach(string i in images){
				r.LocalPhotos.Add (i);
			}

			if(hasLocation){
				r.LocationLatitude = lat.ToString ();
				r.LocationLongitude = lon.ToString ();
			}

			if (isCreatingNew) {
				ReportMgr.getInstance ().SaveDraftReport (null,r);
			} else {
				//no need to pass anything to the ReportMgr as the reports implement INotifyPropertyChanged, so the r.title lines above will update the values automatically.
				ReportMgr.getInstance ().CommitDrafts (null);
			}

			isWorking = false;

			if(ShowConfirmation){
				App.navigation.PopAsync ();
				DisplayAlert (TranslateExtension.Localize("ReportSavedTitle"), TranslateExtension.Localize("ReportSavedLabel"), TranslateExtension.Localize("OKLabel"));
			}
		}

		private void SubmitReport(object sender, EventArgs args){
			SubmitReport ();
		}
		private void SubmitReport(){

			if (hasLocation) {

				this.FindByName<StackLayout> ("ActivityView").IsVisible = true;

				r.Title = (this.FindByName <Entry> ("TitleBox")).Text;
				r.Content = (this.FindByName <Editor> ("ContentBox")).Text;
				r.Date = (this.FindByName <DatePicker> ("DatePicker")).Date;

				r.LocationLatitude = lat.ToString ();
				r.LocationLongitude = lon.ToString ();

				if (hasTargetHyperlocal) {
					r.HyperLocal = UserMgr.getInstance ().AssociatedHyperLocal ();
				} else {
					r.HyperLocal = null;
				}
					

				ReportMgr.getInstance ().SubmitReport (ReportSubmittedResponse, r, images);
			} else {

				isWorking = false;
				DisplayAlert (TranslateExtension.Localize("LocationNeededTitle"), TranslateExtension.Localize("LocationNeededLabel"), TranslateExtension.Localize("OKLabel"));
			}
		}

		private void ReportSubmittedResponse(bool success, string message){

			this.FindByName<StackLayout> ("ActivityView").IsVisible = false;

			isWorking = false;

			if(success){
				App.navigation.PopAsync ();
				DisplayAlert (TranslateExtension.Localize("ReportSubmittedTitle"), TranslateExtension.Localize("ReportSubmittedLabel"), TranslateExtension.Localize("OKLabel"));

			}else{
				SaveAsDraft (false);
				DisplayAlert (TranslateExtension.Localize("SubmissionFailedTitle"), TranslateExtension.Localize("SubmissionFailedLabel")+message, TranslateExtension.Localize("OKLabel"));
			}
		}

		private void AddLocalImageToView(String imgSrcUri){

			(this.FindByName <Button>("GetTheImage")).BackgroundColor = BrandColors.MediumGreen;
			(this.FindByName <Button>("GetTheImage")).TextColor = Color.White;

			Image img = new Image ();

			IGalleryImageService galleryService = DependencyService.Get<IGalleryImageService>();

			Stream imgStream = galleryService.GetThumb(imgSrcUri);

			if(imgStream != null){

				img.Source = ImageSource.FromStream(() => imgStream);

				imageViews.Add (img);
				this.FindByName<StackLayout> ("ImageHolder").Children.Add (img);
				this.FindByName<StackLayout> ("ImageHolder").IsVisible = true;
			}
		}

		private void AddRemoteImageToView(String imgSrcUri){

			Image img = new Image ();

			img.Source = imgSrcUri;

			imageViews.Add (img);
			this.FindByName<StackLayout> ("ImageHolder").Children.Add (img);
			this.FindByName<StackLayout> ("ImageHolder").IsVisible = true;

		}

		private void AttachPhotos(object senderObj, EventArgs eventArgs){

			IGalleryImageService galleryService = DependencyService.Get<IGalleryImageService>();

			galleryService.LaunchImageSelector (gotUserImage);

		}

		private void gotUserImage(String selectedImgSrc){
			if(selectedImgSrc != null){

				//don't add it to the object yet. We don't want it committed to drafts until confirmed. Hold it in images for now.
				images.Add (selectedImgSrc);

				AddLocalImageToView (selectedImgSrc);

				this.FindByName <Button>("GetTheImage").BackgroundColor = BrandColors.MediumGreen;
				(this.FindByName <Button>("GetTheImage")).TextColor = Color.White;
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

				this.FindByName <Button>("GetTheLocation").BackgroundColor = BrandColors.MediumGreen;
				(this.FindByName <Button>("GetTheLocation")).TextColor = Color.White;
			}
		}

		private void ShowReportGuide(object sender, EventArgs args){
			ReportEditHelpScreen rehs = new ReportEditHelpScreen ();

			App.navigation.PushAsync (rehs);
		}

	}

	public interface IGalleryImageService
	{

		void LaunchImageSelector(Action<String> onCompletionMethod);

		Stream GetFileStream(string url);
		//void SelectImage();
		Stream GetThumb(string url);

		void DeleteFile(string url);
	}


	public interface IReportLocationService
	{
		void getCurrentLocation(Action<bool, double, double> onCompletionMethod);
	}
}

