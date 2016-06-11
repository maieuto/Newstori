using System;

[assembly: Xamarin.Forms.Dependency(typeof(HyperlocalNews.iOS.ReportLocationService_iOS))]
namespace HyperlocalNews.iOS
{
	public class ReportLocationService_iOS : IReportLocationService
	{
		public ReportLocationService_iOS ()
		{
		}


		public void getCurrentLocation(Action<bool, double, double> onCompletionMethod){
			onCompletionMethod (false, 0, 0);
		}

	}
}

