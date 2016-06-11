using System;
using Xamarin.Forms;


namespace HyperlocalNews
{
	public class ReportCell : ViewCell
	{
		public ReportCell ()
		{
			StackLayout stl = new StackLayout ();
			stl.Orientation = StackOrientation.Vertical;
			stl.HorizontalOptions = LayoutOptions.Start;

			AbsoluteLayout al = new AbsoluteLayout ();
			al.HorizontalOptions = LayoutOptions.Fill;
			al.VerticalOptions = LayoutOptions.Fill;

			stl.BackgroundColor = Color.FromHex ("#dddddd");
			stl.Padding = new Thickness (20,10,20,10);

			BoxView bv = new BoxView ();
			bv.HorizontalOptions = LayoutOptions.Fill;
			bv.BackgroundColor = Color.FromHex ("#eeeeee");
			bv.VerticalOptions = LayoutOptions.Fill;

			var TitleLabel = new Label()
			{
				YAlign = TextAlignment.Center,
				XAlign = TextAlignment.Start,
				TextColor = Color.Black,
				VerticalOptions = LayoutOptions.Start,
				HorizontalOptions = LayoutOptions.Fill,
				BackgroundColor = Color.FromHex("#eeeeee")
			};
			var ContentLabel = new Label()
			{
				YAlign = TextAlignment.Center,
				XAlign = TextAlignment.Start,
				TextColor = Color.Gray,
				VerticalOptions = LayoutOptions.Start,
				HorizontalOptions = LayoutOptions.Fill,
				BackgroundColor = Color.FromHex("#eeeeee")
			};
			TitleLabel.SetBinding(Label.TextProperty, "Title");
			ContentLabel.SetBinding(Label.TextProperty, "Content");

			al.Children.Add (bv);

			Frame fTitle = new Frame () {
				BackgroundColor = Color.FromHex("#eeeeee"),
				Padding = new Thickness(5,5,5,0)
			};
			Frame fContent = new Frame () {
				BackgroundColor = Color.FromHex("#eeeeee"),
				Padding = new Thickness(5,0,5,5)
			};

			stl.Children.Add (TitleLabel);
			stl.Children.Add (ContentLabel);

			this.View = stl;
		}

		protected override void OnBindingContextChanged()
		{
			View.BindingContext = BindingContext;
			base.OnBindingContextChanged();
		}
	}
}

