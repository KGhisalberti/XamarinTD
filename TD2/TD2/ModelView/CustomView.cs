using Storm.Mvvm.Services;
using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Storm.Mvvm;
using Storm.Mvvm.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TD2.View;
using Xamarin.Forms;
using TD2.ModelView;

namespace TD2.ViewModel {
    class CustomView : ViewModelBase {

        private readonly ApiClient _apiClient = new ApiClient();
        private readonly Lazy<IDialogService> _dialogService;
        private readonly Lazy<INavigationService> _navigationService;

        private string _firstName;
        private string _lastName;
        private int _imageId;
        private string _oldPassword;
        private string _newPassword;
        private bool _photoButtonEnable;
        private bool _imageIdVisible;

        public string FirstName {
            get => _firstName;
            set => SetProperty(ref _firstName, value);
        }
        public string LastName {
            get => _lastName;
            set => SetProperty(ref _lastName, value);
        }

        public int ImageId {
            get => _imageId;
            set => SetProperty(ref _imageId, value);
        }

        public string OldPassword {
            get => _oldPassword;
            set => SetProperty(ref _oldPassword, value);
        }

        public string NewPassword {
            get => _newPassword;
            set => SetProperty(ref _newPassword, value);
        }

        public bool ImageIdVisible {
            get => _imageIdVisible;
            set => SetProperty(ref _imageIdVisible, value);
        }

        public bool PhotoButtonEnable {
            get => _photoButtonEnable;
            set => SetProperty(ref _photoButtonEnable, value);
        }

        public ICommand PostPasswordCommand { get; }

        public ICommand PostChangeCommand { get; }

        public ICommand AddImageCommand { get; }

        //Constructeur
        public CustomView() {
            _dialogService = new Lazy<IDialogService>(() => DependencyService.Resolve<IDialogService>());
            _navigationService = new Lazy<INavigationService>(() => DependencyService.Resolve<INavigationService>());

            PhotoButtonEnable = true;

            LoadUserAction();
            PostPasswordCommand = new Command(PostPasswordAction);
            PostChangeCommand = new Command(PostChangeAction);
            AddImageCommand = new Command(AddImageAction);
        }

        private async void LoadUserAction() {
            HttpResponseMessage res;
            try {
               res = await _apiClient.Execute(HttpMethod.Get, Urls.URI + Urls.ME, null, Controleur._accessToken);
            } catch {
                await _dialogService.Value.DisplayAlertAsync("Connection Failed...", "", "Ok");
                return;
            }
            Response<UserItem> resp = await _apiClient.ReadFromResponse<Response<UserItem>>(res);
            if (resp.IsSuccess) {
                FirstName = resp.Data.FirstName;
                LastName = resp.Data.LastName;
                ImageId = Convert.ToInt32(resp.Data.ImageId);
            } else {
                await _dialogService.Value.DisplayAlertAsync("Failure of loading data", "", "Ok");
            }
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

        public async void PostChangeAction() {
            if (string.IsNullOrEmpty(FirstName) || string.IsNullOrEmpty(LastName) || !ImageIdVisible || PhotoButtonEnable) {
                await _dialogService.Value.DisplayAlertAsync("Fields Empty !", "", "OK");
                return;
            } else { 
                UpdateProfileRequest content = new UpdateProfileRequest() { 
                    FirstName = FirstName,
                    LastName = LastName,
                    ImageId = ImageId,
                };
                HttpResponseMessage res;
                try {
                    res = await _apiClient.Execute(new HttpMethod("PATCH"), Urls.URI + Urls.ME, content, Controleur._accessToken);
                } catch {
                    await _dialogService.Value.DisplayAlertAsync("Connection Failed", "", "OK");
                    return;
                }
                Response<UserItem> resp = await _apiClient.ReadFromResponse<Response<UserItem>>(res);
                if (resp.IsSuccess) {
                    await _dialogService.Value.DisplayAlertAsync("Success", "Profile updated !", "Ok");
                    await _navigationService.Value.PopAsync();
                } else {
                    await _dialogService.Value.DisplayAlertAsync("Failed !", "Profile not updated...", "OK");
                }
            }

        }

        public async void PostPasswordAction() {
            if (string.IsNullOrEmpty(OldPassword) || string.IsNullOrEmpty(NewPassword)) {
                await _dialogService.Value.DisplayAlertAsync("Fields Empty !", "", "OK");
                return;
            } else {
                UpdatePasswordRequest content = new UpdatePasswordRequest() {
                    OldPassword = OldPassword,
                    NewPassword = NewPassword,
                };
                HttpResponseMessage res;
                try {
                    res = await _apiClient.Execute(new HttpMethod("PATCH"), Urls.URI + Urls.UPDATE_PASSWORD, content, Controleur._accessToken);
                } catch {
                    await _dialogService.Value.DisplayAlertAsync("Connection Failed", "", "OK");
                    return;
                }
                Response resp = await _apiClient.ReadFromResponse<Response>(res);
                if (resp.IsSuccess) {
                    await _dialogService.Value.DisplayAlertAsync("Success", "Password updated !", "Ok");
                    await _navigationService.Value.PopAsync();
                } else {
                    await _dialogService.Value.DisplayAlertAsync("Failed !", "Password not updated...", "OK");
                }
            }

        }

    }
}