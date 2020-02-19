using Storm.Mvvm;
using Storm.Mvvm.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TD2.View;
using Xamarin.Forms;

namespace TD2.ModelView {
    public class CommentListView : ViewModelBase {

        private readonly ApiClient _apiClient = new ApiClient();
        private readonly Lazy<INavigationService> _navigationService;
        private readonly Lazy<IDialogService> _dialogService;

        private PlaceItem _place;
        private List<CommentItem> _comList;
        private int _idPlace;

        public PlaceItem Place {
            get => _place;
            set => SetProperty(ref _place, value);
        }

        public List<CommentItem> ComList {
            get => _comList;
            set => SetProperty(ref _comList, value);
        }

        public int IdPlace {
            get => _idPlace;
            set => SetProperty(ref _idPlace, value);
        }
        public ICommand CreateCommentCommand { get; }

        public override void Initialize(Dictionary<string, object> navigationParameters) {
            base.Initialize(navigationParameters);
            IdPlace = GetNavigationParameter<int>("IdPlace");
        }


        public CommentListView() {
            _navigationService = new Lazy<INavigationService>(() => DependencyService.Resolve<INavigationService>());
            _dialogService = new Lazy<IDialogService>(() => DependencyService.Resolve<IDialogService>());
            CreateCommentCommand = new Command(GoAddCommentAction);
        }

        public override async Task OnResume() {
            await base.OnResume();
            HttpResponseMessage res;
            try {
                res = await _apiClient.Execute(HttpMethod.Get, Urls.URI + Urls.PLACES + "/" + IdPlace);
            } catch {
                await _dialogService.Value.DisplayAlertAsync("Échec de la connexion", "", "Ok");
                return;
            }
            Response<PlaceItem> resp = await _apiClient.ReadFromResponse<Response<PlaceItem>>(res);
            if (resp.IsSuccess) {
                Place = resp.Data;
                ComList = resp.Data.Comments;
            }
        }

        private async void GoAddCommentAction() {
            await _navigationService.Value.PushAsync<CommentPage>(new Dictionary<string, object>() {
                { "PlaceItem", _place }
            });
        }
    }
}