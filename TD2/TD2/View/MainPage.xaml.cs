
using System.ComponentModel;
using Storm.Mvvm.Forms;
using TD2;

namespace TD2.View {
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : BaseContentPage {
        public MainPage() { 
            BindingContext = new MainPageModelView();
            InitializeComponent();
        }
    }
}