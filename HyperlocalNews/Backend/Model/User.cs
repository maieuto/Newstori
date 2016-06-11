using System;

namespace HyperlocalNews
{
	public class User
	{

		public User ()
		{
		}

		public string ID { get; set; }
		public bool ShouldSerializeID(){
			return (ID != null);
		}

		public string Token { get; set; }
		public bool ShouldSerializeToken(){
			return (Token != null);
		}

		public string Fullname { get; set; }
		public bool ShouldSerializeFullname(){
			return (Fullname != null);
		}

		public string Email { get; set; }
		public bool ShouldSerializeEmail(){
			return (Email != null);
		}

		public Hyperlocal AssociatedHyperLocal { get; set; }
		public bool ShouldSerializeAssociatedHyperLocal(){
			return (AssociatedHyperLocal != null);
		}

		public Hyperlocal AdministeringHyperLocal { get; set; }
		public bool ShouldSerializeAdministeringHyperLocal(){
			return (AdministeringHyperLocal != null);
		}

		public string Language { get; set; }

	}
}

