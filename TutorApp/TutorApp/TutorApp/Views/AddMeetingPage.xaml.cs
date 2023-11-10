using TutorApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TutorApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddMeetingPage : ContentPage
    {
        AddMeetingViewModel _viewModel;

        public AddMeetingPage()
        {
            InitializeComponent();
            BindingContext = _viewModel = new AddMeetingViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }
    }
}