using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TutorApp.Models;
using Xamarin.Forms;

namespace TutorApp.Services
{
    public class FirebaseTool
    {
        private IFirebaseConfig firebaseConfig;
        private IFirebaseClient client;
        private Dictionary<string, Meeting> meetings = new Dictionary<string, Meeting>();
        private Dictionary<string, Meeting> filteredMeetings = new Dictionary<string, Meeting>();
        private static FirebaseTool instance = new FirebaseTool();

        public FirebaseTool()
        {
            try
            {
                firebaseConfig = new FirebaseConfig() { BasePath = "https://loginwith-dce6a-default-rtdb.firebaseio.com/" };
                client = new FirebaseClient(firebaseConfig);
                GetData();
            }
            catch (Exception ex)
            {
                Task.Run(async () => { await DisplayError(ex.ToString()); }).GetAwaiter();
            }
        }

        #region Public Methods
        public static FirebaseTool GetInstance()
        {
            return instance;
        }

        public Task<Dictionary<string, Meeting>> GetMeetings()
        {
            // Retuns the complete Dictionary if there are no filtered meetings.
            if (filteredMeetings.Count == 0)
                return Task.FromResult(meetings);

            return Task.FromResult(filteredMeetings);
        }

        public async Task Add(Meeting meeting)
        {
            // This creates a unique ID.
            string uniqueID = meeting.ID = Guid.NewGuid().ToString("N");

            // Add to database.
            var response = client.Set($"Meetings/{uniqueID}", meeting);

            // Check if it posted.
            if (response.Body.Length == 0)
                await DisplayError("Failed to add meeting");

            // Add to Dictionary.
            meetings.Add(uniqueID, meeting);

            // Dictionary changed, clear filtered meetings.
            filteredMeetings.Clear();
        }

        public Task Remove(Meeting meeting)
        {
            // Removing from database.
            client.Delete($"Meetings/{meeting.ID}");

            // Removing from Dictionary.
            meetings.Remove(meeting.ID);

            // Dictionary changed, clear filtered meetings.
            filteredMeetings.Clear();

            return Task.CompletedTask;
        }

        public Task FilterMeetings(string subject)
        {
            filteredMeetings.Clear();

            foreach (var meeting in meetings)
                if (meeting.Value.Subject.Equals(subject))
                    filteredMeetings.Add(meeting.Key, meeting.Value);

            VerifyFilteredMeetings();

            return Task.CompletedTask;
        }

        public Task FilterMeetings(DateTime startTime, DateTime endTime)
        {
            long start = startTime.Ticks;
            long end = endTime.Ticks;
            filteredMeetings.Clear();

            foreach (var meeting in meetings)
            {
                DateTime tutorStartTime = Convert.ToDateTime(meeting.Value.StartTime);
                long tutorStart = tutorStartTime.Ticks;
                long tutorEnd = tutorStartTime.Ticks;
                if (start < tutorEnd && end > tutorStart)
                    filteredMeetings.Add(meeting.Key, meeting.Value);
            }

            VerifyFilteredMeetings();

            return Task.CompletedTask;
        }

        public Task FilterMeetings(DateTime startTime, DateTime endTime, string subject)
        {
            long start = startTime.Ticks;
            long end = endTime.Ticks;
            filteredMeetings.Clear();

            foreach (var meeting in meetings)
            {
                DateTime tutorStartTime = Convert.ToDateTime(meeting.Value.StartTime);
                DateTime tutorEndTime = Convert.ToDateTime(meeting.Value.EndTime);
                long tutorStart = tutorStartTime.Ticks;
                long tutorEnd = tutorEndTime.Ticks;
                if (meeting.Value.Subject.Equals(subject))
                    if (start < tutorEnd && end > tutorStart)
                        filteredMeetings.Add(meeting.Key, meeting.Value);
            }

            VerifyFilteredMeetings();

            return Task.CompletedTask;
        }
        #endregion

        #region Private Methods
        private async Task DisplayError(string message)
        {
            string title = "Error";
            await Application.Current.MainPage.DisplayAlert(title, message, "OK");
        }

        private void GetData()
        {
            FirebaseResponse response = client.Get(@"Meetings/");
            string model = response.Body.ToString();
            if (model.Equals("null") == false)
                meetings = JsonConvert.DeserializeObject<Dictionary<string, Meeting>>(model);

        }

        private void VerifyFilteredMeetings()
        {
            if (filteredMeetings.Count == 0)
            {
                Meeting meeting = new Meeting();
                meeting.Name = "No Meetings Found";
                filteredMeetings.Add("Empty", meeting);
            }  
        }
        #endregion
    }
}
