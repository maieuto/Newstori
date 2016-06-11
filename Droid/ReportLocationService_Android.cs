using System;

using Android.Locations;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(HyperlocalNews.Droid.ReportLocationService_Android))]

namespace HyperlocalNews.Droid
{
	public class ReportLocationService_Android : Java.Lang.Object, IReportLocationService, ILocationListener
	{
		public ReportLocationService_Android ()
		{
		}


		Action<bool, double, double> onCompletionMethod;
		LocationManager locMgr;

		public void getCurrentLocation(Action<bool, double, double> onCompletion){

			onCompletionMethod = onCompletion;

			MainActivity androidContext = (MainActivity)Forms.Context;


			locMgr = androidContext.GetSystemService (Android.Content.Context.LocationService) as LocationManager;
			
			Criteria locationCriteria = new Criteria();

			locationCriteria.Accuracy = Accuracy.Low;
			locationCriteria.PowerRequirement = Power.Low;

			string locationProvider = locMgr.GetBestProvider(locationCriteria, true);

			if(locationProvider != null)
			{

				Location last = locMgr.GetLastKnownLocation (locationProvider);

				//if less than 2 minutes ago, use it?
				if (last != null) {
					onCompletionMethod (true, last.Latitude, last.Longitude);
				} else {

					locMgr.RequestLocationUpdates (locationProvider, 2000, 1, this);
				}
			} 
			else 
			{
				onCompletionMethod (false, 0, 0);
			}
		}
		public void OnLocationChanged (Android.Locations.Location location)
		{
			locMgr.RemoveUpdates(this);

			onCompletionMethod (true, location.Latitude, location.Longitude);
		}

		public void OnProviderEnabled (string provider){}
		public void OnProviderDisabled (string provider){}
		public void OnStatusChanged (string provider, Availability status, Android.OS.Bundle extras){}
	}
}

