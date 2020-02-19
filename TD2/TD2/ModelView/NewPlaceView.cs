using System;
using System.IO;
using System.Net.Http;
using System.Windows.Input;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Storm.Mvvm;
using Storm.Mvvm.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
namespace TD2.ModelView {
    public class NewPlaceView : ViewModelBase {
        private readonly Lazy<INavigationService> _navigationService;
        private readonly Lazy<IDialogService> _dialogService;
        private readonly ApiClient _apiClient =new ApiClient();

        private string _title;
        private string _description;
        private double _latitude;
        private double _longitude;
        private string _coordText;
        private bool _coordEnable;
        private int _imageId;
        private bool _imageIdVisible;
        private bool _photoButtonEnable;

        public string Title {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public string Description {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public double Latitude {
            get => _latitude;
            set => SetProperty(ref _latitude, value);
        }

        public double Longitude {
            get => _longitude;
            set => SetProperty(ref _longitude, value);
        }

        public string CoordText {
            get => _coordText;
            set => SetProperty(ref _coordText, value);
        }

        public bool CoordEnable {
            get => _coordEnable;
            set => SetProperty(ref _coordEnable, value);
        }

        public int ImageId {
            get => _imageId;
            set => SetProperty(ref _imageId, value);
        }

        public bool ImageIdVisible {
            get => _imageIdVisible;
            set => SetProperty(ref _imageIdVisible, value);
        }

        public bool PhotoButtonEnable {
            get => _photoButtonEnable;
            set => SetProperty(ref _photoButtonEnable, value);
        }

        public ICommand CoordCommand { get; }

        public ICommand CustomCoordCommand { get;  }

        public ICommand AddImageCommand { get; }

        public ICommand SavePlaceCommand { get; }

        public NewPlaceView() {
            _navigationService = new Lazy<INavigationService>(() => DependencyService.Resolve<INavigationService>());
            _dialogService = new Lazy<IDialogService>(() => DependencyService.Resolve<IDialogService>());

            CoordText = "Find my Location";
            CoordEnable = true;

            ImageIdVisible = false;
            PhotoButtonEnable = true;

            CoordCommand = new Command(CoordAction);
            CustomCoordCommand = new Command(CustomCoordAction);
            AddImageCommand = new Command(AddImageAction);
            SavePlaceCommand = new Command(SavePlaceAction);
        }

        public async void CoordAction() {
            CoordEnable = true;
            CoordText = "Retrieving coordinates...";

            try {
                GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Medium);
                Xamarin.Essentials.Location location = await Geolocation.GetLocationAsync(request);
                if (location != null) {
                    Latitude = location.Latitude;
                    Longitude = location.Longitude;
                } else {
                    await _dialogService.Value.DisplayAlertAsync("Coordinates", "Unable to retrieve coordinates.", "OK");
                }
            } catch {
                await _dialogService.Value.DisplayAlertAsync("Coordinates", "Unable to retrieve coordinates.", "OK");
            }

            CoordText = "Coordinates find !";
            CoordEnable = false;
        }

        private void CustomCoordAction() {
            Latitude = Latitude;
            Longitude = Longitude;
            CoordText = "Coordinates find ! ";
            CoordEnable = false;
        }

        private async void AddImageAction() {
            var result = await _dialogService.Value.DisplayAlertAsync("Add an Image", "Choose wisely", "Photo", "Galerie");
            if (result) {
                PhotoAction();
            } else {
                GalerieAction();
            }
        }

        public async void PhotoAction() {
            await CrossMedia.Current.Initialize();
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported) {
                return;
            }
            MediaFile file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions {
                Directory = "Fourplaces",
                Name = "capture.jpg",
                PhotoSize = PhotoSize.Small
            });
            if (file == null) {
                return;
            }
            UploadImageAsync(file);
        }

        public async void GalerieAction() {
            await CrossMedia.Current.Initialize();
            if (!CrossMedia.Current.IsPickPhotoSupported) {
                return;
            }
            MediaFile file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions {
                PhotoSize = PhotoSize.Small
            });
            if (file == null) {
                return;
            }
            UploadImageAsync(file);
        }

        public async void UploadImageAsync(MediaFile file) {
            MemoryStream memoryStream = new MemoryStream();
            file.GetStream().CopyTo(memoryStream);
            Response<ImageItem> resp = await _apiClient.PostImage(memoryStream.ToArray(), Controleur._accessToken);
            if (resp.IsSuccess) {
                PhotoButtonEnable = false;
                ImageId = resp.Data.Id;
                ImageIdVisible = true;
            }
        }

        public async void SavePlaceAction() {
            if (string.IsNullOrEmpty(Title) || string.IsNullOrEmpty(Description) || !ImageIdVisible || CoordEnable ) {
                await _dialogService.Value.DisplayAlertAsync("Fields Empty !", "", "OK");
                return;
            }
            CreatePlaceRequest createPlaceRequest = new CreatePlaceRequest() {
                Title = Title,
                Description = Description,
                ImageId = ImageId,
                Latitude = Latitude,
                Longitude = Longitude
            };
            HttpResponseMessage res;
            try {
                res = await _apiClient.Execute(HttpMethod.Post, Urls.URI + Urls.PLACES, createPlaceRequest, Controleur._accessToken);
            } catch {
                await _dialogService.Value.DisplayAlertAsync("Connection Failed", "", "OK");
                return;
            }

            Response resp = await _apiClient.ReadFromResponse<Response>(res);
            if (resp.IsSuccess) {
                await _dialogService.Value.DisplayAlertAsync("Success !", "Place save", "OK");
                await _navigationService.Value.PopAsync();
            } else {
                await _dialogService.Value.DisplayAlertAsync("Failed !", "Place not save...", "OK");
            }
        }
    }
}
