using Plugin.DeviceInfo;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TutorApp.Models;

namespace TutorApp.Services
{
    public class DatabaseClient
    {
        public string ProfileID { get; private set; } = CrossDeviceInfo.Current.Id;
        public Profile User { get; private set; } = new Profile();

        private FirebaseTool firebase = new FirebaseTool();
        private Dictionary<string, Meeting> meetings = new Dictionary<string, Meeting>();
        private Dictionary<string, Meeting> filteredMeetings = new Dictionary<string, Meeting>();
        private Dictionary<string, Profile> profiles = new Dictionary<string, Profile>();
        private Dictionary<string, MeetingRating> ratings = new Dictionary<string, MeetingRating>();
        private static DatabaseClient instance = new DatabaseClient();

        public static DatabaseClient GetInstance()
        {
            return instance;
        }

        #region Meeting Methods
        public async Task AddMeeting(Meeting meeting)
        {
            // This creates a unique ID.
            string uniqueID = meeting.ID = Guid.NewGuid().ToString("N");

            // Add meeting to Firebase and Dictionary.
            await firebase.Add(meeting);
            meetings.Add(uniqueID, meeting);

            // New data introduced, clear filtered data.
            filteredMeetings.Clear(); 
        }

        public async Task RemoveMeeting(Meeting meeting)
        {
            // Remove meeting from Firebase and Dictonary.
            await firebase.Remove(meeting);
            meetings.Remove(meeting.ID);

            // Data changed, clear filtered data.
            filteredMeetings.Clear();
        }

        public async Task<Dictionary<string, Meeting>> GetMeetings()
        {
            if (meetings.Count == 0)
                meetings = await firebase.GetMeetings();
            
            if (filteredMeetings.Count > 0)
                return filteredMeetings;
            
            return meetings;
        }

        public Task ClearFilteredMeetings()
        {
            filteredMeetings.Clear();
            return Task.CompletedTask;
        }

        public Task<bool> FilterMeetings(string subject)
        {
            filteredMeetings.Clear();
            foreach (var meeting in meetings)
                if (meeting.Value.Subject.Equals(subject))
                    filteredMeetings.Add(meeting.Key, meeting.Value);
            bool filtered = filteredMeetings.Count > 0;
            return Task.FromResult(filtered);
        }

        public Task<bool> FilterMeetings(DateTime startTime, DateTime endTime)
        {
            long start = startTime.Ticks;
            long end = endTime.Ticks;
            filteredMeetings.Clear();
            foreach (var meeting in meetings)
            {
                DateTime tutorStartTime = Convert.ToDateTime(meeting.Value.StartTime);
                DateTime tutorEndTime = Convert.ToDateTime(meeting.Value.StartTime);
                long tutorStart = tutorStartTime.Ticks;
                long tutorEnd = tutorEndTime.Ticks;
                if (start < tutorEnd && end > tutorStart)
                    filteredMeetings.Add(meeting.Key, meeting.Value);
            }
            bool filtered = filteredMeetings.Count > 0;
            return Task.FromResult(filtered);
        }

        public Task<bool> FilterMeetings(DateTime startTime, DateTime endTime, string subject)
        {
            long start = startTime.Ticks;
            long end = endTime.Ticks;
            filteredMeetings.Clear();
            foreach (var meeting in meetings)
            {
                DateTime tutorStartTime = Convert.ToDateTime(meeting.Value.StartTime);
                DateTime tutorEndTime = Convert.ToDateTime(meeting.Value.StartTime);
                long tutorStart = tutorStartTime.Ticks;
                long tutorEnd = tutorEndTime.Ticks;
                if (meeting.Value.Subject.Equals(subject))
                    if (start < tutorEnd && end > tutorStart)
                        filteredMeetings.Add(meeting.Key, meeting.Value);
            }
            bool filtered = filteredMeetings.Count > 0;
            return Task.FromResult(filtered);
        }
        #endregion

        #region Profile Methods
        public async Task<bool> HasUserProfile()
        {
            await GetProfiles();

            if (profiles.TryGetValue(ProfileID, out Profile profile))
                User = profile;

            bool success = ProfileID.Equals(User.ID);

            return success;
        }

        public async Task<Dictionary<string, Profile>> GetProfiles()
        {
            if (profiles.Count == 0)
                profiles = await firebase.GetProfiles();

            return profiles;
        }

        public async Task AddProfile(string firstName, string lastName)
        {
            // New profile.
            Profile profile = new Profile()
            {
                FirstName = firstName,
                LastName = lastName,
                ID = ProfileID
            };

            // Remove existing profile.
            await RemoveProfile();

            // Add to Firebase and Dictionary.
            await firebase.Add(profile);
            profiles.Add(ProfileID, profile);
        }

        public async Task RemoveProfile()
        {
            if (await HasUserProfile())
            {
                // Remove Profile from Firebase and Dictonary.
                await firebase.Remove(User);
                profiles.Remove(ProfileID);
            }
        }
        #endregion

        #region Rating Methods
        public async Task<Dictionary<string, MeetingRating>> GetRatings()
        {
            if (ratings.Count == 0)
                ratings = await firebase.GetRatings();

            return ratings;
        }

        public async Task AddRating(MeetingRating rating)
        {
            // This creates a unique ID.
            string uniqueID = rating.ID = Guid.NewGuid().ToString("N");

            // Verify Rating dose not already exist.
            if (ratings.ContainsKey(rating.ID))
                await RemoveRating(rating);

            // Add to Firebase and Dictionary.
            await firebase.Add(rating);
            ratings.Add(uniqueID, rating);
        }

        public async Task RemoveRating(MeetingRating rating)
        {
            // Remove Rating from Firebase and Dictonary.
            await firebase.Remove(rating);
            ratings.Remove(rating.ID);
        }
        #endregion
    }
}
