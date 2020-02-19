using Storm.Mvvm;
using Storm.Mvvm.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace TD2.ModelView {
    public class MapView : ViewModelBase {


        private PlaceItemSummary _place;
        private Map _map;
        
        public void Map(Map map) {
            _map = map;
        }

        public PlaceItemSummary Place {
            get => _place;
            set => SetProperty(ref _place, value);
        }

        public MapView() {
           
        }

        public override void Initialize(Dictionary<string, object> navigationParameters) {
            base.Initialize(navigationParameters);
            Place = GetNavigationParameter<PlaceItemSummary>("PlaceItemSummary");
        }

        public override async Task OnResume() {
            await base.OnResume();
            Position position = new Position(Place.Latitude, Place.Longitude);

            MapSpan mapSpan = new MapSpan(position, 0.01, 0.01);
            _map.MoveToRegion(mapSpan);
            if (_map.Pins.Count == 0) {
                _map.Pins.Add(new Pin {
                    Label = Place.Title,
                    Type = PinType.Place,
                    Position = position
                });
            }

        }
    }
}