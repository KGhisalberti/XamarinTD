using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using TD2.View;
using Storm.Mvvm;
using Storm.Mvvm.Services;
using Xamarin.Forms;



namespace TD2 {
    class MainPageModelView : ViewModelBase {
        private readonly Lazy<IDialogService> _dialogService;
        private readonly Lazy<INavigationService> _navigationService;
        private readonly ApiClient _apiClient = new ApiClient();

        public ObservableCollection<PlaceItemSummary> PlaceList {
            get;
        }

        public ICommand SelectPlaceCommand { get; }

        public ICommand GoNewPlaceCommand { get; }

        public ICommand GoCustomCommand { get; }

        public MainPageModelView() {
            _dialogService = new Lazy<IDialogService>(() => DependencyService.Resolve<IDialogService>());
            _navigationService = new Lazy<INavigationService>(() => DependencyService.Resolve<INavigationService>());

            PlaceList = new ObservableCollection<PlaceItemSummary>();
            SelectPlaceCommand = new Command<PlaceItemSummary>(SelectPlaceAction);
            GoNewPlaceCommand = new Command(GoNewPlaceAction);
            GoCustomCommand = new Command(GoCustomAction);
        }

        public override void Initialize(Dictionary<string, object> navigationParameters) {
            base.Initialize(navigationParameters);
        }

        public override async Task OnResume() {
            await base.OnResume();
            HttpResponseMessage res;
            try {
                res = await _apiClient.Execute(HttpMethod.Get, Urls.URI + Urls.PLACES);
            }catch {
                await _dialogService.Value.DisplayAlertAsync("Connection Failed...", "", "Ok");
                return;
            }
            Response<Collection<PlaceItemSummary>> resp = await _apiClient.ReadFromResponse<Response<Collection<PlaceItemSummary>>>(res);
            if (resp.IsSuccess) {
                Dictionary<int, PlaceItemSummary> placeById = PlaceList.ToDictionary(x => x.Id, x => x);
                foreach (PlaceItemSummary place in resp.Data) {
                    if (!placeById.ContainsKey(place.Id)) {
                        place.SelectPlaceCommand = SelectPlaceCommand;
                        PlaceList.Add(place);
                    }
                }
            } else if (PlaceList.Count == 0) {
                await _dialogService.Value.DisplayAlertAsync("Échec de récupération de la liste", "", "OK");
            }
        }

        public async void SelectPlaceAction(PlaceItemSummary place) {
            await _navigationService.Value.PushAsync<PlaceItemPage>(new Dictionary<string, object>()
            {
                { "PlaceItemSummary", place }
            });
        }

       public async void GoNewPlaceAction() {
            await _navigationService.Value.PushAsync<NewPlacePage>();
        }

        public async void GoCustomAction() {
            await _navigationService.Value.PushAsync<CustomPage>();
        }

       
    }
}

