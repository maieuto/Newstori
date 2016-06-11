using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Xamarin.Forms;

namespace HyperlocalNews
{
	public partial class ReporterSentScreen : ContentPage
	{


		ObservableCollection<Report> _Reports;

		public ReporterSentScreen ()
		{
			InitializeComponent ();

			Icon = "Circle.png";

		}

		protected override void OnAppearing ()
		{
			base.OnAppearing ();

			this.FindByName<StackLayout> ("ActivityView").IsVisible = true;

			_Reports = ReportMgr.getInstance ().GetReportsSubmitted (this.ReceivedReports);

			this.FindByName<ListView> ("list").ItemsSource = Reports;
		}

		public ObservableCollection<Report> Reports
		{ 
			get { 
				return _Reports;
			} 
		}

		private void ViewReportDetailed(object sender, ItemTappedEventArgs e){

			FeedbackResponseScreen feedback = new FeedbackResponseScreen((Report)e.Item);

			NavigationPage.SetHasBackButton (feedback, true);

			App.navigation.PushAsync (feedback);
			this.FindByName<ListView> ("list").SelectedItem = null;
		}
			
		public void ReceivedReports(bool success, string message){
			//web request for list update has returned. Hide activity indicator:
			this.FindByName<StackLayout> ("ActivityView").IsVisible = false;
		}
	}
}

