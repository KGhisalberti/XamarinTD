using Storm.Mvvm;
using System;
using TD2.View;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TD2 {
    public partial class App : MvvmApplication {
        public App():base(() => new ConnectionPage()){
            InitializeComponent();
           

            
        }

        protected override void OnStart() {
        }

        protected override void OnSleep() {
        }

        protected override void OnResume() {
        }
    }
}
