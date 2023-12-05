using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;
using TutorApp.Models;
using TutorApp.Services;

namespace Tests
{
    [TestClass]
    public class DatabaseTests
    {
        #region Meeting Tests
        [TestMethod]
        public async Task Metting_AddAndRemove_ReturnsTrue()
        {
            // Arrange
            DatabaseClient database = DatabaseClient.GetInstance();
            Meeting meeting = new Meeting()
            {
                TutorProfileID = database.DeviceID,
                StudentProfileID = database.DeviceID,
                Name = "UnitTest",
                Role = "Student",
                Availability = "Scheduled",
                Subject = "Math",
                StudentName = "UnitTest",
                StartTime = DateTime.Now.ToString(),
                EndTime = DateTime.Now.AddHours(1).ToString()
            };

            // Act
            await database.AddMeeting(meeting);
            var response = await database.GetMeetings();
            bool result = response.ContainsKey(meeting.ID);

            // Assert
            Assert.IsTrue(result);

            // Act
            await database.RemoveMeeting(meeting);
            response = await database.GetMeetings();
            result = response.ContainsKey(meeting.ID) == false;

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task Metting_ClearFilter_ReturnsTrue()
        {
            // Arrange
            DatabaseClient database = DatabaseClient.GetInstance();
            var actual = await database.GetMeetings();
            string subject = "Math";

            // Act
            bool cleared = false;
            await database.FilterMeetings(subject);
            var response = await database.GetMeetings();
            if (response.Count != actual.Count)
            {
                await database.ClearFilteredMeetings();
                response = await database.GetMeetings();
                cleared = response.Count == actual.Count;
            }

            // Assert
            Assert.IsTrue(cleared);
        }

        [TestMethod]
        public async Task Metting_ClearFilter_ReturnsFalse()
        {
            // Arrange
            DatabaseClient database = DatabaseClient.GetInstance();
            var actual = await database.GetMeetings();
            string subject = "Christmas";

            // Act
            bool cleared = false;
            await database.FilterMeetings(subject);
            var response = await database.GetMeetings();
            if (response.Count != actual.Count)
            {
                await database.ClearFilteredMeetings();
                response = await database.GetMeetings();
                cleared = response.Count == actual.Count;
            }

            // Assert
            Assert.IsFalse(cleared);
        }

        [TestMethod]
        public async Task Metting_FilterBySubject_ReturnsTrue()
        {
            // Arrange
            DatabaseClient database = DatabaseClient.GetInstance();
            string subject = "Math";

            // Act
            await database.ClearFilteredMeetings();
            await database.FilterMeetings(subject);
            var response = await database.GetMeetings();
            bool filtered = response.Count > 0;
            foreach (var meetingResonse in response)
                if (meetingResonse.Value.Subject.Equals(subject) == false)
                    filtered = false;

            // Assert
            Assert.IsTrue(filtered);
        }

        [TestMethod]
        public async Task Metting_FilterBySubject_ReturnsFalse()
        {
            // Arrange
            DatabaseClient database = DatabaseClient.GetInstance();
            string subject = "Christmas";

            // Act
            await database.ClearFilteredMeetings();
            await database.FilterMeetings(subject);
            var response = await database.GetMeetings();
            bool filtered = response.Count > 0;
            foreach (var meetingResonse in response)
                if (meetingResonse.Value.Subject.Equals(subject) == false)
                    filtered = false;

            // Assert
            Assert.IsFalse(filtered);
        }

        [TestMethod]
        public async Task Metting_FilterByTimeFrame_ReturnsTrue()
        {
            // Arrange
            DatabaseClient database = DatabaseClient.GetInstance();
            var unfiltered = await database.GetMeetings();
            Meeting meeting = unfiltered.First().Value;
            DateTime startTime = Convert.ToDateTime(meeting.StartTime);
            DateTime endTime = Convert.ToDateTime(meeting.EndTime);

            // Act
            await database.ClearFilteredMeetings();
            bool result = await database.FilterMeetings(startTime, endTime);
            var filtered = await database.GetMeetings();
            if (unfiltered.Count > filtered.Count)
                result = true;

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task Metting_FilterByTimeFrame_ReturnsFalse()
        {
            // Arrange
            DatabaseClient database = DatabaseClient.GetInstance();
            // Not able to add meetings 100 days in advance through app.
            DateTime startTime = DateTime.Now.AddDays(100);
            DateTime endTime = DateTime.Now.AddDays(100).AddHours(1);

            // Act
            await database.ClearFilteredMeetings();
            bool filtered = await database.FilterMeetings(startTime, endTime);

            // Assert
            Assert.IsFalse(filtered);
        }

        [TestMethod]
        public async Task Metting_FilterBySubjectAndTimeFrame_ReturnsTrue()
        {
            // Arrange
            DatabaseClient database = DatabaseClient.GetInstance();
            var unfiltered = await database.GetMeetings();
            Meeting meeting = unfiltered.First().Value;
            string subject = meeting.Subject;
            DateTime startTime = Convert.ToDateTime(meeting.StartTime);
            DateTime endTime = Convert.ToDateTime(meeting.EndTime);

            // Act
            await database.ClearFilteredMeetings();
            bool result = await database.FilterMeetings(startTime, endTime, subject);
            var filtered = await database.GetMeetings();
            if (unfiltered.Count > filtered.Count)
                result = true;

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task Metting_FilterBySubjectAndTimeFrame_SubjectFail_ReturnsFalse()
        {
            // Arrange
            DatabaseClient database = DatabaseClient.GetInstance();
            var unfiltered = await database.GetMeetings();
            Meeting meeting = unfiltered.First().Value;
            string subject = "Christmas";
            DateTime startTime = Convert.ToDateTime(meeting.StartTime);
            DateTime endTime = Convert.ToDateTime(meeting.EndTime);

            // Act
            await database.ClearFilteredMeetings();
            bool filtered = await database.FilterMeetings(startTime, endTime, subject);

            // Assert
            Assert.IsFalse(filtered);
        }

        [TestMethod]
        public async Task Metting_FilterBySubjectAndTimeFrame_TimeFrameFail_ReturnsFalse()
        {
            // Arrange
            DatabaseClient database = DatabaseClient.GetInstance();
            string subject = "Math";
            // Not able to add meetings 100 days in advance through app.
            DateTime startTime = DateTime.Now.AddDays(100);
            DateTime endTime = DateTime.Now.AddDays(100).AddHours(1);

            // Act
            await database.ClearFilteredMeetings();
            bool filtered = await database.FilterMeetings(startTime, endTime, subject);

            // Assert
            Assert.IsFalse(filtered);
        }
        #endregion

        #region Profile Tests
        [TestMethod]
        public async Task Profile_Add_ReturnsTrue()
        {
            // Arrange
            DatabaseClient database = DatabaseClient.GetInstance();

            // Act
            await database.AddProfile("FirstName", "LastName");
            bool hasProfile = await database.HasUserProfile();

            // Assert
            Assert.IsTrue(hasProfile);
        }

        [TestMethod]
        public async Task Profile_Get_ReturnsTrue()
        {
            // Arrange
            DatabaseClient database = DatabaseClient.GetInstance();
            Profile expected = new Profile() { FirstName = "FirstName", LastName = "LastName", ID = database.DeviceID };
            Profile actual = new Profile();

            // Act
            if (await database.HasUserProfile())
                actual = database.User;

            // Assert
            Assert.AreEqual(expected.FirstName, actual.FirstName);
            Assert.AreEqual(expected.LastName, actual.LastName);
            Assert.AreEqual(expected.ID, actual.ID);
        }

        [TestMethod]
        public async Task Profile_Remove_ReturnsFalse()
        {
            // Arrange
            DatabaseClient database = DatabaseClient.GetInstance();

            // Act
            await database.RemoveProfile();
            bool result = await database.HasUserProfile();

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Profile_Removed_ReturnsAreNotEqual()
        {
            // This test must run after Profile_Remove_ReturnsFalse().
            // Otherwise it will not have been removed.

            // Arrange
            DatabaseClient database = DatabaseClient.GetInstance();
            Profile notExpected = new Profile() { FirstName = "FirstName", LastName = "LastName", ID = database.DeviceID };
            Profile actual = database.User;

            // Assert
            Assert.AreNotEqual(notExpected.FirstName, actual.FirstName);
            Assert.AreNotEqual(notExpected.LastName, actual.LastName);
        }
        #endregion

        #region Rating Tests
        [TestMethod]
        public async Task Rating_Add_ReturnsTrue()
        {
            // Arrange
            DatabaseClient database = DatabaseClient.GetInstance();
            MeetingRating rating = new MeetingRating()
            {
                TutorProfileID = database.DeviceID,
                StudentProfileID = database.DeviceID,
                Stars = 1,
                Comment = "Unit testing the add rating method in DatabaseClient",
                TutorName = "FirstName LastName"
            };

            // Act
            await database.AddRating(rating);
            var response = await database.GetRatings();
            bool result = response.ContainsKey(rating.ID);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task Rating_Remove_ReturnsTrue()
        {
            // Arrange
            DatabaseClient database = DatabaseClient.GetInstance();
            MeetingRating toRemove = new MeetingRating();
            MeetingRating rating = new MeetingRating()
            {
                TutorProfileID = database.DeviceID,
                StudentProfileID = database.DeviceID,
                Stars = 1,
                Comment = "Unit testing the add rating method in DatabaseClient",
                TutorName = "FirstName LastName"
            };

            // Act
            var response = await database.GetRatings();
            foreach (var ratingResponse in response)
            {
                if (ratingResponse.Value.TutorName.Equals(rating.TutorName))
                {
                    toRemove = ratingResponse.Value;
                    break;
                }
            }

            bool result = false;
            if (string.IsNullOrEmpty(toRemove.ID) == false)
            {
                await database.RemoveRating(toRemove);
                response = await database.GetRatings();
                result = response.ContainsKey(toRemove.ID) == false;
            }

            // Assert
            Assert.IsTrue(result);
        }
        #endregion
    }
}
