using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Xamarin.Forms;

namespace HyperlocalNews
{
	public partial class HyperlocalSelectorPage : ContentPage
	{

		private Action<Hyperlocal> receiver;

		public HyperlocalSelectorPage (Action<Hyperlocal> ReturnFunction, ObservableCollection<Hyperlocal> hyperlocals)
		{
			InitializeComponent ();

			Title = TranslateExtension.Localize("SelectHyperlocalLabel");

			receiver = ReturnFunction;

			this.FindByName<ListView> ("list").ItemsSource = hyperlocals;
		}

		public void HyperlocalWasSelected(object sender, ItemTappedEventArgs e){

			receiver ((Hyperlocal)e.Item);
			App.navigation.PopAsync ();

		}
	}
}

