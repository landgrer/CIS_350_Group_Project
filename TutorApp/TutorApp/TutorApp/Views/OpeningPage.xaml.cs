using TutorApp.Models;
using TutorApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TutorApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OpeningPage : ContentPage
    {
        public OpeningPage()
        {
            InitializeComponent();
            this.BindingContext = new OpeningViewModel();
        }

        public OpeningPage(Meeting meeting)
        {
            InitializeComponent();
            this.BindingContext = new OpeningViewModel(meeting);
        }
    }
}