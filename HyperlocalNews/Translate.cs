using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Resources;
using System.Globalization;
using System.Reflection;

namespace HyperlocalNews
{

	/*
	 *  XAML Markup Translation For Localization
	 * 	As Detailed: http://developer.xamarin.com/guides/cross-platform/xamarin-forms/localization/	
	 *  TODO: Consider moving this into a separate file
	 */

	// You exclude the 'Extension' suffix when using in Xaml markup
	[ContentProperty ("Text")]
	public class TranslateExtension : IMarkupExtension
	{

		const string ResourceId = "HyperlocalNews.Resx.AppResources";
		public static CultureInfo ci ;

		public TranslateExtension() {
			string userLocale = UserMgr.getInstance ().PreferedLanguage ();

			//set default language
			string netLanguage = "en-GB";

			//check for CYM user preference and set netlanguage appropriately
			if(userLocale == "cym")
			{
				netLanguage = "cy-GB";	
			}

			ci = new System.Globalization.CultureInfo(netLanguage);
		}

		public static void UpdateCurrentLanguage(){
			//ci = DependencyService.Get<ILocalize>().GetCurrentCultureInfo ();
			string userLocale = UserMgr.getInstance ().PreferedLanguage ();

			//set default language
			string netLanguage = "en-GB";

			//check for CYM user preference and set netlanguage appropriately
			if(userLocale == "cym")
			{
				netLanguage = "cy-GB";	
			}

			ci = new System.Globalization.CultureInfo(netLanguage);
		}

		public string Text { get; set; }

		public object ProvideValue (IServiceProvider serviceProvider)
		{
			if (Text == null)
				return "";

			ResourceManager resmgr = new ResourceManager(ResourceId
				, typeof(TranslateExtension).GetTypeInfo().Assembly);

			var translation = resmgr.GetString (Text, ci);

			if (translation == null) {
				#if DEBUG
				throw new ArgumentException (
					String.Format ("Key '{0}' was not found in resources '{1}' for culture '{2}'.", Text, ResourceId, ci.Name),
					"Text");
				#endif
			}
			return translation;
		}


		public static string Localize(string key) {

			ResourceManager temp = new ResourceManager(ResourceId, typeof(TranslateExtension).GetTypeInfo().Assembly);

			string result = temp.GetString (key, ci);

			return result; 
		}
	}
}

