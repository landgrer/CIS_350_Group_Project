using System;
using System.Collections.Generic;
using TutorApp.Models;

namespace TutorApp.Services
{
    public class FakeDataBase
    {
        public static List<Meeting> Meetings { get; set; } = new List<Meeting>()
        {
            new Meeting() { Name = "Tony", Role = "Tutor", Availability = "Scheduled", Subject = "English", StudentName = "Chris", StartTime = SetTime(DateTime.Now), EndTime = SetTime(DateTime.Now.AddHours(1).AddMinutes(15)) },
            new Meeting() { Name = "Jane", Role = "Tutor", Availability = "Open", Subject = "Math", StartTime = SetTime(DateTime.Now), EndTime = SetTime(DateTime.Now.AddHours(1)) },
            new Meeting() { Name = "David", Role = "Tutor", Availability = "Scheduled", Subject = "Math", StudentName = "John", StartTime =SetTime(DateTime.Now), EndTime = SetTime(DateTime.Now.AddHours(1)) },
            new Meeting() { Name = "Molly", Role = "Tutor", Availability = "Open", Subject = "Science", StartTime = SetTime(DateTime.Now), EndTime = SetTime(DateTime.Now.AddHours(1).AddMinutes(30)) },
            new Meeting() { Name = "Peter", Role = "Tutor", Availability = "Open", Subject = "Science", StartTime = SetTime(DateTime.Now), EndTime = SetTime(DateTime.Now.AddHours(1)) },
            new Meeting() { Name = "Ben", Role = "Tutor", Availability = "Scheduled", Subject = "English", StudentName = "Chris", StartTime = SetTime(DateTime.Now), EndTime = SetTime(DateTime.Now.AddHours(2).AddMinutes(45)) },
            new Meeting() { Name = "Zack", Role = "Tutor", Availability = "Scheduled", Subject = "Math", StudentName = "Brandon", StartTime = SetTime(DateTime.Now), EndTime = SetTime(DateTime.Now.AddHours(2)) },
            new Meeting() { Name = "Elsa", Role = "Tutor", Availability = "Open", Subject = "English", StartTime = SetTime(DateTime.Now.AddHours(10)), EndTime = SetTime(DateTime.Now.AddHours(12)) },
            new Meeting() { Name = "Gary", Role = "Tutor", Availability = "Open", Subject = "Science", StartTime = SetTime(DateTime.Now.AddDays(2)), EndTime = SetTime(DateTime.Now.AddDays(2).AddHours(2)) },
            new Meeting() { Name = "Mike", Role = "Tutor", Availability = "Scheduled", Subject = "English", StudentName = "John", StartTime =SetTime(DateTime.Now), EndTime = SetTime(DateTime.Now.AddHours(2)) },
        };

        private static string SetTime(DateTime dateTime)
        {
            int minute = dateTime.Minute;
            int newMinute = minute <= 15 ? 15 : minute <= 30 ? 30 : minute <= 45 ? 45 : 0;
            int newHour = minute == 0 ? dateTime.Hour + 1 : dateTime.Hour;
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, newHour, newMinute, 0).ToString();
        }
    }
}
