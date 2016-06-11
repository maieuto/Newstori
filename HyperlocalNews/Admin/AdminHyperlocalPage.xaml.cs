using System;
using System.Collections.ObjectModel;

using Xamarin.Forms;

namespace HyperlocalNews
{
	public partial class AdminHyperlocalPage : ContentPage
	{

		ObservableCollection<Report> _Reports;

		public AdminHyperlocalPage ()
		{
			InitializeComponent ();

			Title = UserMgr.getInstance().AdministeringHyperLocal().Name;

			_Reports = ReportMgr.getInstance ().GetReportsForHyperlocal (this.ReceivedReports);

			this.FindByName<ListView> ("list").ItemsSource = Reports;
		}

		protected override void OnAppearing ()
		{
			base.OnAppearing ();
		}

		public ObservableCollection<Report> Reports
		{ 
			get { 
				return _Reports;
			} 
		}

		private void ViewReportDetailed(object sender, ItemTappedEventArgs e){

			FeedbackScreen feedback = new FeedbackScreen((Report)e.Item);

			NavigationPage.SetHasBackButton (feedback, true);

			App.navigation.PushAsync (feedback);

			this.FindByName<ListView> ("list").SelectedItem = null;
		}

		public void ReceivedReports(bool success, string message){
			//web request for list update has returned. Maybe hide activity indicator?

			this.FindByName<StackLayout> ("ActivityView").IsVisible = false;
		}

	}
}

