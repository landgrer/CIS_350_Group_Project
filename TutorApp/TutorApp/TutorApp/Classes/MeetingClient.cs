using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TutorApp.Models;
using Xamarin.Forms;

namespace TutorApp.Classes
{
    public class MeetingClient
    {
        #region Meeting Class Methods
        public async Task<bool> IsMeetingValid(Meeting meeting)
        {
            bool valid = true;
            string message = string.Empty;
            if (string.IsNullOrEmpty(meeting.Name))
                message += "Name is empty.\n";
            if (string.IsNullOrEmpty(meeting.Role))
                message += "Role is empty.\n";
            if (string.IsNullOrEmpty(meeting.StartTime))
                message += "Start time is empty.\n";
            if (string.IsNullOrEmpty(meeting.EndTime))
                message += "End time is empty.\n";

            if (string.IsNullOrEmpty(meeting.StartTime) == false && string.IsNullOrEmpty(meeting.EndTime) == false)
            {
                DateTime startTime = Convert.ToDateTime(meeting.StartTime);
                DateTime endTime = Convert.ToDateTime(meeting.EndTime);
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

        public Task<List<string>> GetSubjects()
        {
            List<string> subjects = new List<string>();
            subjects.Add("Math");
            subjects.Add("English");
            subjects.Add("Science");
            return Task.FromResult(subjects);
        }

        public Task<List<string>> GetDayOptions(DateTime startDate, DateTime endDate)
        {
            List<string> dayOptions = new List<string>();
            startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0);
            endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 0, 0, 0);

            while (startDate.Subtract(endDate).Days <= 0)
            {
                dayOptions.Add(startDate.ToString("MM/dd/yyyy"));
                startDate = startDate.AddDays(1);
            }

            return Task.FromResult(dayOptions);
        }

        public Task<List<string>> GetTimeOptions(DateTime startDate)
        {
            List<string> timeOptions = new List<string>();
            DateTime endDate = startDate;
            startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0);
            endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 0, 0, 0);

            while (startDate.Day.Equals(endDate.Day))
            {
                if (startDate.Ticks > DateTime.Now.Ticks)
                    timeOptions.Add(startDate.ToString("h:mm tt"));
                startDate = startDate.AddMinutes(15);
            }

            return Task.FromResult(timeOptions);
        }

        public Task<List<string>> GetTimeOptions(DateTime startDate, DateTime endDate)
        {
            List<string> timeOptions = new List<string>();

            while (startDate.Ticks < endDate.Ticks)
            {
                timeOptions.Add(startDate.ToString("h:mm tt"));
                startDate = startDate.AddMinutes(15);
            }

            return Task.FromResult(timeOptions);
        }
        #endregion

        #region Filter Class Methods
        public MeetingFilter SetupNewFilter(string day, string startTime, string endTime, string subject)
        {
            MeetingFilter filter = new MeetingFilter();

            if (string.IsNullOrEmpty(startTime) == false && string.IsNullOrWhiteSpace(startTime) == false)
                filter.StartTime = Convert.ToDateTime($"{day} {startTime}").ToString();
            if (string.IsNullOrEmpty(endTime) == false && string.IsNullOrWhiteSpace(endTime) == false)
                filter.EndTime = Convert.ToDateTime($"{day} {endTime}").ToString();
            if (string.IsNullOrEmpty(subject) == false && string.IsNullOrWhiteSpace(subject) == false)
                filter.Subject = subject;

            return filter;
        }

        public async Task<bool> IsFilterValid(MeetingFilter filter)
        {
            bool valid = true;
            string message = string.Empty;

            if (string.IsNullOrEmpty(filter.StartTime) == false || string.IsNullOrEmpty(filter.EndTime) == false)
            {
                if (string.IsNullOrEmpty(filter.StartTime))
                    message += "Missing start time.\n";
                if (string.IsNullOrEmpty(filter.EndTime))
                    message += "Missing end time.\n";
                if (string.IsNullOrEmpty(filter.StartTime) == false && string.IsNullOrEmpty(filter.EndTime) == false)
                {
                    DateTime startTime = Convert.ToDateTime(filter.StartTime);
                    DateTime endTime = Convert.ToDateTime(filter.EndTime);
                    if (endTime.Subtract(startTime).TotalMinutes < 1)
                        message += "Start time cannot be the same as or later than the end time.\n";
                }
            }

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
