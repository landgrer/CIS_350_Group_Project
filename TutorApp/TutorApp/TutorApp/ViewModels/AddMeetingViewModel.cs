using System;
using System.Collections.Generic;
using TutorApp.Classes;
using TutorApp.Models;
using TutorApp.Services;
using Xamarin.Forms;

namespace TutorApp.ViewModels
{
    public class AddMeetingViewModel : BaseViewModel
    {
        #region Properties
        private DatabaseClient database = DatabaseClient.GetInstance();
        private MeetingClient client = new MeetingClient();
        public Command SubmitCommand { get; set; }

        string _name = string.Empty;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        string _subject = string.Empty;
        public string Subject
        {
            get { return _subject; }
            set { SetProperty(ref _subject, value); }
        }

        List<string> _dayOptions = new List<string>();
        public List<string> DayOptions
        {
            get { return _dayOptions; }
            set { SetProperty(ref _dayOptions, value); }
        }

        string _day = string.Empty;
        public string Day
        {
            get { return _day; }
            set 
            { 
                SetProperty(ref _day, value);
                DayChanged();
            }
        }

        List<string> _timeOptions = new List<string>();
        public List<string> TimeOptions
        {
            get { return _timeOptions; }
            set { SetProperty(ref _timeOptions, value);}
        }

        List<string> _subjects = new List<string>();
        public List<string> Subjects
        {
            get { return _subjects; }
            set { SetProperty(ref _subjects, value); }
        }

        string _startTime = string.Empty;
        public string StartTime
        {
            get { return _startTime; }
            set { SetProperty(ref _startTime, value); }
        }

        string _endTime = string.Empty;
        public string EndTime
        {
            get { return _endTime; }
            set { SetProperty(ref _endTime, value); }
        }
        #endregion

        #region Constructor
        public AddMeetingViewModel()
        {
            Title = "Add Meeting";
            SubmitCommand = new Command(OnSubmitClicked);
        }
        #endregion

        #region Events
        public async void OnAppearing()
        {
            Subjects = await client.GetSubjects();
            DayOptions = await client.GetDayOptions(DateTime.Now, DateTime.Now.AddMonths(3));
            Day = DayOptions[0];

            // If profile already exists, go to meeting page.
            if (await database.HasUserProfile())
                Name = $"{database.User.FirstName} {database.User.LastName}";
        }

        private async void OnSubmitClicked(object obj)
        {
            Meeting meeting = SetupNewMeeting();

            if (await client.IsMeetingValid(meeting) == false)
                return;

            await database.AddMeeting(meeting);

            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }
        #endregion

        #region Methods
        private Meeting SetupNewMeeting()
        {
            Meeting meeting = new Meeting()
            {
                TutorProfileID = database.ProfileID,
                Name = Name,
                Availability = "Open",
                Role = "Tutor",
                Subject = Subject,
                StartTime = Convert.ToDateTime($"{Day} {StartTime}").ToString(),
                EndTime = Convert.ToDateTime($"{Day} {EndTime}").ToString()
            };
            return meeting;
        }

        private async void DayChanged()
        {
            if (string.IsNullOrEmpty(Day) == false)
            {
                TimeOptions = await client.GetTimeOptions(Convert.ToDateTime(Day));
                StartTime = TimeOptions[0];
            }
        }
        #endregion
    }
}
