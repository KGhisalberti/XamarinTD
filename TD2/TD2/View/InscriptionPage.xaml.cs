using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TD2.ModelView;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TD2.View {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InscriptionPage : ContentPage {
        public InscriptionPage() {
            InitializeComponent();
            BindingContext = new RegisterView();
        }
    }
}