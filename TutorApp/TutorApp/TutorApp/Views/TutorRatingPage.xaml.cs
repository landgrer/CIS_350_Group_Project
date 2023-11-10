using TutorApp.Models;
using TutorApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TutorApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TutorRatingPage : ContentPage
    {
        public TutorRatingPage(Meeting meeting)
        {
            InitializeComponent();
            BindingContext = new TutorRatingViewModel(meeting);
        }
    }
}