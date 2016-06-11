using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Maps;

using XLabs.Platform.Services.Geolocation;
using XLabs.Ioc;

namespace HyperlocalNews
{
	public partial class LocationPage : ContentPage
	{

		private Action<bool, double, double> onCompletion;

		private double givenLat;
		private double givenLon;

		IGeolocator geolocator;

		Map m;


		public LocationPage (Action<bool, double, double> CompletionMethod, double lat, double lon, string title, bool isReadOnly, bool shouldShowUser)
		{
			InitializeComponent ();

			givenLat = lat;
			givenLon = lon;

			onCompletion = CompletionMethod;

			this.Title = title;

			if(!isReadOnly){
				this.ToolbarItems.Add(
					new ToolbarItem(TranslateExtension.Localize("ConfirmLabel"), null, () =>{
						ConfirmLocation();
					}
					)
				);
			}

			m = this.FindByName<Map>("MapItem");

			if(shouldShowUser){
				m.IsShowingUser = true;

				if (geolocator == null) {
					//retrieve XLabs Geolocator service

					geolocator = DependencyService.Get<IGeolocator> ();
					geolocator.PositionChanged += OnPositionChanged;
					geolocator.PositionError += OnListeningError;
					geolocator.StartListening (30000, 0, true);
					
				}
			}

			if (isReadOnly) {
				Pin p = new Pin ();
				p.Position = new Xamarin.Forms.Maps.Position (lat, lon);
				p.Type = PinType.SavedPin;
				p.Label = title;

				m.Pins.Add (p);

				this.FindByName<StackLayout> ("Crosshair").IsVisible = false;
			}

			MapSpan sp = MapSpan.FromCenterAndRadius(new Xamarin.Forms.Maps.Position(lat, lon), Distance.FromMeters(400));
			m.MoveToRegion (sp);
		}

		/// <summary>
		/// Handles the <see cref="E:ListeningError" /> event.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="PositionErrorEventArgs"/> instance containing the event data.</param>
		private void OnListeningError(object sender, PositionErrorEventArgs e)
		{
			
		}

		/// <summary>
		/// Handles the <see cref="E:PositionChanged" /> event.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="PositionEventArgs"/> instance containing the event data.</param>
		private void OnPositionChanged(object sender, PositionEventArgs e)
		{
			if(e.Position.Latitude != 0 && e.Position.Longitude != 0){
				MapSpan sp = MapSpan.FromCenterAndRadius(new Xamarin.Forms.Maps.Position(e.Position.Latitude, e.Position.Longitude), Distance.FromMeters(400));
				m.MoveToRegion (sp);
			}
			geolocator.StopListening ();
		}

		public void ConfirmLocation(object senderObj, EventArgs eventArgs){
			ConfirmLocation ();
		}
		public void ConfirmLocation(){
			if(geolocator != null){
				geolocator.StopListening ();
			}
			
			Xamarin.Forms.Maps.Position p = m.VisibleRegion.Center;

			if(onCompletion != null){
				onCompletion(true, p.Latitude, p.Longitude);
			}
		}
	}
}

