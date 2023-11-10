using TutorApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TutorApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyReviewPage : ContentPage
    {
        MyReviewViewModel _viewModel;

        public MyReviewPage()
        {
            InitializeComponent();
            BindingContext = _viewModel = new MyReviewViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }
    }
}