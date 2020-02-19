using Android;
using Newtonsoft.Json;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Xamarin.Essentials;
using Storm.Mvvm;
using Storm.Mvvm.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Windows.Input;
using TD2.ModelView;
using TD2.View;
using Xamarin.Forms;

namespace TD2{

    class LoginView : ViewModelBase {

       private readonly ApiClient apiClient = new ApiClient();
        private readonly Lazy<INavigationService> _navigationService;
        private readonly Lazy<IDialogService> _dialogService;

        private string _login;
        private string _mdp;

        public string Login {
            get => _login;
            set => SetProperty(ref _login, value);
        }

        public string Mdp {
            get => _mdp;
            set => SetProperty(ref _mdp, value);
        }

        public ICommand GoMainCommand{ get; }

        public ICommand GoRegisterCommand { get; }

        public LoginView() {
            _navigationService = new Lazy<INavigationService>(() => DependencyService.Resolve<INavigationService>());
            _dialogService = new Lazy<IDialogService>(() => DependencyService.Resolve<IDialogService>());
            GoMainCommand = new Command(LoginAction);
            GoRegisterCommand = new Command(RegisterAction);

        }

        private async void LoginAction() {
            var content = new LoginRequest {
                Email = Login,
                Password = Mdp
            };
            HttpResponseMessage res;
            try {
                 res = await apiClient.Execute(HttpMethod.Post, Urls.URI + Urls.LOGIN, content);
            } catch {
                await _dialogService.Value.DisplayAlertAsync("Connection Failed...","","Ok");
                return;
            } 
            Response<LoginResult> resp = await apiClient.ReadFromResponse<Response<LoginResult>>(res);
            if (resp.IsSuccess) {
                Controleur._accessToken = resp.Data.AccessToken;
                Controleur._refreshToken = resp.Data.RefreshToken;
                await _navigationService.Value.PushAsync(new MainPage());
            }
        }

        private async void RegisterAction() {
            await _navigationService.Value.PushAsync(new InscriptionPage());
        }
    }
}