using Storm.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Windows.Input;
using TD2.View;
using Xamarin.Forms;

namespace TD2.ModelView {
    public class RegisterView : ViewModelBase {

        ApiClient apiClient = new ApiClient();

        private string _fname;
        private string _lname;
        private string _login;
        private string _mdp;

        public string Fname {
            get => _fname;
            set => SetProperty(ref _fname, value);
        }

        public string Lname {
            get => _lname;
            set => SetProperty(ref _lname, value);
        }

        public string Login {
            get => _login;
            set => SetProperty(ref _login, value);
        }

        public string Mdp {
            get => _mdp;
            set => SetProperty(ref _mdp, value);
        }

        public ICommand GoToMainCommand { get; }



        public RegisterView() {
            GoToMainCommand = new Command(RegisterAction);
        }

        private async void RegisterAction() {
            var content = new RegisterRequest {
                FirstName = Fname,
                LastName = Lname,
                Email = Login,
                Password = Mdp
            };

            HttpResponseMessage res = await apiClient.Execute(HttpMethod.Post, Urls.URI + Urls.REGISTER, content);
            Response<LoginResult> resp = await apiClient.ReadFromResponse<Response<LoginResult>>(res);
            if (res.IsSuccessStatusCode) {
                System.Diagnostics.Debug.WriteLine("Le token de connexion est : " + resp.Data.AccessToken);
                await NavigationService.PushAsync(new MainPage());
            } else {
                System.Diagnostics.Debug.WriteLine("Problème dans la requête de connexion : " + resp.ErrorMessage);
            }
        }
    }
}