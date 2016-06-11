using System;

namespace HyperlocalNews
{
	public class Hyperlocal
	{
		public Hyperlocal ()
		{

		}

		public string ID { get; set; }
		public bool ShouldSerializeID(){
			return (ID != null);
		}

		public string Name { get; set; }
		public bool ShouldSerializeName(){
			return (Name != null);
		}

		public string LocationLatitude { get; set; }
		public bool ShouldSerializeLocationLatitude(){
			return (LocationLatitude != null);
		}

		public string LocationLongitude { get; set; }
		public bool ShouldSerializeLocationLongitude(){
			return (LocationLongitude != null);
		}

		public string AdminEmail { get; set; }
		public bool ShouldSerializeAdminEmail(){
			return (AdminEmail != null);
		}

	}
}