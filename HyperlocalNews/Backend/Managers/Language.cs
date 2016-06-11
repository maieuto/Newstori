using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Reflection;
using System.IO;

namespace HyperlocalNews
{
	public class Language
	{
		/* singleton manager */
		private static Language instance;

		private Language () {}

		public static Language getInstance(){

			if(instance == null){
				instance = new Language();
			}
			return instance;

		}


		private static Dictionary<string, Dictionary<string, string>> stringsDictionary = null;

		private string languageSelected;


		// static method that will return a string for the key, in the current selected language.
		public static string Str(string key){
			string returnStr = "";

			if(stringsDictionary == null){
				instance.GetPreferredLanguage ();
			}

			returnStr = stringsDictionary[key][instance.languageSelected];
			
			if(returnStr == null){
				returnStr = "Local String Not Found";
			}

			return returnStr;
		}

		private void GetPreferredLanguage(){
			languageSelected = UserMgr.getInstance ().PreferedLanguage();
		}

	}

}

