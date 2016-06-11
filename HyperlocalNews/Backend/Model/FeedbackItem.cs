using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace HyperlocalNews
{

	public class FeedbackItem : INotifyPropertyChanged
	{

		private int _RequestPhoto = 0;
		private int _RequestDate = 0;
		private int _RequestLocation = 0;

		private string _FullName;
		private string _Message;
		private string _ResponseMessage;

		public event PropertyChangedEventHandler PropertyChanged;

		public FeedbackItem()
		{
		}

		private void NotifyPropertyChanged(String info)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(info));
			}
		}

		public int RequestPhoto{

			get{ return _RequestPhoto; } 

			set{ 
				if(this._RequestPhoto != value){
					this._RequestPhoto = value;
					NotifyPropertyChanged ("RequestPhoto");
				}
			}
		}

		public int RequestDate{

			get{ return _RequestDate; } 

			set{ 
				if(this._RequestDate != value){
					this._RequestDate = value;
					NotifyPropertyChanged ("RequestDate");
				}
			}
		}

		public int RequestLocation{

			get{ return _RequestLocation; } 

			set{ 
				if(this._RequestLocation != value){
					this._RequestLocation = value;
					NotifyPropertyChanged ("RequestLocation");
				}
			}
		}

		public string FullNameLabel{

			get{ return _FullNameLabel; } 

			set{ 
				if(this.FullNameLabel != value){
					this.FullNameLabel = value;
					NotifyPropertyChanged ("RequestLocation");
				}
			}
		}

		public string Message{

			get{ return _Message; } 

			set{ 
				if(this._Message != value){
					this._Message = value;
					NotifyPropertyChanged ("Message");
				}
			}
		}

		public string ResponseMessage {

			get{ return _ResponseMessage; } 

			set { 
				if (this._ResponseMessage != value) {
					this._ResponseMessage = value;
					NotifyPropertyChanged ("ResponseMessage");
				}
			}
		}

	}}
	