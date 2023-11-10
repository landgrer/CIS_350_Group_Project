using TutorApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TutorApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MeetingReviewPage : ContentPage
    {
        MeetingReviewViewModel _viewModel;

        public MeetingReviewPage()
        {
            InitializeComponent();
            BindingContext = _viewModel = new MeetingReviewViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }
    }
}