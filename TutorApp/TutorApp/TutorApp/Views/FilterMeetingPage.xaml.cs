using TutorApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TutorApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FilterMeetingPage : ContentPage
    {
        FilterMeetingViewModel _viewModel;

        public FilterMeetingPage()
        {
            InitializeComponent();
            BindingContext = _viewModel = new FilterMeetingViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }
    }
}