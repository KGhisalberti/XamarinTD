using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using Storm.Mvvm;
using Storm.Mvvm.Navigation;
using Storm.Mvvm.Services;
using TD2.View;
using Xamarin.Forms;

namespace TD2.ViewModel {
    public class PlaceItemView : ViewModelBase {

        private readonly Lazy<INavigationService> _navigationService;

        private PlaceItemSummary _place;
        private string _coord;
        private string _description;

        public PlaceItemSummary Place {
            get => _place;
            set => SetProperty(ref _place, value);
        }

        public string Coord {
            get => _coord;
            set => SetProperty(ref _coord, value);
        }

        public string Description {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public ICommand GoMapCommand { get; }
        
        public ICommand GoListComCommand { get; }

        public PlaceItemView() {
            _navigationService = new Lazy<INavigationService>(() => DependencyService.Resolve<INavigationService>());
            GoMapCommand = new Command(GoMapAction);
            GoListComCommand = new Command(GoListComAction);
        }

        public override void Initialize(Dictionary<string, object> navigationParameters) {
            base.Initialize(navigationParameters);
            Place = GetNavigationParameter<PlaceItemSummary>("PlaceItemSummary");
        }

        public override async Task OnResume() {
            await base.OnResume();
            Coord ="" + Place.Latitude + " , " + Place.Longitude;
            Description = Place.Description;
        }

        private async void GoMapAction() {
            await _navigationService.Value.PushAsync<MapPage>(new Dictionary<string, object>(){
                { "PlaceItemSummary", _place }
            });
        }

        private async void GoListComAction() {
            await _navigationService.Value.PushAsync<CommentListPage>(new Dictionary<string, object>() {
                { "IdPlace", _place.Id }
            });
        }
    }
}
