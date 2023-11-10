using System.Collections.ObjectModel;
using System.Linq;
using TutorApp.Models;
using TutorApp.Services;
using Xamarin.Forms;

namespace TutorApp.ViewModels
{
    public class RatingViewModel : BaseViewModel
    {
        #region Properties
        public Command SubmitCommand { get; set; }
        public Command ReviewSelectedCommand { get; set; }

        private DatabaseClient database = DatabaseClient.GetInstance();

        ObservableCollection<MeetingRating> _meetingRatings = new ObservableCollection<MeetingRating>();
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
        public RatingViewModel()
        {
            Title = "Ratings";
            SubmitCommand = new Command(OnSubmitClicked);
            ReviewSelectedCommand = new Command(OnCollectionViewSelectionChanged);
            SelectedRating = MeetingRatings.FirstOrDefault();
        }

        public RatingViewModel(Meeting meeting)
        {
            Title = "Ratings";
            SubmitCommand = new Command(OnSubmitClicked);
            ReviewSelectedCommand = new Command(OnCollectionViewSelectionChanged);
            SelectedRating = MeetingRatings.FirstOrDefault();
        }
        #endregion

        #region Events
        public async void OnAppearing()
        {
            MeetingRatings.Clear();
            foreach (var rating in await database.GetRatings())
                MeetingRatings.Add(rating.Value);
        }

        private async void OnSubmitClicked(object obj)
        {

        }

        private async void OnCollectionViewSelectionChanged(object obj)
        {
            if (SelectedRating.ID.Equals(database.ProfileID))
            {
                string title = "Delete Review?";
                string message = "";
                bool delete = await Application.Current.MainPage.DisplayAlert(title, message, "Delete", "Cancel");
                if (delete)
                {
                    await database.RemoveRating(SelectedRating);
                }
            }
        }
        #endregion
    }
}
