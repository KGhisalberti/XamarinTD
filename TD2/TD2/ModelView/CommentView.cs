using Storm.Mvvm;
using Storm.Mvvm.Navigation;
using Storm.Mvvm.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Windows.Input;
using Xamarin.Forms;

namespace TD2.ModelView {
    public class CommentView : ViewModelBase {

        private readonly ApiClient _apiClient = new ApiClient();
        private readonly Lazy<INavigationService> _navigationService;
        private readonly Lazy<IDialogService> _dialogService;


        private UserItem _user;
        private DateTime _date;
        private string _comment;
        private PlaceItem _place;

        public PlaceItem Place {
            get => _place;
            set => SetProperty(ref _place, value);
        }

        public UserItem User {
            get => _user;
            set => SetProperty(ref _user, value);
        }

        public DateTime Date {
            get => _date;
            set => SetProperty(ref _date, value);
        }

        public string Comment {
            get => _comment;
            set => SetProperty(ref _comment, value);
        }

        public ICommand PostCommand { get; }

        public CommentView() {
            _dialogService = new Lazy<IDialogService>(() => DependencyService.Resolve<IDialogService>());
            _navigationService = new Lazy<INavigationService>(() => DependencyService.Resolve<INavigationService>());
            PostCommand = new Command(PostAction);
        }

        public override void Initialize(Dictionary<string, object> navigationParameters) {
            base.Initialize(navigationParameters);
            Place = GetNavigationParameter<PlaceItem>("PlaceItem");
        }

        private async void PostAction() {
            HttpResponseMessage res;
            var content = new CreateCommentRequest {
                Text = Comment,
            };
            try {
                res = await _apiClient.Execute(HttpMethod.Post, Urls.URI + Urls.PLACES + "/" + _place.Id + Urls.COMMENT,content,Controleur._accessToken);
            } catch {
                await _dialogService.Value.DisplayAlertAsync("Échec de la connexion", "", "Ok");
                return;
            }
            Response resp = await _apiClient.ReadFromResponse<Response>(res);
            if (resp.IsSuccess) {
                await _navigationService.Value.PopAsync();
            } else {
                await _dialogService.Value.DisplayAlertAsync("Échec du post", "", "Ok");
                return;
            }
        }
    }
}