using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TutorApp.Models;
using TutorApp.Services;
using TutorApp.Views;
using Xamarin.Forms;

namespace TutorApp.ViewModels
{
    public class MeetingViewModel : BaseViewModel
    {
        #region Properties
        private FirebaseTool database = FirebaseTool.GetInstance();
        public Command MeetingSelectedCommand { get; set; }

        private Meeting _selectedMeeting = new Meeting();
        public Meeting SelectedMeeting
        {
            get { return _selectedMeeting; }
            set { SetProperty(ref _selectedMeeting, value); }
        }

        private string _filterText = string.Empty;
        public string FilterText
        {
            get { return _filterText; }
            set { SetProperty(ref _filterText, value); OnTextChanged(); }
        }

        ObservableCollection<Meeting> _meetings = new ObservableCollection<Meeting>();
        public ObservableCollection<Meeting> Meetings
        {
            get { return _meetings; }
            set { SetProperty(ref _meetings, value); }
        }
        #endregion

        #region Constructor
        public MeetingViewModel()
        {
            Title = "Browse";
            MeetingSelectedCommand = new Command(OnCollectionViewSelectionChanged);
            SelectedMeeting = Meetings.FirstOrDefault();
        }
        #endregion

        #region Events
        public async void OnAppearing()
        {
            Meetings.Clear();
            foreach (var meeting in await database.GetMeetings())
                Meetings.Add(meeting.Value);
        }

        private void OnTextChanged()
        {
            var searchName = FilterText;

            if (string.IsNullOrWhiteSpace(searchName))
                searchName = string.Empty;

            searchName = searchName.ToLower();

            List<Meeting> filteredMeetings = new List<Meeting>();
            if (string.IsNullOrEmpty(searchName) == false)
                foreach (Meeting item in Meetings)
                    if (item.Name.ToLower().Contains(searchName))
                        filteredMeetings.Add(item);

            OnAppearing();
            if (string.IsNullOrEmpty(searchName) == false)
            {
                Meetings.Clear();
                foreach (Meeting meeting in filteredMeetings)
                    Meetings.Add(meeting);
            }
        }

        private async void OnCollectionViewSelectionChanged(object obj)
        {
            if (SelectedMeeting.Availability.Equals("Open") == false)
                return;

            string title = $"{SelectedMeeting.Role}  ({SelectedMeeting.Availability})";
            string message = $"{SelectedMeeting.Name}\n{SelectedMeeting.StartTime}\n{SelectedMeeting.EndTime}\n";
            bool schedule = await Application.Current.MainPage.DisplayAlert(title, message, "Schedule", "Cancel");

            if (schedule)
            {
                OpeningPage openingView = new OpeningPage(SelectedMeeting);
                await Shell.Current.Navigation.PushAsync(openingView);
            }
        }
        #endregion
    }
}
