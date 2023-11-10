using TutorApp.Views;
using Xamarin.Forms;

namespace TutorApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));
            Routing.RegisterRoute(nameof(TutorRatingPage), typeof(TutorRatingPage));
            Routing.RegisterRoute(nameof(MeetingPage), typeof(MeetingPage));
            Routing.RegisterRoute(nameof(AddMeetingPage), typeof(AddMeetingPage));
            Routing.RegisterRoute(nameof(FilterMeetingPage), typeof(FilterMeetingPage));
            Routing.RegisterRoute(nameof(MeetingReviewPage), typeof(MeetingReviewPage));
            Routing.RegisterRoute(nameof(ScheduleMeetingPage), typeof(ScheduleMeetingPage));
        }
    }
}
