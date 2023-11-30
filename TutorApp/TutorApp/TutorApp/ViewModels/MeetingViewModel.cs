using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TutorApp.Models;
using TutorApp.Services;
using TutorApp.Views;
using Xamarin.Forms;

namespace TutorApp.ViewModels
{
    public class MeetingViewModel : BaseViewModel
    {
        #region Properties
        private DatabaseClient database = DatabaseClient.GetInstance();
        public Command MeetingSelectedCommand { get; }
        public Command AddMeetingCommand { get; }
        public Command FilterMeetingCommand { get; }
        public Command RefreshCommand { get; }

        private Meeting _selectedMeeting = new Meeting();
        public Meeting SelectedMeeting
        {
            get { return _selectedMeeting; }
            set { SetProperty(ref _selectedMeeting, value); }
        }

        private string _filterText;
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
            AddMeetingCommand = new Command(OnAddMeeting);
            FilterMeetingCommand = new Command(OnFilterMeetings);
            RefreshCommand = new Command(OnRefresh);
        }
        #endregion

        #region Events
        public async void OnAppearing()
        {
            await GetMeetings();
        }

        private async void OnTextChanged()
        {
            await GetMeetings();

            string searchName = FilterText;
            if (string.IsNullOrWhiteSpace(searchName) || string.IsNullOrEmpty(searchName))
                return;

            searchName = searchName.ToLower();

            List<Meeting> filteredMeetings = new List<Meeting>();
            if (string.IsNullOrEmpty(searchName) == false)
                foreach (Meeting item in Meetings)
                    if (item.Name.ToLower().Contains(searchName))
                        filteredMeetings.Add(item);

            if (string.IsNullOrEmpty(searchName) == false)
            {
                Meetings.Clear();
                foreach (Meeting meeting in filteredMeetings)
                    Meetings.Add(meeting);
            }
        }

        private async void OnCollectionViewSelectionChanged(object obj)
        {
            string[] actions = GetActions();

            if (actions.Length > 0)
            {
                string name = SelectedMeeting.Name;
                string availability = SelectedMeeting.Availability;
                string startTime = SelectedMeeting.StartTime;
                string endTime = SelectedMeeting.EndTime;
                string title = "Select Meeting Action";
                string action = await Application.Current.MainPage.DisplayActionSheet(title, "Cancel", null, actions);
                switch (action)
                {
                    case "Schedule":
                        ScheduleMeetingPage schedulePage = new ScheduleMeetingPage(SelectedMeeting);
                        await Shell.Current.Navigation.PushAsync(schedulePage);
                        break;
                    case "Cancle Meeting":
                        await CancelMeeting();
                        await GetMeetings();
                        break;
                    case "Rate":
                        TutorRatingPage tutorRatingPage = new TutorRatingPage(SelectedMeeting);
                        await Shell.Current.Navigation.PushAsync(tutorRatingPage);
                        break;
                    case "Delete":
                        title = "Are you sure you want to delete?";
                        string message = $"{name}  ({availability})\nStart: {startTime}\nEnd:   {endTime}";
                        bool delete = await Application.Current.MainPage.DisplayAlert(title, message, "Delete", "Cancel");
                        if (delete)
                        {
                            await database.RemoveMeeting(SelectedMeeting);
                            await GetMeetings();
                        }
                        break;
                }
            }
        }

        private async void OnAddMeeting(object obj)
        {
            await Shell.Current.GoToAsync(nameof(AddMeetingPage));
        }

        private async void OnFilterMeetings(object obj)
        {
            await Shell.Current.GoToAsync(nameof(FilterMeetingPage));
        }

        private async void OnRefresh(object obj)
        {
            await database.ClearFilteredMeetings();
            await GetMeetings();
        }
        #endregion

        #region Methods
        private async Task GetMeetings()
        {
            Meetings.Clear();
            var meetings = await database.GetMeetings();
            if (meetings.Count > 0)
                foreach (var meeting in meetings)
                    Meetings.Add(meeting.Value);
        }

        private string[] GetActions()
        {
            long time = DateTime.Now.Ticks;
            long endTimeTicks = Convert.ToDateTime(SelectedMeeting.EndTime).Ticks;
            string attendyID = SelectedMeeting.TutorProfileID;
            string profileID = database.DeviceID;
            bool userAccess = attendyID.Equals(profileID) || attendyID.Equals(profileID);
            bool pastScheduling = endTimeTicks <= time;
            bool available = SelectedMeeting.Availability.Equals("Open");
            List<string> actions = new List<string>();

            // Meeting can be scheduled
            if (pastScheduling == false && available)
                actions.Add("Schedule");
            // Meeting can be cancled
            if (pastScheduling == false && available == false && userAccess)
                actions.Add("Cancle Meeting");
            // Meeting was scheduled and has finished
            if (pastScheduling && userAccess && available == false)
                actions.Add("Rate");
            // Meeting is outdated
            if (pastScheduling && userAccess)
                actions.Add("Delete");
            
            return actions.ToArray();
        }   
        
        private async Task CancelMeeting()
        {
            Meeting meeting = new Meeting()
            {
                TutorProfileID = SelectedMeeting.TutorProfileID,
                Name = SelectedMeeting.Name,
                Availability = "Open",
                Role = "Tutor",
                Subject = SelectedMeeting.Subject,
                StartTime = SelectedMeeting.StartTime,
                EndTime = SelectedMeeting.EndTime
            };
            await database.RemoveMeeting(SelectedMeeting);
            await database.AddMeeting(meeting);
        }
        #endregion
    }
}
