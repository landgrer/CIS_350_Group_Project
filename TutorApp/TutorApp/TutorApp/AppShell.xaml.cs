using TutorApp.Views;
using Xamarin.Forms;

namespace TutorApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(OpeningPage), typeof(OpeningPage));
            Routing.RegisterRoute(nameof(MeetingPage), typeof(MeetingPage));
        }
    }
}
