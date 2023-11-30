using System.Threading.Tasks;
using TutorApp.Models;
using TutorApp.Services;
using Xamarin.Forms;

namespace TutorApp.ViewModels
{
    public class TutorRatingViewModel : BaseViewModel
    {
        #region Properties
        private DatabaseClient database = DatabaseClient.GetInstance();
        private Meeting _meeting = new Meeting();
        public Command SubmitCommand { get; }

        string _tutorName = string.Empty;
        public string TutorName
        {
            get { return _tutorName; }
            set { SetProperty(ref _tutorName, value); }
        }

        string _comment = string.Empty;
        public string Comment
        {
            get { return _comment; }
            set { SetProperty(ref _comment, value); }
        }

        int _star = 3;
        public int Star
        {
            get { return _star; }
            set { SetProperty(ref _star, value); }
        }
        #endregion

        #region Constructors
        public TutorRatingViewModel(Meeting meeting)
        {
            Title = "Browse";
            _meeting = meeting;
            TutorName = meeting.Name;
            SubmitCommand = new Command(OnSubmit);
        }
        #endregion

        #region Events
        public async void OnSubmit(object obj)
        {
            MeetingRating meetingRating = SetupNewRating();

            if (await IsValid(meetingRating) == false)
                return;

            await database.AddRating(meetingRating);

            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }
        #endregion

        #region Methods
        private MeetingRating SetupNewRating()
        {
            MeetingRating meetingRating = new MeetingRating()
            {
                TutorProfileID = _meeting.TutorProfileID,
                StudentProfileID = database.DeviceID,
                Stars = Star,
                Comment = Comment,
                TutorName = _meeting.Name
            };
            return meetingRating;
        }

        private async Task<bool> IsValid(MeetingRating meetingRating)
        {
            bool valid = true;
            string message = string.Empty;
            if (string.IsNullOrEmpty(meetingRating.TutorProfileID))
                message += "No tutor ID.\n";
            if (string.IsNullOrEmpty(meetingRating.StudentProfileID))
                message += "No student ID.\n";
            if (string.IsNullOrEmpty(meetingRating.TutorName))
                message += "No tutor name.\n";
            if (meetingRating.Stars < 0 || meetingRating.Stars > 5)
                message += "Error setting star value.\n";
            if (string.IsNullOrEmpty(meetingRating.Comment) || string.IsNullOrWhiteSpace(meetingRating.Comment))
                message += "Please add a comment.\n";

            if (string.IsNullOrEmpty(message) == false)
            {
                await Application.Current.MainPage.DisplayAlert("Alert", message, "OK");
                valid = false;
            }

            return valid;
        }
        #endregion
    }
}
