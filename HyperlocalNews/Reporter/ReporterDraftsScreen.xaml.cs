using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Xamarin.Forms;

namespace HyperlocalNews
{
	public partial class ReporterDraftsScreen : ContentPage
	{

		ObservableCollection<Report> _Reports;

		public ReporterDraftsScreen ()
		{
			InitializeComponent ();

			Icon = "Circle.png";

			_Reports = ReportMgr.getInstance ().GetDraftReports (reportsReturned);

			this.FindByName<ListView> ("list").ItemsSource = Reports;

			NavigationPage.SetHasBackButton (this, false);
		}

		private void reportsReturned(){
			this.FindByName<StackLayout> ("ActivityView").IsVisible = false;
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

		private void EditDraft(object sender, ItemTappedEventArgs e){

			ReportEditScreen edit = new ReportEditScreen((Report)e.Item);

			NavigationPage.SetHasBackButton (edit, true);

			App.navigation.PushAsync (edit);

			this.FindByName<ListView> ("list").SelectedItem = null;
		}

		public void OnDelete(object sender, EventArgs e){

		}
	}
}