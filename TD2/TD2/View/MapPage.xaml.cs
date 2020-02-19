using Storm.Mvvm.Forms;
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
    public partial class MapPage : BaseContentPage {
        public MapPage() {
            MapView view = new MapView();
            BindingContext = view;
            InitializeComponent();
            view.Map(Map);
        }
    }
}