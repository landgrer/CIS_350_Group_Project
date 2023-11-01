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
            return Task.FromResult(meetings);
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

            // Add to list.
            meetings.Add(uniqueID, meeting);
        }

        public Task Remove(Meeting meeting)
        {
            client.Delete($"Meetings/{meeting.ID}");
            meetings.Remove(meeting.ID);
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
        #endregion
    }
}
