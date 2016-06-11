using System;
using System.ComponentModel;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace HyperlocalNews
{
	public class Report : INotifyPropertyChanged
	{
		private string _ID;
		private string _Title;
		private string _Content;
		private DateTime _Date;
		private FeedbackItem _Feedback = new FeedbackItem();
		private List<Photo> _Photos;
		private List<string> _LocalPhotos;
		private Hyperlocal _HyperLocal;
		private string _LocationLatitude;
		private string _LocationLongitude;
		private Color _CellColour;
		private Color _ReportHeadingColour;
		private Color _AdminReportCellColour;
		private User _ClaimedBy;
		private string _ClaimedStatus;
		private int _Declined;

		public event PropertyChangedEventHandler PropertyChanged;

		private void NotifyPropertyChanged(String info)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(info));
			}
		}

		public Report ()
		{
		}
		public Report (string title, string content)
		{
			Title = title;
			Content = content;
		}
		public string ID{

			get{ return _ID; } 
			set{ 
				if(this._ID != value){
					this._ID = value;
					NotifyPropertyChanged ("ID");
				}
			} 
		}
		public bool ShouldSerializeID(){
			return (ID != null);
		}
		public string Title{

			get{ return _Title; } 
			set{ 
				if(this._Title != value){
					this._Title = value;
					NotifyPropertyChanged ("Title");
				}
			} 
		}
		public string Content{

			get{ return _Content; } 
			set{ 
				if(this._Content != value){
					this._Content = value;
					NotifyPropertyChanged ("Content");
				}
			}
		}
		public string ContentLimited{

			get{ 
				if (Content != null) {
					string str = Content;
					if (str.Length > 450) {
						str = str.Substring (0, 400) + "...";
					}
					return str;
				} else {
					return null;
				}
			}
		}
		public bool ShouldSerializeContentLimited(){
			return false;
		}
			

		public Color CellColour{
			get{
				_CellColour = BrandColors.LightGrey;
				if(this.HyperLocal != null){
					_CellColour = BrandColors.LightGrey;
				}
				return _CellColour;
			}
		}

		public Color AdminReportCellColour{
			get{
				_AdminReportCellColour = BrandColors.LightGrey;
				if(this.ClaimedBy != null || this.Declined == 1){
					_AdminReportCellColour = BrandColors.InvalidGrey;
				}
				return _AdminReportCellColour;
			}
		}

		public Color ReportHeadingColour{
			get{
				_ReportHeadingColour = BrandColors.LightPink;
				if(this.ClaimedBy != null || this.Declined == 1){
					_ReportHeadingColour = BrandColors.DarkGrey;
				}
				return _ReportHeadingColour;
			}
		}

		public bool ShouldSerializeCellColour(){
			return false;
		}
		public DateTime Date{

			get{ return _Date; } 
			set{ 
				if(this._Date != value){
					this._Date = value;
					NotifyPropertyChanged ("Date");
				}
			}
		}
		public string FormattedDate{
			get{ return _Date.ToString("d", TranslateExtension.ci.DateTimeFormat); }
		}
		public bool ShouldSerializeFormattedDate(){
			return false;
		}
		public FeedbackItem Feedback{

			get{ return _Feedback; } 
			set{ 
				if(this._Feedback != value){
					this._Feedback = value;
					NotifyPropertyChanged ("Feedback");
				}
			}
		}
		public bool ShouldSerializeFeedback(){
			return (Feedback != null);
		}
		/*public string FeedbackResponse{

			get{ return _FeedbackResponse; } 
			set{ 
				if(this._FeedbackResponse != value){
					this._FeedbackResponse = value;
					NotifyPropertyChanged ("FeedbackResponse");
				}
			}
		}*/

		public Hyperlocal HyperLocal{

			get{ return _HyperLocal; } 
			set{ 
				if(this._HyperLocal != value){
					this._HyperLocal = value;
					NotifyPropertyChanged ("HyperLocal");
					NotifyPropertyChanged ("CellColour");
				}
			}
		}

		public bool ShouldSerializeHyperLocal(){
			return (HyperLocal != null);
		}

		public string LocationLatitude{

			get{ return _LocationLatitude; } 
			set{ 
				if(this._LocationLatitude != value){
					this._LocationLatitude = value;
					NotifyPropertyChanged ("LocationLatitude");
				}
			}
		}

		public User ClaimedBy{

			get{ return _ClaimedBy; } 
			set{ 
				if(this._ClaimedBy != value){
					this._ClaimedBy = value;
					NotifyPropertyChanged ("ClaimedBy");
					NotifyPropertyChanged ("ClaimedStatus");
					NotifyPropertyChanged ("AdminReportCellColour");
					NotifyPropertyChanged ("ReportHeadingColour");
				}
			}
		}

		public string ClaimedStatus{
			get{
				_ClaimedStatus = TranslateExtension.Localize("ClaimedPrefix") +  "-";
				if(this.ClaimedBy != null){
					_ClaimedStatus =  TranslateExtension.Localize("ClaimedPrefix") + this.ClaimedBy.Fullname;
				}
				return _ClaimedStatus;
			}
		}


		public int Declined{

			get{ return _Declined; } 
			set{ 
				if(this._Declined != value){
					this._Declined = value;
					NotifyPropertyChanged ("Declined");
					NotifyPropertyChanged ("AdminReportCellColour");
					NotifyPropertyChanged ("ReportHeadingColour");
				}
			}
		}


		public string LocationLongitude{

			get{ return _LocationLongitude; } 
			set{ 
				if(this._LocationLongitude != value){
					this._LocationLongitude = value;
					NotifyPropertyChanged ("LocationLongitude");
				}
			}
		}
			
		public List<Photo> Photos{

			get{
				if (_Photos == null) {
					_Photos = new List<Photo> ();
				}
				return _Photos; 
			}
			set{ 
				if(this._Photos != value){
					this._Photos = value;
					NotifyPropertyChanged ("Photos");
				}
			}
		}
		public bool ShouldSerializePhotos(){
			return (Photos != null);
		}

		public List<string> LocalPhotos {

			get{
				if (_LocalPhotos == null) {
					_LocalPhotos = new List<string> ();
				}
				return _LocalPhotos; 
			}
			set{ 
				if(this._LocalPhotos != value){
					this._LocalPhotos = value;
					NotifyPropertyChanged ("LocalPhotos");
				}
			}
		}

		public void AddPhoto(string photoToAdd){
			if(_LocalPhotos == null){
				_LocalPhotos = new List<string> ();
			}
			if ( _LocalPhotos.Contains(photoToAdd)) {
				_LocalPhotos.Add (photoToAdd);
				NotifyPropertyChanged ("LocalPhotos");
			}
		}
	}
}


