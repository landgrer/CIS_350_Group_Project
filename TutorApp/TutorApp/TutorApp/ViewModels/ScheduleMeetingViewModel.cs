using System;
using System.Collections.Generic;
using TutorApp.Classes;
using TutorApp.Models;
using TutorApp.Services;
using Xamarin.Forms;

namespace TutorApp.ViewModels
{
    public class ScheduleMeetingViewModel : BaseViewModel
    {
        #region Properties
        private DatabaseClient database = DatabaseClient.GetInstance();
        private MeetingClient client = new MeetingClient();
        public Command SubmitCommand { get; set; }
        public EventHandler DateChangedCommand { get; set; }
        public Meeting Tutor { get; set; } = new Meeting();

        string _name = string.Empty;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        string _studentName = string.Empty;
        public string StudentName
        {
            get { return _studentName; }
            set { SetProperty(ref _studentName, value); }
        }

        string _role = string.Empty;
        public string Role
        {
            get { return _role; }
            set { SetProperty(ref _role, value); }
        }

        string _subject = string.Empty;
        public string Subject
        {
            get { return _subject; }
            set { SetProperty(ref _subject, value); }
        }

        string _day = DateTime.Now.ToString("MM/dd/yyyy");
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
            set { SetProperty(ref _timeOptions, value); }
        }

        List<string> _dayOptions = new List<string>();
        public List<string> DayOptions
        {
            get { return _dayOptions; }
            set { SetProperty(ref _dayOptions, value); }
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

        #region Constructors
        public ScheduleMeetingViewModel(Meeting meeting)
        {
            Title = "Schedule Meeting";
            SubmitCommand = new Command(OnScheduledSubmitClicked);
            Tutor = meeting;
        }
        #endregion

        #region Events
        public async void OnAppearing()
        {
            Name = Tutor.Name;
            Subject = Tutor.Subject;
            DateTime startTime = Convert.ToDateTime(Tutor.StartTime);
            DateTime endTime = Convert.ToDateTime(Tutor.EndTime);
            DayOptions = await client.GetDayOptions(startTime, endTime);
            Day = DayOptions[0];

            // If profile already exists, go to meeting page.
            if (await database.HasUserProfile())
                StudentName = $"{database.User.FirstName} {database.User.LastName}";
        }

        private async void OnScheduledSubmitClicked(object obj)
        {
            var meeting = ScheduleNewMeeting();

            if (await client.IsMeetingValid(meeting) == false)
                return;

            DateTime studentStartTime = Convert.ToDateTime(meeting.StartTime);
            DateTime studentEndTime = Convert.ToDateTime(meeting.EndTime);
            DateTime tutorStartTime = Convert.ToDateTime(Tutor.StartTime);
            DateTime tutorEndTime = Convert.ToDateTime(Tutor.EndTime);

            if (studentStartTime.Ticks >= tutorStartTime.Ticks)
            {
                var newMeeting = SetupNewMeeting(Tutor.StartTime, meeting.StartTime);                
                await database.AddMeeting(newMeeting);
            }

            if (studentEndTime.Ticks <= tutorEndTime.Ticks)
            {
                var newMeeting = SetupNewMeeting(meeting.EndTime, Tutor.EndTime);
                await database.AddMeeting(newMeeting);
            }

            await database.RemoveMeeting(Tutor);

            await database.AddMeeting(meeting);

            // Alert user of update.
            string title = "Scheduled";
            string message = $"{meeting.Name}\n{meeting.StartTime}\n{meeting.EndTime}\n";
            await Application.Current.MainPage.DisplayAlert(title, message, "OK");

            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }
        #endregion

        #region Methods
        private Meeting SetupNewMeeting(string startTime, string endTime)
        {
            Meeting meeting = new Meeting()
            {
                TutorProfileID = Tutor.TutorProfileID,
                Name = Tutor.Name,
                Availability = "Open",
                Role = Tutor.Role,
                Subject = Tutor.Subject,
                StartTime = startTime,
                EndTime = endTime
            };
            return meeting;
        }

        public Meeting ScheduleNewMeeting()
        {
            Meeting meeting = new Meeting()
            {
                TutorProfileID = Tutor.TutorProfileID,
                StudentProfileID = database.DeviceID,
                Name = Tutor.Name,
                Availability = "Scheduled",
                Role = Tutor.Role,
                StudentName = Name,
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
                DateTime startTime = Convert.ToDateTime(Tutor.StartTime);
                DateTime endTime = Convert.ToDateTime(Tutor.EndTime);
                TimeOptions = await client.GetTimeOptions(startTime, endTime);
                StartTime = TimeOptions[0];
            }
        }
        #endregion
    }
}
