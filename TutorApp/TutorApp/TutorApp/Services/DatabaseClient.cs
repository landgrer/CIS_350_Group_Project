using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TutorApp.Models;

namespace TutorApp.Services
{
    public class DatabaseClient
    {
        private static readonly DatabaseClient instance = new DatabaseClient();
        private bool filter = false;
        private List<Meeting> meetings = new List<Meeting>();
        private List<Meeting> filteredMeetings = new List<Meeting>();

        public DatabaseClient()
        {
            meetings = FakeDataBase.Meetings;
        }

        public static DatabaseClient GetInstance()
        {
            return instance;
        }

        public Task AddMeeting(Meeting meeting)
        {
            meetings.Add(meeting);
            return Task.CompletedTask;
        }

        public Task RemoveMeeting(Meeting person)
        {
            meetings.Remove(person);
            return Task.CompletedTask;
        }

        public Task<List<Meeting>> GetMeetings()
        {
            if (filter)
                return Task.FromResult(filteredMeetings);
            else
                return Task.FromResult(meetings);
        }

        public Task ClearFilter()
        {
            filter = false;
            filteredMeetings.Clear();
            return Task.CompletedTask;
        }

        public Task FilterMeetings(string subject)
        {
            filter = true;
            filteredMeetings.Clear();
            foreach (Meeting meeting in meetings)
                if (meeting.Subject.Equals(subject))
                    filteredMeetings.Add(meeting);
            return Task.CompletedTask;
        }

        public Task FilterMeetings(DateTime startTime, DateTime endTime)
        {
            filter = true;
            long start = startTime.Ticks;
            long end = endTime.Ticks;
            filteredMeetings.Clear();
            foreach (Meeting meeting in this.meetings)
            {
                DateTime tutorStartTime = Convert.ToDateTime(meeting.StartTime);
                long tutorStart = tutorStartTime.Ticks;
                long tutorEnd = tutorStartTime.Ticks;
                if (start < tutorEnd && end > tutorStart)
                    filteredMeetings.Add(meeting);
            }
            return Task.CompletedTask;
        }

        public Task FilterMeetings(DateTime startTime, DateTime endTime, string subject)
        {
            filter = true;
            long start = startTime.Ticks;
            long end = endTime.Ticks;
            filteredMeetings.Clear();
            foreach (Meeting meeting in this.meetings)
            {
                DateTime tutorStartTime = Convert.ToDateTime(meeting.StartTime);
                long tutorStart = tutorStartTime.Ticks;
                long tutorEnd = tutorStartTime.Ticks;
                if (meeting.Subject.Equals(subject))
                    if (start < tutorEnd && end > tutorStart)
                        filteredMeetings.Add(meeting);
            }
            return Task.CompletedTask;
        }
    }
}
