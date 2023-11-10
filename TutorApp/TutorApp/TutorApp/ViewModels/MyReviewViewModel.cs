using System.Collections.ObjectModel;
using TutorApp.Models;
using TutorApp.Services;

namespace TutorApp.ViewModels
{
    public class MyReviewViewModel : BaseViewModel
    {
        private DatabaseClient database = DatabaseClient.GetInstance();

        ObservableCollection<MeetingRating> _meetingRatings = new ObservableCollection<MeetingRating>();
        public ObservableCollection<MeetingRating> MeetingRatings
        {
            get { return _meetingRatings; }
            set { SetProperty(ref _meetingRatings, value); }
        }

        public MyReviewViewModel()
        {
            Title = "My Reviews";
        }

        public async void OnAppearing()
        {
            MeetingRatings.Clear();
            var ratings = await database.GetRatings();
            foreach (var rating in ratings)
                if (rating.Value.TutorProfileID.Equals(database.ProfileID))
                    MeetingRatings.Add(rating.Value);
        }
    }
}
