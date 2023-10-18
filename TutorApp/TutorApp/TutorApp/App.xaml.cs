using System;
using TutorApp.Services;
using TutorApp.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TutorApp
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
