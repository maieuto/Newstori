using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace HyperlocalNews
{
	public partial class FeedbackScreen : ContentPage
	{
		private Report r;
		//private List<Image> imageViews = new List<Image>();

		public FeedbackScreen (Report report)
		{
			InitializeComponent ();

			r = report;

			Title = report.Title;

			(this.FindByName <StackLayout>("layout")).BindingContext = r;


			//Image button = this.FindByName <Image> ("DeclineButton");
			//button.GestureRecognizers.Add (new TapGestureRecognizer (sender => {
			//	DeclineArticle();
			//}));

			UpdateReportClaimedSection ();


			if(r.Photos.Count > 0){
				foreach(Photo ph in r.Photos){
					//AddLocalImageToView (imgSrc);
					AddRemoteImageToView (ph.PublicUrl);
				}
			}


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

			if( ! String.IsNullOrEmpty(r.Feedback.ResponseMessage)){
				this.FindByName<StackLayout> ("UserResponseBox").IsVisible = true;
			}

			this.ToolbarItems.Add(
				new ToolbarItem(TranslateExtension.Localize("SendFeedbackLabel"), null, () =>{
					SendFeedback();
				}
				)
			);
		}

		private void UpdateReportClaimedSection(){
			if (r.ClaimedBy != null) {

				if (r.ClaimedBy.ID.Equals (UserMgr.getInstance ().ID ())) {
					//I claimed it

					this.FindByName <Button> ("DeclineButton").IsVisible = false;

					Button b = (this.FindByName <Button> ("ClaimButton"));
					b.Text = r.ClaimedStatus;
				} else {
					//someone else claimed it

					this.FindByName <Button> ("DeclineButton").IsVisible = false;
					this.FindByName <Button> ("ClaimButton").IsVisible = false;

					Label l = this.FindByName <Label> ("ClaimedLabel");
					l.Text = r.ClaimedStatus;
					l.IsVisible = true;
				}

			} else if (r.Declined == 1) {
				//someone trashed it

				this.FindByName <Button> ("ClaimButton").IsVisible = false;

				Button b = this.FindByName <Button> ("DeclineButton");
				b.BackgroundColor = BrandColors.LightGreen;
				b.Text = TranslateExtension.Localize ("DeclinedString");
			} else {

				this.FindByName <Button> ("ClaimButton").IsVisible = true;
				this.FindByName <Button> ("ClaimButton").Text = TranslateExtension.Localize ("ClaimButtonLabel");
				this.FindByName <Button> ("DeclineButton").IsVisible = true;
				this.FindByName <Button> ("DeclineButton").BackgroundColor = BrandColors.LightMediumGrey;
				this.FindByName <Button> ("DeclineButton").Text = TranslateExtension.Localize ("DeclinedButtonLabel");
				this.FindByName <Label> ("ClaimedLabel").IsVisible = false;
			}
		}

		private void SendFeedback (object sender, EventArgs args){

			SendFeedback ();
		}

		private void SendFeedback (){

			r.Feedback.Message = (this.FindByName <Editor>("Feedback")).Text;

			if((this.FindByName<Button>("DateButton").BackgroundColor == BrandColors.MediumGreen)){
				r.Feedback.RequestDate = 1;
			}
			if((this.FindByName<Button>("PhotosButton").BackgroundColor == BrandColors.MediumGreen)){
				r.Feedback.RequestPhoto = 1;
			}
			if((this.FindByName<Button>("LocationButton").BackgroundColor == BrandColors.MediumGreen)){
				r.Feedback.RequestLocation = 1;
			}

			ReportMgr.getInstance ().SendFeedback (receivedSubmissionResponse, r);

			this.FindByName<StackLayout> ("ActivityView").IsVisible = true;
		}

		private void AdditionalItemToggle(object sender, EventArgs args){
			Button b = (Button)sender;

			if (b.BackgroundColor == Color.FromHex ("#eeeeee")) {
				b.BackgroundColor = BrandColors.MediumGreen;
				b.TextColor = Color.White;
			} else {
				b.BackgroundColor = Color.FromHex ("#eeeeee");
				b.TextColor = BrandColors.LightPink;
			}
		}

		private void receivedSubmissionResponse(bool success, string message){

			this.FindByName<StackLayout> ("ActivityView").IsVisible = false;

			if(success){
				//DisplayAlert ("Feedback Sent", "Your feedback has been sent.", "OK");
				DisplayAlert (TranslateExtension.Localize("FeedbackSendTitle"), TranslateExtension.Localize("FeedbackSendLabel"), TranslateExtension.Localize("OKLabel"));
			}else{
				DisplayAlert (TranslateExtension.Localize("FeedbackFailedTitle"), TranslateExtension.Localize("FeedbackFailedLabel"), TranslateExtension.Localize("OKLabel"));
			}
		}


		private void AddRemoteImageToView(String imgSrcUri){

			Image img = new Image ();

			img.Source = imgSrcUri;

			//imageViews.Add (img);
			this.FindByName<StackLayout> ("ImageHolder").Children.Add (img);
			this.FindByName<StackLayout> ("ImageHolder").IsVisible = true;

		}

		private void ViewLocation(object sender, EventArgs args){
			ViewLocation ();	
		}

		private void ViewLocation(){
			LocationPage lp = new LocationPage (null, Double.Parse(r.LocationLatitude), Double.Parse(r.LocationLongitude), r.Title, true, false);
			App.navigation.PushAsync (lp);
		}

		private void ExportArticle (object sender, EventArgs args){
			ReportMgr.getInstance ().ExportReport (ExportArticleReturned, r);
		}
		private void ExportArticleReturned (bool success, string message){
			if(success){
				//DisplayAlert ("Article Exported", "The article has been sent to your email address.", "Okay");
				DisplayAlert (TranslateExtension.Localize("ArticleExplortedTitle"), TranslateExtension.Localize("ArticleExplortedLabel"), TranslateExtension.Localize("OKLabel"));
			}else{
				DisplayAlert (TranslateExtension.Localize("ArticleExplortFailedTitle"), TranslateExtension.Localize("ArticleExplortFailedLabel"), TranslateExtension.Localize("OKLabel"));
			}
		}


		private void ClaimArticle (object sender, EventArgs args){
			ClaimArticle ();
		}
		private void ClaimArticle (){
			if (r.ClaimedBy != null) {
				ReportMgr.getInstance ().UnclaimReport (ClaimArticleReturned, r);
			} else {
				ReportMgr.getInstance ().ClaimReport (ClaimArticleReturned, r);
			}
		}
		private void ClaimArticleReturned (bool success, string message){
			if(success){

				if (r.ClaimedBy != null) {
					r.ClaimedBy = null;
					//DisplayAlert ("Article Claim Removed", "You have removed your claim on this article.", "Okay");
					DisplayAlert (TranslateExtension.Localize("ArticleClaimRemovedTitle"), TranslateExtension.Localize("ArticleClaimRemovedLabel"), TranslateExtension.Localize("OKLabel"));
				} else {
					r.ClaimedBy = UserMgr.getInstance ().GetUserObject ();
					//DisplayAlert ("Article Claimed", "You have claimed this article.", "Okay");
					DisplayAlert (TranslateExtension.Localize("ArticleClaimedTitle"), TranslateExtension.Localize("ArticleClaimedLabel"), TranslateExtension.Localize("OKLabel"));
				}

				UpdateReportClaimedSection ();

				ReportMgr.getInstance().ReloadReportsForHyperlocal ();

				/*
				Button b = (this.FindByName <Button> ("ClaimButton"));
				b.IsEnabled = false;
				b.TextColor = Color.White;
				b.BackgroundColor = BrandColors.MediumGreen;
				b.Text = TranslateExtension.Localize("ClaimedString");*/
			}else{
				//DisplayAlert ("Article Claim Failed", "The article could not be claimed because: "+message, "Okay");
				DisplayAlert (TranslateExtension.Localize("ArticleClaimFailedTitle"), TranslateExtension.Localize("ArticleClaimFailedLabel")+message, TranslateExtension.Localize("OKLabel"));
			}
		}

		private void DeclineArticle (object sender, EventArgs args){
			DeclineArticle ();
		}
		private void DeclineArticle (){
			if (r.Declined == 1) {
				ReportMgr.getInstance ().UndeclineReport (DeclineArticleReturned, r);
			} else {
				ReportMgr.getInstance ().DeclineReport (DeclineArticleReturned, r);
			}
		}
		private void DeclineArticleReturned (bool success, string message){
			if(success){

				if (r.Declined == 1) {
					r.Declined = 0;
					//DisplayAlert ("Article undeclined", "You have undeclined this article.", "Okay");
					DisplayAlert (TranslateExtension.Localize("ArticleUndeclinedTitle"), TranslateExtension.Localize("ArticleUndeclinedLabel"), TranslateExtension.Localize("OKLabel"));
				} else {
					r.Declined = 1;
					//DisplayAlert ("Article declined", "You have declined this article.", "Okay");
					DisplayAlert (TranslateExtension.Localize("ArticleDeclinedTitle"), TranslateExtension.Localize("ArticleDeclinedLabel"), TranslateExtension.Localize("OKLabel"));
				}

				UpdateReportClaimedSection ();

				ReportMgr.getInstance().ReloadReportsForHyperlocal ();

				/*
				Button b = (this.FindByName <Button> ("DeclineButton"));
				b.IsEnabled = false;
				b.TextColor = Color.White;
				b.BackgroundColor = BrandColors.MediumGreen;
				b.Text = TranslateExtension.Localize("DeclinedString");
				//b.FontSize = 10; */
			}else{
				//DisplayAlert ("Article Decline Failed", "The article could not be declined because: "+message, "Okay");
				DisplayAlert (TranslateExtension.Localize("ArticleDeclineFailedTitle"), TranslateExtension.Localize("ArticleDeclineFailedLabel")+message, "Okay");
			}
		}
	}

}
