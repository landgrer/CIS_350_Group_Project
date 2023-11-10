using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TutorApp.Classes;
using TutorApp.Models;
using TutorApp.Services;
using Xamarin.Forms;

namespace TutorApp.ViewModels
{
    public class FilterMeetingViewModel : BaseViewModel
    {
        #region Properties
        private DatabaseClient database = DatabaseClient.GetInstance();
        private MeetingClient client = new MeetingClient();
        public Command SubmitCommand { get; set; }
        public Command ClearCommand { get; set; }
        public EventHandler DateChangedCommand { get; set; }

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
            set { SetProperty(ref _timeOptions, value); }
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

        #region Constructors
        public FilterMeetingViewModel()
        {
            Title = "Filter Meetings";
            SubmitCommand = new Command(OnSubmitCommand);
            ClearCommand = new Command(OnClearCommand);
        }
        #endregion

        #region Events
        public async void OnAppearing()
        {
            Subjects = await client.GetSubjects();
            DayOptions = await client.GetDayOptions(DateTime.Now, DateTime.Now.AddMonths(3));
            //Day = DayOptions[0];
        }

        private async void OnSubmitCommand(object obj)
        {
            MeetingFilter filter = client.SetupNewFilter(Day, StartTime, EndTime, Subject);

            if (await client.IsFilterValid(filter) == false)
                return;

            if (await FilterMeetings(filter))
            {
                // This will pop the current page off the navigation stack
                await Shell.Current.GoToAsync("..");
            }
        }

        private void OnClearCommand(object obj)
        {
            Day = string.Empty;
            Subject = string.Empty;
            StartTime = string.Empty;
            EndTime = string.Empty;
        }
        #endregion

        #region Methods
        private async void DayChanged()
        {
            if (string.IsNullOrEmpty(Day) == false)
            {
                TimeOptions = await client.GetTimeOptions(Convert.ToDateTime(Day));
            }
        }

        private async Task<bool> FilterMeetings(MeetingFilter filter)
        {
            bool successful = false;
            bool subject = string.IsNullOrEmpty(filter.Subject) == false && string.IsNullOrWhiteSpace(filter.Subject) == false;
            bool start = string.IsNullOrEmpty(filter.StartTime) == false && string.IsNullOrWhiteSpace(filter.StartTime) == false;
            bool end = string.IsNullOrEmpty(filter.EndTime) == false && string.IsNullOrWhiteSpace(filter.EndTime) == false;

            if (subject && start == false && end == false)
            {
                successful = await database.FilterMeetings(filter.Subject);
            }
            else if (subject == false && start && end)
            {
                DateTime startTime = Convert.ToDateTime(filter.StartTime);
                DateTime endTime = Convert.ToDateTime(filter.EndTime);
                successful = await database.FilterMeetings(startTime, endTime);
            }
            else if (subject && start && end)
            {
                DateTime startTime = Convert.ToDateTime(filter.StartTime);
                DateTime endTime = Convert.ToDateTime(filter.EndTime);
                successful = await database.FilterMeetings(startTime, endTime, filter.Subject);
            }               
 
            if (successful == false)
            {
                string message = "No meetings found in filtered meetings";
                await Application.Current.MainPage.DisplayAlert("Alert", message, "OK");
            }

            return successful;
        }
        #endregion
    }
}
