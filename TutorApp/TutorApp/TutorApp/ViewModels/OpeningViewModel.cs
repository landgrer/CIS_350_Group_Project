using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TutorApp.Models;
using TutorApp.Services;
using TutorApp.Views;
using Xamarin.Forms;

namespace TutorApp.ViewModels
{
    public class OpeningViewModel : BaseViewModel
    {
        #region Properties
        private DatabaseClient database = DatabaseClient.GetInstance();
        public Command SubmitCommand { get; set; }
        public EventHandler DateChangedCommand { get; set; }
        public Meeting Tutor { get; set; } = new Meeting();
        public List<string> DayOptions { get; set; } = new List<string>();
        public List<string> Subjects { get; set; } = new List<string>();
        public List<string> Roles { get; set; } = new List<string>();

        string _name = string.Empty;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
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
            set { SetProperty(ref _day, value); OnDateChanged(); }
        }

        List<string> _timeOptions = new List<string>();
        public List<string> TimeOptions
        {
            get { return _timeOptions; }
            set { SetProperty(ref _timeOptions, value); }
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
        public OpeningViewModel()
        {
            Title = "Welcome";
            SubmitCommand = new Command(OnSubmitClicked);
            SetupDayOptions().GetAwaiter();
            SetupTimeOptions().GetAwaiter();
            SetupSubjects().GetAwaiter();
            SetupRoles().GetAwaiter();
        }

        public OpeningViewModel(Meeting meeting)
        {
            Title = "Schedule Meeting";
            SubmitCommand = new Command(OnScheduledSubmitClicked);
            Tutor = meeting;
            SetupDayOptions(meeting).GetAwaiter();
            SetupTimeOptions(meeting).GetAwaiter();
            SetupSubjects(meeting).GetAwaiter();
            SetupRoles(meeting).GetAwaiter();
        }
        #endregion

        #region Events
        private async void OnSubmitClicked(object obj)
        {
            if (await IsEntryValid() == false)
                return;

            var meeting = new Meeting()
            {
                Name = Name,
                Availability = "Open",
                Role = Role,
                Subject = Subject,
                StartTime = Convert.ToDateTime($"{Day} {StartTime}").ToString(),
                EndTime = Convert.ToDateTime($"{Day} {EndTime}").ToString()
            };

            if (Role.Equals("Tutor"))
                await database.AddMeeting(meeting);
            else
                await database.FilterMeetings(Convert.ToDateTime(meeting.StartTime), Convert.ToDateTime(meeting.EndTime), meeting.Subject);

            // Prefixing with `//` switches to a different navigation stack instead of pushing to the active one
            await Shell.Current.GoToAsync($"//{nameof(MeetingPage)}?");
        }

        private async void OnScheduledSubmitClicked(object obj)
        {
            if (await IsEntryValid() == false)
                return;

            var meeting = new Meeting()
            {
                Name = Tutor.Name,
                Availability = "Scheduled",
                Role = Role,
                StudentName = Name,
                Subject = Subject,
                StartTime = Convert.ToDateTime($"{Day} {StartTime}").ToString(),
                EndTime = Convert.ToDateTime($"{Day} {EndTime}").ToString()
            };

            DateTime personStartTime = Convert.ToDateTime(meeting.StartTime);
            DateTime personEndTime = Convert.ToDateTime(meeting.EndTime);
            DateTime tutorStartTime = Convert.ToDateTime(Tutor.StartTime);
            DateTime tutorEndTime = Convert.ToDateTime(Tutor.EndTime);

            if (personStartTime.Subtract(tutorStartTime).Minutes > 0)
            {
                var newMeeting = new Meeting()
                {
                    Name = Tutor.Name,
                    Availability = "Open",
                    Role = Tutor.Role,
                    Subject = Tutor.Subject,
                    StartTime = Tutor.StartTime,
                    EndTime = meeting.StartTime
                };
                await database.AddMeeting(newMeeting);
            }

            if (personEndTime.Subtract(tutorEndTime).Minutes < 0)
            {
                var newMeeting = new Meeting()
                {
                    Name = Tutor.Name,
                    Availability = "Open",
                    Role = Tutor.Role,
                    Subject = Tutor.Subject,
                    StartTime = meeting.EndTime,
                    EndTime = Tutor.EndTime
                };
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

        public async void OnDateChanged()
        {
            if (string.IsNullOrEmpty(Tutor.Name))
                await SetupTimeOptions();
            else
                await SetupTimeOptions(Tutor);
        }
        #endregion

        #region Validate Entry
        private async Task<bool> IsEntryValid()
        {
            bool valid = true;
            string message = string.Empty;
            if (string.IsNullOrEmpty(Name))
                message += "Name is empty.\n";
            if (string.IsNullOrEmpty(Role))
                message += "Role is empty.\n";
            if (string.IsNullOrEmpty(StartTime))
                message += "Start time is empty.\n";
            if (string.IsNullOrEmpty(EndTime))
                message += "End time is empty.\n";

            if (string.IsNullOrEmpty(StartTime) == false && string.IsNullOrEmpty(EndTime) == false)
            {
                DateTime startTime = Convert.ToDateTime(StartTime);
                DateTime endTime = Convert.ToDateTime(EndTime);
                if (endTime.Subtract(startTime).TotalMinutes < 1)
                    message += "Start time cannot be the same as or later than the end time.\n";
            }

            if (string.IsNullOrEmpty(message) == false)
            {
                await Application.Current.MainPage.DisplayAlert("Alert", message, "OK");
                valid = false;
            }

            return valid;
        }
        #endregion

        #region Setup Time Options
        private Task SetupDayOptions()
        {
            DayOptions.Clear();
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now.AddMonths(3);
            startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0);
            endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 0, 0, 0);

            while (startDate.Subtract(endDate).Days <= 0)
            {
                DayOptions.Add(startDate.ToString("MM/dd/yyyy"));
                startDate = startDate.AddDays(1);
            }

            // Set Day to earliest option
            Day = DayOptions[0];

            return Task.CompletedTask;
        }

        private Task SetupDayOptions(Meeting meeting)
        {
            DayOptions.Clear();
            DateTime startDate = Convert.ToDateTime(meeting.StartTime);
            DateTime endDate = Convert.ToDateTime(meeting.EndTime);
            startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0);
            endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 0, 0, 0);

            while (startDate.Subtract(endDate).Days <= 0)
            {
                DayOptions.Add(startDate.ToString("MM/dd/yyyy"));
                startDate = startDate.AddDays(1);
            }

            // Set Day to earliest option
            Day = DayOptions[0];

            return Task.CompletedTask;
        }

        private Task SetupTimeOptions()
        {
            TimeOptions.Clear();
            DateTime startDate = Convert.ToDateTime(Day);
            DateTime endDate = Convert.ToDateTime(Day);

            while (startDate.Day.Equals(endDate.Day))
            {
                //DateTime dateNow = DateTime.Now;
                //if (startDate.Day.Equals(dateNow.Day) && startDate.Subtract(dateNow).Ticks < 0)
                //{
                //    startDate = startDate.AddMinutes(15);
                //    continue;
                //}
                TimeOptions.Add(startDate.ToString("h:mm tt"));
                startDate = startDate.AddMinutes(15);
            }

            return Task.CompletedTask;
        }

        private Task SetupTimeOptions(Meeting meeting)
        {
            TimeOptions.Clear();
            DateTime startDate = Convert.ToDateTime(meeting.StartTime);
            DateTime endDate = Convert.ToDateTime(meeting.EndTime);

            while (startDate.Subtract(endDate).Ticks <= 0)
            {
                TimeOptions.Add(startDate.ToString("h:mm tt"));
                startDate = startDate.AddMinutes(15);
            }

            // Set the start time to the earliest time.
            StartTime = TimeOptions[0];

            return Task.CompletedTask;
        }
        #endregion

        #region Setup Subjects
        private Task SetupSubjects()
        {
            Subjects.Clear();
            Subjects.Add("Math");
            Subjects.Add("English");
            Subjects.Add("Science");
            return Task.CompletedTask;
        }

        private Task SetupSubjects(Meeting meeting)
        {
            Subjects.Clear();
            Subjects.Add(meeting.Subject);
            Subject = Subjects[0];
            return Task.CompletedTask;
        }
        #endregion

        #region Setup Roles
        private Task SetupRoles()
        {
            Roles.Add("Student");
            Roles.Add("Tutor");
            return Task.CompletedTask;
        }

        private Task SetupRoles(Meeting meeting)
        {
            Roles.Clear();
            Roles.Add("Student");
            Role = Roles[0];
            return Task.CompletedTask;
        }
        #endregion
    }
}
