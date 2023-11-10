using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using TutorApp.Models;
using Xamarin.Forms;

namespace TutorApp.Services
{
    public class FirebaseTool
    {
        private IFirebaseConfig firebaseConfig;
        private IFirebaseClient client;

        public FirebaseTool()
        {
            firebaseConfig = new FirebaseConfig() { BasePath = "https://loginwith-dce6a-default-rtdb.firebaseio.com/" };
            client = new FirebaseClient(firebaseConfig);
        }

        #region Meeting Methods
        public async Task<Dictionary<string, Meeting>> GetMeetings()
        {
            Dictionary<string, Meeting> meetings = new Dictionary<string, Meeting>();
            FirebaseResponse response = await client.GetAsync(@"Meetings/");
            string model = response.Body.ToString();
            if (model.Equals("null") == false)
                meetings = JsonConvert.DeserializeObject<Dictionary<string, Meeting>>(model);
            return meetings;
        }

        public async Task Add(Meeting meeting)
        {
            // Add to database.
            var response = await client.SetAsync($"Meetings/{meeting.ID}", meeting);

            // Check if it posted.
            if (response.Body.Length == 0)
                await DisplayError("Failed to add meeting");
        }

        public async Task Remove(Meeting meeting)
        {
            // Removing from database.
            var response = await client.DeleteAsync($"Meetings/{meeting.ID}");

            // Check if it was unsuccessful.
            if (response.StatusCode.Equals(HttpStatusCode.OK) == false)
                await DisplayError("Failed to remove meeting");
        }
        #endregion

        #region Profile Methods
        public async Task<Dictionary<string, Profile>> GetProfiles()
        {
            Dictionary<string, Profile> profiles = new Dictionary<string, Profile>();
            FirebaseResponse response = await client.GetAsync(@"Profiles/");
            string model = response.Body.ToString();
            if (model.Equals("null") == false)
                profiles = JsonConvert.DeserializeObject<Dictionary<string, Profile>>(model);
            return profiles;
        }

        public async Task Add(Profile profile)
        {
            // Add to database.
            var response = await client.SetAsync($"Profiles/{profile.ID}", profile);

            // Check if it was unsuccessful.
            if (response.StatusCode.Equals(HttpStatusCode.OK) == false)
                await DisplayError("Failed to add profile");
        }

        public async Task Remove(Profile profile)
        {
            // Removing from database.
            var response = await client.DeleteAsync($"Profiles/{profile.ID}");

            // Check if it was unsuccessful.
            if (response.StatusCode.Equals(HttpStatusCode.OK) == false)
                await DisplayError("Failed to remove profile");
        }
        #endregion

        #region Profile Methods
        public async Task<Dictionary<string, MeetingRating>> GetRatings()
        {
            Dictionary<string, MeetingRating> ratings = new Dictionary<string, MeetingRating>();
            FirebaseResponse response = await client.GetAsync(@"Ratings/");
            string model = response.Body.ToString();
            if (model.Equals("null") == false)
                ratings = JsonConvert.DeserializeObject<Dictionary<string, MeetingRating>>(model);
            return ratings;
        }

        public async Task Add(MeetingRating rating)
        {
            // Add to database.
            var response = await client.SetAsync($"Ratings/{rating.ID}", rating);

            // Check if it was unsuccessful.
            if (response.StatusCode.Equals(HttpStatusCode.OK) == false)
                await DisplayError("Failed to add rating");
        }

        public async Task Remove(MeetingRating rating)
        {
            // Removing from database.
            var response = await client.DeleteAsync($"Ratings/{rating.ID}");

            // Check if it was unsuccessful.
            if (response.StatusCode.Equals(HttpStatusCode.OK) == false)
                await DisplayError("Failed to remove rating");
        }
        #endregion

        #region Methods
        private async Task DisplayError(string message)
        {
            string title = "Error";
            await Application.Current.MainPage.DisplayAlert(title, message, "OK");
        }
        #endregion
    }
}
