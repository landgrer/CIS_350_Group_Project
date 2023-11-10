using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TutorApp.Models;
using TutorApp.Services;
using Xamarin.Forms;

namespace TutorApp.ViewModels
{
    public class MeetingReviewViewModel : BaseViewModel
    {
        #region Properties
        public Command AllReviewsCommand { get; set; }
        public Command MyReviewsCommand { get; set; }
        public Command ReviewSelectedCommand { get; set; }

        private DatabaseClient database = DatabaseClient.GetInstance();

        private ObservableCollection<MeetingRating> _meetingRatings = new ObservableCollection<MeetingRating>();
        public ObservableCollection<MeetingRating> MeetingRatings
        {
            get { return _meetingRatings; }
            set { SetProperty(ref _meetingRatings, value); }
        }

        private MeetingRating _selectedRating = new MeetingRating();
        public MeetingRating SelectedRating
        {
            get { return _selectedRating; }
            set { SetProperty(ref _selectedRating, value); }
        }
        #endregion

        #region Constructors
        public MeetingReviewViewModel()
        {
            AllReviewsCommand = new Command(OnAllReviews);
            MyReviewsCommand = new Command(OnMyReviews);
            ReviewSelectedCommand = new Command(OnCollectionViewSelectionChanged);
        }
        #endregion

        #region Events
        public async void OnAppearing()
        {
            await GetAllReviews();
        }

        private async void OnAllReviews(object obj)
        {
            await GetAllReviews();
        }

        private async void OnMyReviews(object obj)
        {
            await GetMyReviews();
        }

        private async void OnCollectionViewSelectionChanged(object obj)
        {
            if (SelectedRating.ID.Equals(database.ProfileID))
            {
                string title = "Alert";
                string message = "Delete review?";
                bool delete = await Application.Current.MainPage.DisplayAlert(title, message, "Delete", "Cancel");
                if (delete)
                {
                    await database.RemoveRating(SelectedRating);
                }
            }
        }
        #endregion

        #region Methods
        private async Task GetMyReviews()
        {
            Title = "My Reviews";
            MeetingRatings.Clear();
            var ratings = await database.GetRatings();
            foreach (var rating in ratings)
                if (rating.Value.TutorProfileID.Equals(database.ProfileID))
                    MeetingRatings.Add(rating.Value);
        }

        private async Task GetAllReviews()
        {
            Title = "All Reviews";
            MeetingRatings.Clear();
            var ratings = await database.GetRatings();
            foreach (var rating in ratings)
                MeetingRatings.Add(rating.Value);
        }
        #endregion
    }
}
