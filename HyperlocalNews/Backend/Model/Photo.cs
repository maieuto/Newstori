using System;

namespace HyperlocalNews
{
	public class Photo
	{
		public Photo ()
		{
		}

		public string ID { get; set; }
		public bool ShouldSerializeID(){
			return (ID != null);
		}

		public string PublicUrl { get; set; }
		public bool ShouldSerializePublicUrl(){
			return (PublicUrl != null);
		}
	}
}

