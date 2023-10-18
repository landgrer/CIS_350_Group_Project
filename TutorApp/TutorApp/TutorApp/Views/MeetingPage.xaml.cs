using TutorApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TutorApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MeetingPage : ContentPage
    {
        MeetingViewModel _viewModel;

        public MeetingPage()
        {
            InitializeComponent();

            BindingContext = _viewModel = new MeetingViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }
    }
}