using Storm.Mvvm.Forms;
using TD2.ViewModel;
using Xamarin.Forms.Xaml;

namespace TD2.View {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PlaceItemPage : BaseContentPage {
        public PlaceItemPage() {
            BindingContext = new PlaceItemView();
            InitializeComponent();
           
        }
    }
}